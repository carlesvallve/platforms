using UnityEngine;
using System.Collections;

public enum States {
	IDLE = 0,
	ATTACK = 2,
	HURT = 3
}

[System.Serializable]
public class Stats {
	public int str = 1;
	public int dex = 1;
	public int con = 1;
	public int wis = 1;
	public int cha = 1;
}


[System.Serializable]
public class Atr {
	public int hp = 8;
	public float speed = 5f;
	public float atkSpeed = 1f;
	public int[] dmg = { 1, 3 };
	public float jump = 1f;
}


[System.Serializable]
public class Inv {
	public int coins = 0;
}

	
[RequireComponent (typeof (Controller2D))]
public class Ent : MonoBehaviour {

	public Controller2D controller;

	public States state;
	public Stats stats;
	public Atr atr;
	public Inv inv;

	protected float jumpHeight = 2.5f;
	protected float timeToJumpApex = 0.25f;
	
	public bool isCreature = false;
	public bool affectedByGravity = true;

	public GameObject lootPrefab;
	public GameObject bloodPrefab;
	
	protected Vector2 input;
	protected float speed = 1.0f;
	protected float gravity;
	protected float jumpVelocity;
	protected Vector2 velocity;

	//public float accelerationTimeAirborne = 0;
	//public float accelerationTimeGrounded = 0;
	//protected float velocityXSmoothing;
	//protected float velocityYSmoothing;

	protected bool jumping = false;
	protected bool jumpingDown = false;
	protected bool jumpingFromLadder = false;

	protected Ladder ladder;
	protected Ent interactiveObject;
	protected Ent pickedUpObject;

	protected bool hasAttackedInAir = false;

	protected bool isWater = false;

	protected Transform hpBar;
	protected Transform hpPercent;
	protected float hpMax;

	protected TextMesh info;

	protected GameObject sprite;


	// ===========================================================
	// Init
	// ===========================================================

	public virtual void Awake () {
		controller = GetComponent<Controller2D>();

		sprite = transform.Find("Sprite").gameObject;

		hpBar = transform.Find("Bar");
		hpPercent = transform.Find("Bar/Percent");
		hpMax = (float)atr.hp;
		StartCoroutine(UpdateHpBar());

		Transform obj = transform.Find("Info");
		if (obj) {
			info = obj.GetComponent<TextMesh>();
			StartCoroutine(UpdateInfo(null));
		}

		gravity = -((2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2)); // -80
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex; // 20

		state = States.IDLE;
		stats = new Stats();
	}


	public virtual void Update () {
		Reset();
		SetInput();
		SetSpeed();
		SetMove();
		OutOfBounds();
	}


	// ===========================================================
	// Actions
	// ===========================================================
	
	protected void SetActionB () {
		if (pickedUpObject) {
			ThrowItem(pickedUpObject);
			return;
		}

		StartCoroutine(Attack());
	}


	protected void SetActionC () {
		StartCoroutine(PickItem(interactiveObject));
	}

	protected void SetActionCHold () {
		StartCoroutine(OpenItem(interactiveObject));
	}


	// ===========================================================
	// Movement
	// ===========================================================

	protected void ApplyGravity () {
		if (affectedByGravity) {
			if (IsOnWater()) {
				velocity.y += gravity * 0.1f * Time.deltaTime;
			} else {
				velocity.y += gravity * Time.deltaTime;
			}
		}
	}


	protected void Reset () {
		if (controller.collisions.above || controller.collisions.below) {
			if (jumping) { velocity.x = 0; }
			velocity.y = 0;
			jumping = false;
			jumpingDown = false;
			jumpingFromLadder = false;
		}

		if (velocity.y < -18f && !jumpingFromLadder) {
			jumpingDown = false;
			
		}

		if (velocity.y < -4f && !jumpingDown) {
			jumpingFromLadder = false;
		}
	}


