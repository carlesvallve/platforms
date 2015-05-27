using UnityEngine;
using System.Collections;

public enum States {
	IDLE = 0,
	ATTACK = 2,
	HURT = 3,
	//DIE = 4
}


public class Stats {
	public int hp = 1;
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

	protected Ent interactiveObject = null;
	protected Ent pickedUpObject = null;




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
		CheckCollisionTarget();

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
		PickItem(interactiveObject);
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
		}

		if (velocity.y < -18f) {
			jumpingDown = false;
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
		if (state == States.ATTACK || state == States.HURT) { return; };

		// set velocity x
		float targetVelocityX = input.x * speed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);

		// set velocity y (apply gravity)
		ApplyGravity();

		if (velocity.x != 0) { 
			transform.localScale = new Vector2(Mathf.Sign(velocity.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y); 
		}	

		// set 2d controller move
		controller.Move (velocity * Time.deltaTime, jumpingDown);
	}


	protected void SetJump (bool isJumpingDown, float intensity = 1) {
		if (jumping) { return; }

		velocity.y = jumpVelocity * intensity; 

		jumping = true;
		if (isJumpingDown) {
			jumpingDown = true; 
			velocity.y *= 0.5f;
		}
	}


	// ===========================================================
	// Item interaction
	// ===========================================================

	protected void OnTriggerStay2D (Collider2D collider) {
		interactiveObject = collider.gameObject.GetComponent<Ent>();
	}


	protected void OnTriggerExit2D (Collider2D collider) {
		interactiveObject = null;
	}


	protected void PickItem (Ent ent) {
		if (pickedUpObject) { DropItem(pickedUpObject); }
		if (!ent) { return; }

		Vector2 sc = ent.transform.localScale;
		ent.affectedByGravity = false;
		ent.input = Vector2.zero;
		ent.transform.SetParent(transform);
		ent.transform.localPosition = new Vector2(0, 1f);

		ent.transform.localScale = new Vector2(sc.x / (Mathf.Abs(transform.localScale.x)), sc.y / (transform.localScale.y));

		pickedUpObject = ent;
		interactiveObject = null;
	}


	protected void DropItem (Ent ent) {
		if (!ent) { return; }

		ent.affectedByGravity = true;
		ent.transform.SetParent(transform.parent);

		pickedUpObject = null;
		interactiveObject = ent;
	}


	protected void ThrowItem (Ent ent) {
		if (!ent) { return; }

		float dir  = Mathf.Sign(transform.localScale.x);
		ent.input = new Vector2(dir * 1.5f, 0);
		ent.velocity.y = 10f; 

		DropItem(ent);
	}


	// ===========================================================
	// Combat
	// ===========================================================

	protected IEnumerator Attack () {
		state = States.ATTACK;

		// attack parameters
		float weaponRange = 0.8f;
		float knockback = 1.5f;
		float directionX = Mathf.Sign(transform.localScale.x);

		Vector2 d = directionX * Vector2.right * 0.25f;
		StartCoroutine(PushBackwards(d, 0.1f));
		yield return new WaitForSeconds(0.05f);

		// project a ray forward
		Vector2 rayOrigin = new Vector2 (transform.position.x, transform.position.y + transform.localScale.y / 2);
		//RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.right * directionX, weaponRange, attackCollisionMask);
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, weaponRange, controller.attackCollisionMask);
		Debug.DrawRay(rayOrigin, Vector2.right * directionX * weaponRange, Color.yellow);

		//foreach (RaycastHit2D hit in hits) {
		if (hit) {
			Ent target = hit.transform.GetComponent<Ent>();
			StartCoroutine(target.Hurt(directionX * Vector2.right * knockback + Vector2.up * 5));
		}

		yield return StartCoroutine(PushBackwards(-d , 0.1f));

		input = Vector2.zero;
		velocity = Vector2.zero;
		state = States.IDLE;
	}


	public virtual IEnumerator Die () {
		// instantiate blood splats
		if (bloodPrefab) {
			int max = Random.Range(8, 16);
			for (int i = 0; i < max; i++) {
				Blood blood = ((GameObject)Instantiate(bloodPrefab, transform.position, Quaternion.identity)).GetComponent<Blood>();
				blood.Init();
			}
		}
		
		// destroy entity
		yield return null;
		Destroy(gameObject);
	}


	public virtual IEnumerator Hurt (Vector2 vec) {
		state = States.HURT;
		input = Vector2.zero;
		velocity = Vector2.zero;

		// update stats
		stats.hp -= Random.Range(1, 4);
		print (stats.hp);
		if (stats.hp <= 0) { 
			stats.hp = 0; 
			// if no hp left, die instead
			yield return StartCoroutine(Die());
			yield break;
		}

		// push backwards
		yield return StartCoroutine(PushBackwards(vec, 0.5f));

		state = States.IDLE;
	}


	public virtual IEnumerator PushBackwards (Vector2 vec, float duration) {

		velocity.y = vec.y;
		Vector2 pos = new Vector2(transform.position.x + vec.x, transform.position.y);
		
		float startTime = Time.time;
		while (Time.time <= startTime + duration) {
			float targetVelocityX = (pos.x - transform.position.x) * 10f;
			velocity.x = Mathf.Lerp(targetVelocityX, 0, Time.deltaTime * 5f);
			ApplyGravity();
			controller.Move (velocity * Time.deltaTime, jumpingDown);

			yield return null;
		}
	}


	protected virtual void CheckCollisionTarget () {
		if (controller.collisions.below) { return; }

		// check if we jumped over a monster, if so, rebound in him and kill it
		if (controller.collisions.target && velocity.y < 0){
			Ent target = controller.collisions.target.GetComponent<Ent>();

			jumping = false;
			SetJump(false, 1f);

			float knockback = 1f;
			Vector2 d = (target.transform.position - transform.position).normalized * knockback;
			StartCoroutine(target.Hurt(d));
		}
	}
			
}
