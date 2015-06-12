using UnityEngine;
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

		// otherwise, pick the interactive object
		StartCoroutine(PickItem(interactiveObject));
	}


	protected void SetActionHold () {
		Chest chest = interactiveObject && (interactiveObject is Chest) ? (Chest)interactiveObject : null;
		if (chest) {
			//StartCoroutine(OpenChest(chest));
			StartCoroutine(chest.Opening(this));
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

		float dir  = Mathf.Sign(sprite.localScale.x);
		ent.SetThrow(dir);	
	}


	protected void PushItem (GameObject obj) {
		if (state != States.IDLE) { return; }

		Ent ent = obj.GetComponent<Ent>();
		ent.SetPush(input, 0.25f);
	}


	// ===========================================================
	// Loot
	// ===========================================================


	protected virtual void PickCoin (Coin coin) {
		StartCoroutine(coin.Pickup(this));
	}


	protected virtual void PickWeapon (Weapon weapon) {
		StartCoroutine(weapon.Pickup(this));
	}


	public virtual void AddLootToInventory (Loot loot) {
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
			inv.items[inv.items.Count -1].sprite = loot.sprite.GetComponent<SpriteRenderer>().sprite;
		}

		// update hud
		if (this is Player) {
			Player player = (Player)this;
			player.hud.UpdateInventory();
		}
	}


	// ===========================================================
	// Combat
	// ===========================================================

	protected IEnumerator Roll () {
		if (state == States.ROLL || state == States.ATTACK || state == States.HURT) { yield break; }

		state = States.ROLL;
		Audio.play("Audio/sfx/woosh", 0.25f, Random.Range(0.5f, 0.5f));

		// push attacker forward
		float directionX = Mathf.Sign(sprite.localScale.x);
		Vector2 d = directionX * Vector2.right * 2.5f; // + Vector2.up * (IsOnWater() ? 1f : 3f);
		yield return StartCoroutine(PushBackwards(d, 0.4f));

		state = States.IDLE;
	}


	protected override IEnumerator JumpAttack (Ent target) {
		StartCoroutine(base.JumpAttack(target));
		yield break;
	}


	protected IEnumerator Attack () {
		if (state == States.ATTACK || state == States.HURT) { yield break; }
		if (hasAttackedInAir) { yield break; }

		state = States.ATTACK;
		Audio.play("Audio/sfx/woosh", 0.15f, Random.Range(1.0f, 1.5f));
		
		// attack parameters
		float weaponRange = 0.8f;
		float knockback = 1.5f;
		float directionX = Mathf.Sign(sprite.localScale.x);

		// push attacker forward
		Vector2 d = directionX * Vector2.right * 0.5f + Vector2.up * (IsOnWater() ? 1f : 3f);
		StartCoroutine(PushBackwards(d, 0.1f));
		yield return new WaitForSeconds(0.05f);

		// project a ray forward
		Vector2 rayOrigin = new Vector2 (transform.position.x, transform.position.y + sprite.localScale.y / 2);
		//RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.right * directionX, weaponRange, attackCollisionMask);
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, weaponRange, controller.attackCollisionMask);
		Debug.DrawRay(rayOrigin, Vector2.right * directionX * weaponRange, Color.yellow);

		//foreach (RaycastHit2D hit in hits) {
		if (hit) {
			// push target forward
			Ent target = hit.transform.GetComponent<Ent>();
			int dmg = Random.Range(atr.dmg[0], atr.dmg[1]);
			Vector2 dd = directionX * Vector2.right * knockback + Vector2.up * 3;
			StartCoroutine(target.Hurt(dmg, dd));

			// push attacker backwards
			yield return StartCoroutine(PushBackwards(-d / 2 , 0.05f));
		}

		input = Vector2.zero;
		velocity = Vector2.zero;

		yield return new WaitForSeconds(0.1f);
		state = States.IDLE;
		hasAttackedInAir = !controller.collisions.below;
	}


	public override IEnumerator Hurt (int dmg, Vector2 vec) {
		yield return StartCoroutine(base.Hurt(dmg, vec));
	}


	public override IEnumerator Die () {
		Audio.play("Audio/sfx/bite", 0.5f, Random.Range(3f, 3f));

		// instantiate blood splats
		Bleed(Random.Range(8, 16));
		
		// destroy entity
		yield return null;
		Destroy(gameObject);
	}


	public override IEnumerator PushBackwards (Vector2 vec, float duration) {
		yield return StartCoroutine(base.PushBackwards(vec, duration));
	}


	// ===========================================================
	// Triggers
	// ===========================================================

	public override bool TriggerCollisionAttack (GameObject obj) {
		Ent target = obj.GetComponent<Ent>();

		// decide if alive being is gonna hit
		if (velocity.y > -2) { return false; }
		if (transform.position.y < target.transform.position.y + transform.localScale.y * 0.75f) { 
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
		if (state == States.ATTACK) { return; }
		base.OnTriggerStay2D(collider);

		switch (collider.gameObject.tag) {
			case "Ladder":
			ladder = collider.transform.parent.GetComponent<Ladder>();
			break;

			case "Item":
			interactiveObject = collider.gameObject.GetComponent<Ent>();
			if (interactiveObject is Coin) {
				PickCoin((Coin)interactiveObject);
				interactiveObject = null;
			} else if (interactiveObject is Weapon) {
				PickWeapon((Weapon)interactiveObject);
				interactiveObject = null;
			}
			break;

			case "Trap":
			Trap trap = collider.gameObject.GetComponent<Trap>();
			trap.Activate();
			break;

			case "Spike":
			if (state == States.HURT) { break; }
			Ent spike = collider.gameObject.GetComponent<Ent>();
			int dmg = Random.Range(spike.atr.dmg[0], spike.atr.dmg[1]);
			Vector2 d = new Vector2(Mathf.Sign(transform.position.x - spike.transform.position.x), 3);
			StartCoroutine(Hurt(dmg, d));
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
			interactiveObject = null;
			break;
		}
	}

}
