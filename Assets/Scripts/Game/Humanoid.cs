﻿using UnityEngine;
using System.Collections;



public class Humanoid : Ent {


	// ===========================================================
	// Actions
	// ===========================================================
	
	protected void SetAttack (bool isDown) {
		if (state == States.ATTACK || state == States.HURT) { return; };

		if (pickedUpObject) {
			ThrowItem(pickedUpObject);
			return;
		}

		if (isDown) {
			StartCoroutine (Roll());
		} else {
			StartCoroutine(Attack());
		}
	}


	protected void SetAction () {
		// if we are opening a chest, cancel the action
		Chest chest = interactiveObject && (interactiveObject is Chest) ? (Chest)interactiveObject : null;
		if (chest && chest.opening) {
			chest.CancelOpening();
			return;
		}

		// if we are opening a door, cancel the action
		Door door = interactiveObject && (interactiveObject is Door) ? (Door)interactiveObject : null;
		if (door) {
			if (door.opening) { 
				door.CancelOpening(); 
			} else {
				if (door.opened) { 
					door.Enter(this); 
				} else {
					StartCoroutine(door.UpdateInfo("CLOSED", 0.3f));
				}
			}
			return;
		}

		// otherwise, pick the interactive object
		StartCoroutine(PickItem(interactiveObject));
	}


	protected void SetActionHold () {
		// open chests
		Chest chest = interactiveObject && (interactiveObject is Chest) ? (Chest)interactiveObject : null;
		if (chest) {
			StartCoroutine(chest.Opening(this));
			return;
		}

		// open closed doors / enter through open doors
		Door door = interactiveObject && (interactiveObject is Door) ? (Door)interactiveObject : null;
		if (door) {
			if (!door.opened) { 
				StartCoroutine(door.Opening(this)); 
			} 
			return;
		}
	}


	// ===========================================================
	// Item interaction
	// ===========================================================

	protected IEnumerator PickItem (Ent ent) {
		if (pickedUpObject) { 
			StartCoroutine(DropItem(pickedUpObject)); 
		}

		if (!ent) { yield break; }
		if (!ent.pickable) { yield break; }
		
		yield return StartCoroutine(ent.Pickup(this));

		pickedUpObject = ent;
	}


	protected IEnumerator DropItem (Ent ent) {
		if (!ent) { yield break; }

		ent.affectedByGravity = true;
		ent.transform.SetParent(World.itemContainer);

		pickedUpObject = null;
	}


	protected void ThrowItem (Ent ent) {
		if (!ent) { return; }

		StartCoroutine(DropItem(ent));

		float dir  = GetSpriteDirection();
		ent.SetThrow(dir);	
	}


	protected void PushItem (GameObject obj) {
		if (state != States.IDLE) { return; }

		Ent ent = obj.GetComponent<Ent>();
		ent.SetPush(input, 1f);
	}


	// ===========================================================
	// Loot
	// ===========================================================

	public void AddLootToInventory (Loot loot) {
		bool stacked = false;

		// if we already own this item type, increase item number
		for (int n = 0; n < inv.items.Count; n++) {
			InvItem item = inv.items[n];

			if (item.path == loot.path) {
				int num = (loot is Coin) ? loot.value : 1;
				item.num += num;
				item.value = loot.value;
				stacked = true;
			}
		}

		// otherwise, add item to inventory
		if (!stacked) {
			inv.items.Add(new InvItem());
			inv.items[inv.items.Count -1].path = loot.path; 
			inv.items[inv.items.Count -1].num = (loot is Coin) ? loot.value : 1;
			inv.items[inv.items.Count -1].value = loot.value;
			inv.items[inv.items.Count -1].sprite = loot.GetSpriteImage();
		}

		// update hud
		// if (this is Player) {
		// 	Player player = (Player)this;
		// 	player.hud.UpdateInventory();
		// }
	}


	// ===========================================================
	// Combat
	// ===========================================================

	protected IEnumerator Roll () {
		if (state == States.ROLL || state == States.ATTACK || state == States.HURT) { yield break; }

		state = States.ROLL;
		AudioManager.Play("Audio/sfx/woosh", 0.25f, Random.Range(0.5f, 0.5f));

		// push attacker forward
		float directionX = GetSpriteDirection(); //Mathf.Sign(sprite.localScale.x);
		Vector2 d = directionX * Vector2.right * 2.5f; // + Vector2.up * (IsOnWater() ? 1f : 3f);
		yield return StartCoroutine(PushBackwards(d, 0.4f));

		state = States.IDLE;
	}


	protected override IEnumerator JumpAttack (Ent target) {
		StartCoroutine(base.JumpAttack(target));
		yield break;
	}