	protected virtual void SetInput () {
		input.x *= 0.99f;
		if (controller.collisions.left || controller.collisions.right) { input.x = 0; }
		if (controller.collisions.above || controller.collisions.below) { input.x *= 0.9f; }
	}


	protected virtual void SetSpeed () {
		speed = atr.speed;
	}


	protected void SetMove () {
		//if (state == States.ATTACK || state == States.HURT) { return; };

		// set velocity x
		float targetVelocityX = input.x * speed;
		velocity.x = targetVelocityX;
		//velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
			
		if (velocity.x != 0) { 
			if (state != States.ATTACK && state != States.HURT) {
				sprite.transform.localScale = new Vector2(Mathf.Sign(velocity.x) * Mathf.Abs(sprite.transform.localScale.x), sprite.transform.localScale.y); 
			}
		}

		// set velocity y
		if (IsOnLadder()) {
			SetMoveOnLadder();
		} else {
			ApplyGravity();
		}

		// apply controller2d movement
		controller.Move (velocity * Time.deltaTime, jumpingDown);

		// snap to ladders
		if (IsOnLadder()) { 
			SnapToLadder(); 
		}
	}


	protected void SetJump (bool isJumpingDown, float intensity = 1) {
		if (jumping && !IsOnWater()) { return; }

		if (IsOnWater()) { intensity *= 0.25f; }

		velocity.y = jumpVelocity * intensity; 

		jumping = true;
		jumpingFromLadder = IsOnLadder();

		if (isJumpingDown) {
			jumpingDown = true;
			velocity.y *= 0.5f;
		}
	}


	protected void OutOfBounds () {
		// Ddestroy if is out of bounds
		if (transform.position.y < -1) {
			if (isCreature) {
				StartCoroutine(Die());
			} else {
				Destroy(gameObject);
			}
		}
	}


	


	// ===========================================================
	// Water
	// ===========================================================

	public bool IsOnWater () {
		return isWater;
	}


	private IEnumerator SetWaterIn () {
		if (isWater || velocity.y >= 0) { yield break; }

		Audio.play("Audio/sfx/splash", 0.05f, Random.Range(0.8f, 1f));
		isWater = true;
		velocity.x *= 0.1f;
		velocity.y = 0;

		yield break;
	}


	private IEnumerator SetWaterOut () {
		if (!isWater || velocity.y < 0) { yield break; }

		Audio.play("Audio/sfx/splash", 0.05f, Random.Range(1f, 1.2f));
		isWater = false;

		if (!IsOnLadder() && isCreature) {
			velocity.y = 0;
			jumping = false;
			SetJump(false, atr.jump * 0.75f); 
		}

		yield break;
	}


	// ===========================================================
	// Ladders
	// ===========================================================

	private bool previouslyOnLadder;


	public bool IsOnLadder() {
		bool onLadder = ladder && !jumpingFromLadder;

		if (state == States.ATTACK) { onLadder = false; }
		
		if (!previouslyOnLadder && velocity.y != 0 && input.y == 0) { onLadder = false; }

		if (ladder && !jumpingFromLadder &&
			transform.position.y - ladder.transform.position.y > ladder.GetHeight() - 1) {
			onLadder = true;
		}

		if (onLadder) { previouslyOnLadder = true; }
		if (controller.landed) { previouslyOnLadder = false; }

		return onLadder;
	}


	protected void SetMoveOnLadder () {
		if (!jumpingFromLadder) {
			if (jumping || (velocity.y < 0 && input.y >= 0)) { PlayAudioStep(); }
			jumping = false;
		}

		float targetVelocityY = input.y * speed;
		velocity.y = targetVelocityY ;
	}


	private void SnapToLadder () {
		if (!controller.landed && !jumpingFromLadder) {
			Vector2 pos = new Vector2(ladder.transform.position.x, transform.position.y);
			transform.position = Vector2.Lerp(transform.position, pos, Time.deltaTime * 20f);
		}
	}


	// ===========================================================
	// Item interaction
	// ===========================================================

