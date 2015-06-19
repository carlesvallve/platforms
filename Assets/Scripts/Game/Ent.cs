using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum States {
	IDLE = 0,
	ATTACK = 2,
	PARRY = 3,
	ROLL = 4,
	HURT = 5
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
	public int armor = 0;
	public float regeneration = 0f;
	public float speed = 5f;
	public float jump = 1f;
	public int[] dmg = { 1, 3 };
	public float vision = 5f;
	public float mass = 1f;
}


[System.Serializable]
public class InvItem {
	public string path;
	public int num;
	public int value;
	[HideInInspector]
	public Sprite sprite;
}


[System.Serializable]
public class Inv {
	public List<InvItem> items = new List<InvItem>();
}

[System.Serializable]
public class Prefabs {
	public GameObject bloodPrefab;
	public GameObject damagePrefab;
	public GameObject parryPrefab;
}

	
[RequireComponent (typeof (Controller2D))]
public class Ent : MonoBehaviour {

	public Controller2D controller;

	public States state;
	public Stats stats;
	public Atr atr;
	public Inv inv;
	public Prefabs prefabs;

	public bool affectedByGravity = true;
	public bool pickable = false;
	public bool pushable = false;
	public bool destructable = false;
	public float destructableJumpMass = 0;

	protected Transform sprite;
	
	protected Vector2 input;
	protected Vector2 velocity;
	protected float speed = 1.0f;
	protected float gravity;
	protected float jumpVelocity;
	protected float jumpHeight = 2.5f;
	protected float timeToJumpApex = 0.3125f;
	//public float accelerationTimeAirborne = 0;
	//public float accelerationTimeGrounded = 0;
	//protected float velocityXSmoothing;
	//protected float velocityYSmoothing;

	protected bool running = false;
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
	protected int hpMax;

	protected TextMesh info;

	protected Anim anim;


	// ===========================================================
	// Init
	// ===========================================================

	public virtual void Awake () {
		controller = GetComponent<Controller2D>();
		anim = GetComponent<Anim>();
		sprite = transform.Find("Sprite");

		hpBar = transform.Find("Bar");
		hpPercent = transform.Find("Bar/Percent");
		hpMax = atr.hp;
		StartCoroutine(UpdateHpBar());

		Transform obj = transform.Find("Info");
		if (obj) {
			info = obj.GetComponent<TextMesh>();
			StartCoroutine(UpdateInfo(null));
		}

		gravity = -((2 * jumpHeight) / Mathf.Pow (timeToJumpApex, 2)) * atr.mass; // -80
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex; // 20

		state = States.IDLE;
		stats = new Stats();

		StartCoroutine(Regenerate());
	}


	public virtual void Update () {
		Reset();
		SetInput();
		SetSpeed();
		SetMove();
		OutOfBounds();
	}


	public float GetHeight () {
		return sprite.localScale.y;
	}


