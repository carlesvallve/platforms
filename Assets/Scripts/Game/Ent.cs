using UnityEngine;
using System.Collections;

public enum States {
	IDLE = 0,
	ATTACK = 2,
	HURT = 3,
}


public class Stats {
	public int coins = 0;
	public int hp = 8;
}


[RequireComponent (typeof (Controller2D))]
public class Ent : MonoBehaviour {

	public Controller2D controller;

	public States state;
	public Stats stats;

	public float jumpHeight = 2.5f;
	public float timeToJumpApex = 0.25f;
	public float accelerationTimeAirborne = 0;
	public float accelerationTimeGrounded = 0;
	public float moveSpeed = 5f;
	public float runSpeed = 5f;
	public bool affectedByGravity = true;

	public GameObject lootPrefab;
	public GameObject bloodPrefab;
	
	protected Vector2 input;
	protected float speed = 1.0f;
	protected float gravity;
	protected float jumpVelocity;
	protected Vector2 velocity;
	protected float velocityXSmoothing;
	protected float velocityYSmoothing;

	protected bool jumping = false;
	protected bool jumpingDown = false;
	protected bool jumpingFromLadder = false;

	protected Ladder ladder;
	protected Ent interactiveObject;
	protected Ent pickedUpObject;


	// ===========================================================
	// Init
	// ===========================================================

	public virtual void Awake () {
		controller = GetComponent<Controller2D>();

		gravity = -((2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2));
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

		state = States.IDLE;
		stats = new Stats();
	}


	public virtual void Update () {
		Reset();
		SetInput();
		SetSpeed();
		SetMove();
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


	// ===========================================================
	// Movement
	// ===========================================================

	protected void ApplyGravity () {
		if (affectedByGravity) {
			velocity.y += gravity * Time.deltaTime;
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
		speed = moveSpeed;
	}


	protected void SetMove () {
		//if (state == States.ATTACK || state == States.HURT) { return; };

		// set velocity x
		float targetVelocityX = input.x * speed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
		
		if (velocity.x != 0) { 
			if (state != States.ATTACK && state != States.HURT) {
				transform.localScale = new Vector2(Mathf.Sign(velocity.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y); 
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
		if (jumping) { return; }

		velocity.y = jumpVelocity * intensity; 

		jumping = true;
		jumpingFromLadder = IsOnLadder();

		if (isJumpingDown) {
			jumpingDown = true;
			velocity.y *= 0.5f;
		}
	}


	// ===========================================================
	// Triggers and Interactive Objects
	// ===========================================================

	protected void OnTriggerStay2D (Collider2D collider) {
		if (state == States.ATTACK) { return; }

		switch (collider.gameObject.tag) {
			case "Ladder":
				ladder = collider.transform.parent.GetComponent<Ladder>();
				return;
		}

		interactiveObject = collider.gameObject.GetComponent<Ent>();

		if (interactiveObject is Coin) {
			PickCoin((Coin)interactiveObject);
			interactiveObject = null;
		}
	}


	protected void OnTriggerExit2D (Collider2D collider) {
		switch (collider.gameObject.tag) {
			case "Ladder":
				ladder = null;
				return;
		}

		interactiveObject = null;
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

	protected IEnumerator PickItem (Ent ent) {
		if (pickedUpObject) { 
			StartCoroutine(DropItem(pickedUpObject)); 
		}

		if (!ent || !(ent is Item)) { yield break; } //  || !ent.controller.landed
		
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

		float dir  = Mathf.Sign(transform.localScale.x);
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

	protected void SpawnLoot (int maxLoot) {
		if (!lootPrefab) { return; }

		for (int i = 0; i < maxLoot; i++) {
			Loot loot = ((GameObject)Instantiate(lootPrefab, transform.position, Quaternion.identity)).GetComponent<Loot>();
			loot.Init(World.lootContainer);
		}
	}


	protected virtual void PickCoin (Coin coin) {
		Audio.play("Audio/sfx/chimes", 0.5f, Random.Range(1.0f, 1.0f));
		StartCoroutine(coin.Pickup(this));
		stats.coins += 1;
	}


	// ===========================================================
	// Combat
	// ===========================================================

	protected IEnumerator JumpAttack (GameObject obj) {
		Ent target = obj.GetComponent<Ent>();

		if (velocity.y < 0 && transform.position.y > target.transform.position.y + transform.localScale.y * 0.75f) { 
			jumping = false;
			jumpingDown = false;
			SetJump(false, 1f);

			float knockback = 1f;
			Vector2 d = new Vector2(Mathf.Sign(target.transform.position.x - transform.position.x) * knockback, 0);
			StartCoroutine(target.Hurt(d));

			PlayAudioStep();
		}

		
		yield break;
	}


	protected IEnumerator Attack () {
		if (state == States.ATTACK) { yield break; }
		if (IsOnLadder() && !controller.landed) { yield break; }

		state = States.ATTACK;
		Audio.play("Audio/sfx/swishA", 0.025f, Random.Range(0.5f, 0.5f));
		
		// attack parameters
		float weaponRange = 0.8f;
		float knockback = 1.5f;
		float directionX = Mathf.Sign(transform.localScale.x);

		// push attacker forward
		Vector2 d = directionX * Vector2.right * 0.5f + Vector2.up * 5;
		StartCoroutine(PushBackwards(d, 0.1f));
		yield return new WaitForSeconds(0.05f);

		// project a ray forward
		Vector2 rayOrigin = new Vector2 (transform.position.x, transform.position.y + transform.localScale.y / 2);
		//RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.right * directionX, weaponRange, attackCollisionMask);
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, weaponRange, controller.attackCollisionMask);
		Debug.DrawRay(rayOrigin, Vector2.right * directionX * weaponRange, Color.yellow);

		//foreach (RaycastHit2D hit in hits) {
		if (hit) {
			// push target forward
			Ent target = hit.transform.GetComponent<Ent>();
			StartCoroutine(target.Hurt(directionX * Vector2.right * knockback)); // + Vector2.up * 5));

			// push attacker backwards
			yield return StartCoroutine(PushBackwards(-d , 0.1f));
		}

		input = Vector2.zero;
		velocity = Vector2.zero;

		yield return new WaitForSeconds(0.1f);
		state = States.IDLE;
	}


	public virtual IEnumerator Hurt (Vector2 vec) {
		state = States.HURT;
		input = Vector2.zero;
		velocity = Vector2.zero;

		Audio.play("Audio/sfx/step", 1f, Random.Range(2.5f, 2.5f));

		// update stats
		stats.hp -= Random.Range(1, 4);
		if (stats.hp <= 0) {
			stats.hp = 0; 
			// if no hp left, die instead
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
		Audio.play("Audio/sfx/bite", 0.3f, Random.Range(3f, 3f));

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
	// Controller2D Triggers
	// ===========================================================

	public virtual void TriggerLanding () {
		PlayAudioStep();
	}


	public virtual void TriggerCollisionAttack (GameObject obj) {
		StartCoroutine(JumpAttack(obj));
	}


	public virtual void TriggerPushable (GameObject obj) {
		PushItem(obj);
	}
			
}