	protected IEnumerator Attack () {
		if (state == States.ATTACK || state == States.PARRY ||state == States.HURT) { yield break; }
		if (hasAttackedInAir) { yield break; }

		state = States.ATTACK;
    PlayAnimation("attack", 4f);
		AudioManager.Play("Audio/sfx/woosh", 0.15f, Random.Range(1.0f, 1.5f));
		
		// attack parameters
		float weaponRange = 0.8f;
		float knockback = 1.5f;
		float directionX = GetSpriteDirection();

		// push attacker forward
		Vector2 d = directionX * Vector2.right * 0.75f + Vector2.up * (IsOnWater() ? 1f : 3f);
		StartCoroutine(PushBackwards(d, 0.1f));
		yield return new WaitForSeconds(0.05f);

		// get possible attack target
		Ent target = null;

		if (interactiveObject && interactiveObject.destructable) {
			// by distance (in case we are overlapping the target)
			if (Vector2.Distance(transform.position, interactiveObject.transform.position) < 0.3f) {
				target = interactiveObject;
			}
		} else {
			// by projecting a ray forward
			Vector2 rayOrigin = new Vector2 (transform.position.x, transform.position.y + GetHeight() / 2);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, weaponRange, controller.attackCollisionMask);
			Debug.DrawRay(rayOrigin, Vector2.right * directionX * (weaponRange + 0.3f), Color.red);
			if (hit) { 
				target = hit.transform.GetComponent<Ent>();
			} 
		}

		// if we have a destructable target
		if (target && target.destructable) {
	     SetSpriteDirection(Mathf.Sign(target.transform.position.x - transform.position.x));
      
			// if target is attacking lets both parry
			if (target.state == States.ATTACK) {
				Humanoid enemy = (Humanoid)target;
				StartCoroutine(enemy.Parry(this, d * 1));
				yield return StartCoroutine(Parry(enemy, -d * 1));
			} else {
				// calculate damage, hurt target, and push him backwards
				int dmg = Random.Range(atr.dmg[0], atr.dmg[1]);
				Vector2 dd = directionX * Vector2.right * knockback + Vector2.up * 3;
				StartCoroutine(target.Hurt(dmg, dd));

				// push attacker backwards
				yield return StartCoroutine(PushBackwards(-d / 2 , 0.05f));
			}
		}

		// finish attack
		input = Vector2.zero;
		velocity = Vector2.zero;
		yield return new WaitForSeconds(0.1f);
		state = States.IDLE;
		hasAttackedInAir = !controller.collisions.below;
	}


	public override IEnumerator Parry (Humanoid enemy, Vector2 vec) {
		state = States.PARRY;
    PlayAnimation("attack", 4f);
		AudioManager.Play("Audio/sfx/Sword", 1f, Random.Range(1f, 2f));

		input = Vector2.zero;
		velocity = Vector2.zero;

		if (prefabs.parryPrefab) {
			Vector3 pos = transform.position + ((enemy.transform.position - transform.position) / 2) + Vector3.up * GetHeight() / 2;
			Star star = ((GameObject)Instantiate(prefabs.parryPrefab)).GetComponent<Star>();
			star.Init(pos, GetSpriteDirection()); //sprite.localScale.x);
		}
		
		yield return StartCoroutine(PushBackwards(vec , 0.2f));
	}


	public override IEnumerator Hurt (int dmg, Vector2 vec) {
		yield return StartCoroutine(base.Hurt(dmg, vec));
	}


	public override IEnumerator Die () {
		AudioManager.Play("Audio/sfx/bite", 0.5f, Random.Range(3f, 3f));
		Bleed(Random.Range(8, 16));
		SpawnLoot();

		yield return null;
		Destroy(gameObject);
	}


	protected override void Bleed (int dmg, int maxBloodSplats = 20) {
		base.Bleed(dmg, maxBloodSplats);
	}


	public override IEnumerator PushBackwards (Vector2 vec, float duration) {
		yield return StartCoroutine(base.PushBackwards(vec, duration));
	}


	// ===========================================================
	// Triggers
	// ===========================================================

	public override bool TriggerCollisionAttack (GameObject obj) {
		Ent target = obj.GetComponent<Ent>();
		if (!target || target.destructableJumpMass == 0 || target.destructableJumpMass > atr.mass) { return false; }

		// decide if alive being is gonna hit
		if (velocity.y > -1.5f) { return false; }
		if (transform.position.y < target.transform.position.y + GetSpriteDirection() * 0.75f) { //transform.localScale.y * 0.75f) { 
			return false;
		}
		
		// execute jump attack
		StartCoroutine(JumpAttack(target));
		return true;
	}


	public override void TriggerPushable (GameObject obj) {
		PushItem(obj);
	}


	protected override void OutOfBounds () {
		// Kill if is out of bounds
		if (transform.position.y < -1) {
			StartCoroutine(Die());
		}
	}


	protected override void OnTriggerStay2D (Collider2D collider) {
		//if (state == States.ATTACK) { return; }
		
		base.OnTriggerStay2D(collider);

		Ent ent = null;

		switch (collider.gameObject.tag) {
			case "Ladder":
			ladder = collider.transform.parent.GetComponent<Ladder>();
			break;

			case "Item":
			case "Platform":
			case "OneWayPlatform":
			ent = collider.gameObject.GetComponent<Ent>();
			if (ent && (ent.pickable || ent.destructable)) {
				interactiveObject = ent;
			}
			break;

			case "Door":
			ent = collider.gameObject.GetComponent<Ent>();
			if (ent) { interactiveObject = ent; }
			break;

			case "Loot":
			StartCoroutine(collider.gameObject.GetComponent<Loot>().Pickup(this));
			break;
		}
	}


	protected override void OnTriggerExit2D (Collider2D collider) {
		base.OnTriggerExit2D(collider);

		switch (collider.gameObject.tag) {
			case "Ladder":
			ladder = null;
			break;

			case "Item":
			case "Platform":
			case "OneWayPlatform":
			interactiveObject = null;
			break;

			case "Door":
			interactiveObject = null;
			break;
		}
	}

}