	protected IEnumerator OpenItem (Ent ent) {
		if (!ent || !(ent is Item)) { yield break; }
		Item item = (Item)ent;
		
		yield return StartCoroutine(item.Opening(this));
	}


	protected IEnumerator PickItem (Ent ent) {
		if (pickedUpObject) { 
			StartCoroutine(DropItem(pickedUpObject)); 
		}

		if (!ent || !(ent is Item)) { yield break; }
		
		Item item = (Item)ent;
		yield return StartCoroutine(item.Pickup(this));

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

		float dir  = Mathf.Sign(sprite.transform.localScale.x);
		ent.input = new Vector2(dir * 1.5f, 0);
		ent.velocity.y = 10f; 	
	}


	protected void PushItem (GameObject obj) {
		if (state != States.IDLE) { return; }
		Ent ent = obj.GetComponent<Ent>();
		ent.input.x = input.x * 0.25f;
	}


	// ===========================================================
	// Loot interaction
	// ===========================================================

	protected IEnumerator SpawnLoot (int maxLoot, float delay = 0) {
		if (!lootPrefab) { yield break; }

		for (int i = 0; i < maxLoot; i++) {
			Loot loot = ((GameObject)Instantiate(lootPrefab, transform.position, Quaternion.identity)).GetComponent<Loot>();
			loot.Init(World.lootContainer);
			//yield return new WaitForSeconds(delay);
		}
	}


	protected virtual void PickCoin (Coin coin) {
		StartCoroutine(coin.Pickup(this));
	}


	// ===========================================================
	// Combat
	// ===========================================================

	protected IEnumerator JumpAttack (Ent target) {
		// jump
		jumping = false;
		jumpingDown = false;
		SetJump(false,  atr.jump * 0.75f);
		
		// hurt target and knock him back
		float knockback = 1f;
		int dmg = Random.Range(atr.dmg[0], atr.dmg[1]);
		Vector2 d = new Vector2(Mathf.Sign(target.transform.position.x - transform.position.x) * knockback, 3);
		StartCoroutine(target.Hurt(dmg, d));

		yield break;
	}