	public Sprite GetSpriteImage () {
		return sprite.GetComponent<SpriteRenderer>().sprite;
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


	protected IEnumerator Regenerate () {
		if (atr.regeneration == 0) { yield break; }

		yield return new WaitForSeconds(atr.regeneration);

		if (state != States.HURT) {
			atr.hp += 1;
			if (atr.hp > hpMax) { atr.hp = hpMax; }
			StartCoroutine(UpdateHpBar());
		}
		
		StartCoroutine(Regenerate());
	}


	protected virtual void SetInput () {
		// apply friction
		input.x *= 0.99f;
		if (controller.collisions.left || controller.collisions.right) { input.x = 0; }
		if (controller.collisions.above || controller.collisions.below) { input.x *= 0.9f; }
	}


	protected virtual void SetSpeed () {
		speed = running ? atr.speed * 3 : atr.speed;
	}


	protected void SetMove () {
		//if (state == States.ATTACK || state == States.HURT) { return; };

		// set velocity x
		float targetVelocityX = input.x * speed;
		velocity.x = targetVelocityX;
		//velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
			
		if (velocity.x != 0) {
			if (state != States.ATTACK && state != States.HURT && state != States.PARRY) {
				if (this is Humanoid) {
					sprite.localScale = new Vector2(Mathf.Sign(velocity.x) * Mathf.Abs(sprite.localScale.x), sprite.localScale.y); 
				}
			}
		}

		// set velocity y
		if (IsOnLadder()) {
			hasAttackedInAir = false;
			SetMoveOnLadder();

			if (anim) { anim.Play(velocity.y == 0 ? "ladder90" : "ladder90"); }
		} else {
			ApplyGravity();

			if (anim) { anim.Play(velocity.x == 0 ? "idle90" : "walk90"); }
		}

		// apply controller2d movement
		controller.Move (velocity * Time.deltaTime, jumpingDown);

		// snap to ladders
		if (IsOnLadder()) {
			SnapToLadder(); 
		}
	}


	protected bool CanJump () {
		if (IsOnWater()) { return true; }
		return controller.grounded || previouslyOnLadder;
	}


	protected void SetJump (bool isJumpingDown, float intensity = 1, bool escapeCheck = false) {
		if (!CanJump() && !escapeCheck) { return; }

		if (IsOnWater()) { intensity *= 0.2f; }

		velocity.y = jumpVelocity * intensity;

		hasAttackedInAir = false;
		jumping = true;
		jumpingFromLadder = IsOnLadder();

		if (isJumpingDown) {
			jumpingDown = true;
			velocity.y *= 0.5f;
		}
	}


	public void SetThrow (float dir) {
		input = new Vector2(dir * 1.5f, 0);
		velocity.y = 20f;
	}


	public void SetPush (Vector2 pusherInput, float factor) {
		input.x = pusherInput.x * factor / atr.mass;
		//input.y = pusherInput.y * factor / atr.mass;
	}


	// ===========================================================
	// Water
	// ===========================================================


	public bool IsOnWater () {
		return isWater;
	}


	private IEnumerator SetWaterIn () {
		if (isWater || velocity.y >= 0) { yield break; }

		if (sprite.localScale.x > 0.25f && gameObject.tag != "Fx") {
			Audio.play("Audio/sfx/splash", 0.05f, Random.Range(0.8f, 1f));
		}
		
		isWater = true;
		velocity.x *= 0.1f;
		velocity.y = 0;

		yield break;
	}


	private IEnumerator SetWaterOut () {
		if (!isWater || velocity.y < 0) { yield break; }

		if (sprite.localScale.x > 0.25f && gameObject.tag != "Fx") {
			Audio.play("Audio/sfx/splash", 0.05f, Random.Range(1f, 1.2f));
		}

		isWater = false;

		if (!IsOnLadder() && (this is Humanoid)) {
			velocity.y = 0;
			jumping = false;
			controller.grounded = true;
			SetJump(false, atr.jump * 0.75f);
			controller.grounded = false;
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
		if (controller.grounded) { previouslyOnLadder = false; }

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
		if (!controller.grounded && !jumpingFromLadder) {
			Vector2 pos = new Vector2(ladder.transform.position.x, transform.position.y);
			transform.position = Vector2.Lerp(transform.position, pos, Time.deltaTime * 20f);
		}
	}


	// ===========================================================
	// Items and Loot
	// ===========================================================

	public virtual IEnumerator Pickup (Humanoid collector) {
		//gameObject.GetComponent<BoxCollider2D>().enabled = false;

		input = Vector2.zero;
		velocity = Vector2.zero;
		affectedByGravity = false;

		//Vector2 sc = transform.localScale;
		transform.SetParent(collector.transform);
		//transform.localScale = new Vector2(sc.x / (Mathf.Abs(collector.transform.localScale.x)), sc.y / (collector.transform.localScale.y));

		float duration = 0.2f;
		Vector3 pos = Vector3.up * collector.sprite.localScale.y;
		
		float startTime = Time.time;
		while (Time.time <= startTime + duration) {
			transform.localPosition = Vector3.Lerp(transform.localPosition, pos, Time.deltaTime * 25f);
			yield return null;
		}

		transform.localPosition = pos;
	}


	protected void SpawnLoot () {
		// spawn generic items in inventory
		for (int n = 0; n < inv.items.Count; n++) {
			InvItem item = inv.items[n];

			int max = item.num;

			string folder = item.path.Split('/')[0];
			if (folder == "Treasure") {
				max = (int)(item.num / (item.value == 0 ? 1 : item.value));
			}

			string path = "Prefabs/Loot/" + item.path;
			for (int i = 0; i < max; i++) {
				if (path == "Prefabs/Loot/") { continue; }
				Loot loot = ((GameObject)Instantiate(Resources.Load(path))).GetComponent<Loot>();
				loot.Init(World.lootContainer, this, item.path);
			}
		}
	}


	// ===========================================================
	// Combat
	// ===========================================================

	protected virtual IEnumerator JumpAttack (Ent target) {
		// jump
		jumping = false;
		jumpingDown = false;
		SetJump(false,  atr.jump * 0.5f, true);
		
		// hurt target and knock him back
		float knockback = 1f;
		int dmg = Random.Range(atr.dmg[0], atr.dmg[1]);
		Vector2 d = new Vector2(Mathf.Sign(target.transform.position.x - transform.position.x) * knockback, 3);
		StartCoroutine(target.Hurt(dmg, d));

		yield break;
	}


	public virtual IEnumerator Parry (Humanoid enemy, Vector2 vec) {
		yield break;
	}


	public virtual IEnumerator Hurt (int dmg, Vector2 vec) {
		if (state == States.ATTACK || state == States.PARRY || state == States.HURT) { yield break; }

		state = States.HURT;
		input = Vector2.zero;
		velocity = Vector2.zero;

		Audio.play("Audio/sfx/punch", 0.15f, Random.Range(1f, 1.5f));

		// update stats
		atr.hp -= dmg;
		if (atr.hp <= 0) { atr.hp = 0; }

		// update hp bar
		StartCoroutine(UpdateHpBar());

		// make him bleed
		Bleed(Random.Range(3, 6), dmg);

		// if no hp left, die instead
		if (atr.hp <= 0) {
			yield return StartCoroutine(Die());
			yield break;
		}

		// push backwards
		yield return StartCoroutine(PushBackwards(vec, 0.5f));
		
		state = States.IDLE;
	}


	public virtual IEnumerator Die () {
		Bleed(Random.Range(8, 16));
		
		yield return null;
		Destroy(gameObject);
	}


	protected virtual void Bleed (int maxBloodSplats, int dmg = 0) {
		if (prefabs.bloodPrefab) {
			for (int i = 0; i < maxBloodSplats; i++) {
				Blood blood = ((GameObject)Instantiate(prefabs.bloodPrefab)).GetComponent<Blood>();
				blood.Init(World.fxContainer, this);
			}
		}

		if (prefabs.damagePrefab && dmg > 0) {
			Damage damage = ((GameObject)Instantiate(prefabs.damagePrefab)).GetComponent<Damage>();
			damage.Init(World.fxContainer, this, "+" + dmg);
		}
	}


	public virtual IEnumerator PushBackwards (Vector2 vec, float duration) {
		vec /= atr.mass;
		velocity.y = vec.y;
		Vector2 pos = new Vector2(transform.position.x + vec.x, transform.position.y);
		
		float startTime = Time.time;
		while (Time.time <= startTime + duration) {
			
			float targetVelocityX = (pos.x - transform.position.x) * 10f;
			velocity.x = Mathf.Lerp(targetVelocityX, 0, Time.deltaTime * 5f);

			if (jumping) { yield break; }
			controller.Move (velocity * Time.deltaTime, jumpingDown);

			yield return null;
		}

		if (jumping) { yield break; }
		controller.Move (velocity * Time.deltaTime, jumpingDown);
	}


	protected void PlayAudioStep () {
		Audio.play("Audio/sfx/step", 1f, Random.Range(2.0f, 3.0f));
	}


	// ===========================================================
	// Hud
	// ===========================================================

	protected virtual IEnumerator UpdateHpBar () {
		if (!hpBar) { yield break; }

		hpBar.gameObject.SetActive(atr.hp < hpMax);

		float percent = atr.hp / (float)hpMax;
		hpPercent.localScale = new Vector2(percent, 1);
		hpPercent.localPosition = new Vector2((-0.5f + percent / 2) * hpBar.transform.localScale.x, 0);
	}


	public virtual IEnumerator UpdateInfo (string str, float duration = 0) {
		if (!info) { yield break; }

		info.gameObject.SetActive(str != null);
		if (str == null) { yield break; }
		info.text = str;

		if (duration > 0) {
			yield return new WaitForSeconds(duration);
			info.gameObject.SetActive(false);
		}
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
		if (!target || target.destructableJumpMass == 0 || target.destructableJumpMass > atr.mass) { return false; }

		// decide if item/block is gonna hit (throwing objects)
		if (target.state == States.HURT) { return false; }
		if (velocity.magnitude < 4f) { return false; }

		if (transform.position.y < target.transform.position.y + transform.localScale.y * 0.5f) { 
			return false;
		}
		
		// execute jump attack
		StartCoroutine(JumpAttack(target));
		return true;
	}


	public virtual void TriggerPushable (GameObject obj) {}


	protected virtual void OutOfBounds () {
		// Destroy if is out of bounds
		if (transform.position.y < -1) {
			Destroy(gameObject);
		}
	}


	// ===========================================================
	// Collision Triggers and Interactive Objects
	// ===========================================================

	protected virtual void OnTriggerStay2D (Collider2D collider) {
		//if (state == States.ATTACK) { return; }

		switch (collider.gameObject.tag) {
			case "Water":
			if (!IsOnWater()) { StartCoroutine(SetWaterIn()); }
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


	protected virtual void OnTriggerExit2D (Collider2D collider) {
		switch (collider.gameObject.tag) {
			case "Water":
			if (IsOnWater()) { StartCoroutine(SetWaterOut()); }
			break;
		}
	}
			
}