	protected IEnumerator Attack () {
		if (state == States.ATTACK) { yield break; }
		if (IsOnLadder() && !controller.landed) { yield break; }
		if (hasAttackedInAir) { yield break; }

		state = States.ATTACK;
		Audio.play("Audio/sfx/swishA", 0.025f, Random.Range(0.5f, 0.5f));
		
		// attack parameters
		float weaponRange = 0.8f;
		float knockback = 1.5f;
		float directionX = Mathf.Sign(sprite.transform.localScale.x);

		// push attacker forward
		Vector2 d = directionX * Vector2.right * 0.5f + Vector2.up * (IsOnWater() ? 1f : 3f);
		StartCoroutine(PushBackwards(d, 0.1f));
		yield return new WaitForSeconds(0.05f);

		// project a ray forward
		Vector2 rayOrigin = new Vector2 (transform.position.x, transform.position.y + sprite.transform.localScale.y / 2);
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


	public virtual IEnumerator Hurt (int dmg, Vector2 vec) {
		state = States.HURT;
		input = Vector2.zero;
		velocity = Vector2.zero;

		Audio.play("Audio/sfx/step", 1f, Random.Range(2.5f, 2.5f));

		// update stats
		atr.hp -= dmg;
		if (atr.hp <= 0) { atr.hp = 0; }

		// update hp bar
		StartCoroutine(UpdateHpBar());

		// if no hp left, die instead
		if (atr.hp <= 0) {
			yield return StartCoroutine(Die());
			yield break;
		}

		// make him bleed
		Bleed(Random.Range(3, 6));

		// push backwards
		yield return StartCoroutine(PushBackwards(vec, 0.5f));

		state = States.IDLE;
	}


	public virtual IEnumerator Die () {
		Audio.play("Audio/sfx/bite", 0.5f, Random.Range(3f, 3f));

		// instantiate blood splats
		Bleed(Random.Range(8, 16));
		
		// destroy entity
		yield return null;
		Destroy(gameObject);
	}


	public virtual IEnumerator PushBackwards (Vector2 vec, float duration) {
		velocity.y = vec.y;
		Vector2 pos = new Vector2(transform.position.x + vec.x, transform.position.y);
		
		float startTime = Time.time;
		while (Time.time <= startTime + duration) {
			float targetVelocityX = (pos.x - transform.position.x) * 10f;
			velocity.x = Mathf.Lerp(targetVelocityX, 0, Time.deltaTime * 5f);
			controller.Move (velocity * Time.deltaTime, jumpingDown);

			yield return null;
		}

		controller.Move (velocity * Time.deltaTime, jumpingDown);
	}


	protected void Bleed (int maxBloodSplats) {
		if (!bloodPrefab) { return; }

		for (int i = 0; i < maxBloodSplats; i++) {
			Blood blood = ((GameObject)Instantiate(bloodPrefab, transform.position, Quaternion.identity)).GetComponent<Blood>();
			blood.Init(World.bloodContainer);
		}
	}


	protected void PlayAudioStep () {
		Audio.play("Audio/sfx/step", 1f, Random.Range(1.25f, 1.75f));
	}


	// ===========================================================
	// Hud
	// ===========================================================

	protected virtual IEnumerator UpdateHpBar () {
		if (!hpBar) { yield break; }

		hpBar.gameObject.SetActive(atr.hp < hpMax);

		float percent = atr.hp / (float)hpMax;
		hpPercent.localScale = new Vector2(percent, 1);
		hpPercent.localPosition = new Vector2((-0.5f + percent / 2) * transform.localScale.x, 0);
	}


	public virtual IEnumerator UpdateInfo (string str) {
		if (!info) { yield break; }

		info.gameObject.SetActive(str != null);
		if (str == null) { yield break; }
		info.text = str;
	}


	// ===========================================================
	// Controller2D Triggers
	// ===========================================================

	public virtual void TriggerLanding () {
		PlayAudioStep();
		hasAttackedInAir = false;
	}


	public virtual bool TriggerCollisionAttack (GameObject obj) {
		Ent target = obj.GetComponent<Ent>();

		if (isCreature) {
			// decide if alive being is gonna hit
			if (velocity.y > -2) { return false; }
			if (transform.position.y < target.transform.position.y + transform.localScale.y * 0.75f) { 
				return false;
			}
		} else {
			// decide if item/block is gonna hit
			if (target.state == States.HURT) { return false; }
			if (velocity.magnitude < 4f) { return false; }
			if (velocity.y > 0) { return false; }
		}

		// execute jump attack
		StartCoroutine(JumpAttack(target));
		return true;
	}


	public virtual void TriggerPushable (GameObject obj) {
		PushItem(obj);
	}


	// ===========================================================
	// Collision Triggers and Interactive Objects
	// ===========================================================

	protected void OnTriggerStay2D (Collider2D collider) {
		if (state == States.ATTACK) { return; }

		switch (collider.gameObject.tag) {
			case "Water":
			if (!IsOnWater()) { StartCoroutine(SetWaterIn()); }
			break;

			case "Ladder":
			ladder = collider.transform.parent.GetComponent<Ladder>();
			break;

			case "Item":
				interactiveObject = collider.gameObject.GetComponent<Ent>();
				if (interactiveObject is Coin) {
					PickCoin((Coin)interactiveObject);
					interactiveObject = null;
				}
				break;
		}
	}


	protected void OnTriggerExit2D (Collider2D collider) {
		switch (collider.gameObject.tag) {
			case "Water":
			if (IsOnWater()) { StartCoroutine(SetWaterOut()); }
			break;

			case "Ladder":
			ladder = null;
			break;

			case "Item":
			interactiveObject = null;
			break;
		}
	}
			
}
