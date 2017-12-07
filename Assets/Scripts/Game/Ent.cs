using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum States {
	IDLE = 0,
	ATTACK = 2,
	PARRY = 3,
	ROLL = 4,
	HURT = 5,
  
  JUMP = 6,
  DOUBLEJUMP = 7,
  
  DEAD = 8,
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
  protected Animator animator;
  
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
  protected SpriteRenderer spriteRenderer;
  protected float spriteDirection = 1f;
	
	protected Vector2 input;
	protected Vector2 velocity;
	protected float speed = 1.0f;
	protected float gravity;
	protected float jumpVelocity;
	protected float jumpHeight = 2.75f;
	protected float timeToJumpApex = 0.225f; // 0.3125f;
	//public float accelerationTimeAirborne = 0;
	//public float accelerationTimeGrounded = 0;
	//protected float velocityXSmoothing;
	//protected float velocityYSmoothing;

	protected bool running = false;
	protected bool jumping = false;
	protected bool jumpingDown = false;
	protected bool jumpingFromLadder = false;
  
  protected int jumpCount = 0;

	protected Ladder ladder;
	protected Ent interactiveObject;
	protected Ent pickedUpObject;

	protected bool hasAttackedInAir = false;

	protected bool isWater = false;

	protected Transform hpBar;
	protected Transform hpPercent;
	protected int hpMax;

	protected TextMesh info;
  
  
  protected float currentSpeed = 0;
	protected float accelerationTime = 0.5f;
	protected float deaccelerationTime = 0.2f;


	// ===========================================================
	// Init
	// ===========================================================

	public virtual void Awake () {
		controller = GetComponent<Controller2D>();

		sprite = transform.Find("Sprite");
    spriteRenderer = sprite.GetComponent<SpriteRenderer>();
    animator = sprite.GetComponent<Animator>();

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
    //Debug.Log(gravity);
		jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex; // 20

		state = States.IDLE;
		stats = new Stats();

		StartCoroutine(Regenerate());
	}


	public virtual void FixedUpdate () {
    SetSpeed();
		SetInput();
		SetMove();
    SetAnimationStates();
		OutOfBounds();
	}
	
	
	public float GetHeight () {
		return sprite.localScale.y;
	}


	public Sprite GetSpriteImage () {
		return spriteRenderer.sprite;
	}
  
  
  // ===========================================================
	// Rpg
	// ===========================================================
  
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


	// ===========================================================
	// Movement
	// ===========================================================

	protected virtual void SetSpeed () {
		speed = running ? atr.speed * 3 : atr.speed;
	}
  
  
  protected virtual void SetInput () {
    // each ai might chose a different way to set their movement input
    
    // 	// apply friction (this is outdated)
  	// 	input.x *= 0.99f;
  	// 	if (controller.collisions.left || controller.collisions.right) { input.x = 0; }
  	// 	if (controller.collisions.above || controller.collisions.below) { input.x *= 0.9f; }
  }


	protected void SetMove () {
		// No Smooth Damp, simple velocity method
		//float targetVelocityX = input.x * speed; // * 0.5f;
		//velocity.x = targetVelocityX;
		  
    // Smooth damped velocity with friction
    float targetVelocityX = input.x * speed * 10f;
		float acceleration = Mathf.Abs(targetVelocityX) >= Mathf.Abs(velocity.x) ? accelerationTime : deaccelerationTime;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref currentSpeed, acceleration);
    velocity.x *= 0.85f; // apply friction

    // set sprite direction
    SetSpriteDirection();

		// set velocity y
		SetVelocityY();
    
		// apply controller2d movement
		controller.Move (velocity * Time.deltaTime, jumpingDown);
	}
  
  
  private void SetVelocityY() {
    if (controller.collisions.left || controller.collisions.right) { velocity.x = 0; }
    
    if (controller.collisions.below && velocity.y < 0) { velocity.y = 0; }
    if (controller.collisions.above && velocity.y > 0) { velocity.y = 0; }
    if (velocity.y < -18f && !jumpingFromLadder) { jumpingDown = false; }
		if (velocity.y < -4f && !jumpingDown) { jumpingFromLadder = false; }
    
    if (IsOnLadder()) {
			hasAttackedInAir = false;
			SetMoveOnLadder();
      SnapToLadder(); 
		} else {
			ApplyGravity();
		}
  }
  
  
  protected void ApplyGravity () {
		if (affectedByGravity) {
			if (IsOnWater()) {
				velocity.y += gravity * 0.1f * Time.deltaTime;
			} else {
				velocity.y += gravity * 1f * Time.deltaTime;
			}
		}
	}
  
  
  // ===========================================================
	// Direction
	// ===========================================================
  
  protected virtual bool CanChangeDirection() {
    if (this is Humanoid && state != States.ATTACK && state != States.HURT && state != States.PARRY) {
        return true;
    }
    
    return false;
  }
  

  protected virtual float SetSpriteDirection(float dir = 0) {
    if (!CanChangeDirection()) {
      return spriteDirection;
    }
    
    // force direction to given one
    if (dir != 0) {
      spriteDirection = dir;
      spriteRenderer.flipX = spriteDirection == 1f ? false : true;
      return spriteDirection;
    }
    
    // get movement vector and escape if we are not moving
    float targetVelocityX = input.x * speed;
    if (targetVelocityX == 0) { 
      return spriteDirection;
    }
    
    // set direction to movement vector
    int newDir = (int)Mathf.Sign(targetVelocityX); //velocity.x);
    if (newDir != spriteDirection) {
      spriteDirection = newDir;
      spriteRenderer.flipX = spriteDirection == 1f ? false : true;
      //return true;
    }
    
    return spriteDirection;
  }
  
  public virtual float GetSpriteDirection() {
    return spriteDirection;
  }
  

  // ===========================================================
	// Jump
	// ===========================================================
  
	protected bool CanJump () {
		if (IsOnWater()) { return true; }
		return controller.grounded || previouslyOnLadder;
	}


	protected void SetJump (bool isJumpingDown, float intensity = 1, bool escapeCheck = false) {
    
    //Debug.Log("JUMP:" + isJumpingDown); // + " " + intensity + " " + escapeCheck);
    
    if (!IsOnLadder() && !controller.grounded && jumpCount == 0) { //} && velocity.y < 0) {
			SetDoubleJump(intensity);
      return;
		}
    
		if (!CanJump() && !escapeCheck) { return; }
    
    if (IsOnWater()) { intensity *= 0.2f; }

		velocity.y = jumpVelocity * intensity;

		hasAttackedInAir = false;
		jumping = true;
		jumpingFromLadder = IsOnLadder();
    
    state = States.JUMP;
    AudioManager.Play("Audio/sfx/jump", 0.8f, Random.Range(1f, 2f));

		if (isJumpingDown) {
			jumpingDown = true;
			velocity.y *= 0.5f;
		}
	}
  
  
  protected virtual void SetDoubleJump(float intensity = 1.25f) {
		jumpCount = 1;

		velocity.y = jumpVelocity * intensity;
		//input.x *= 1.5f;

    AudioManager.Play("Audio/sfx/jump", 0.8f, Random.Range(1f, 2f));
	}
  

  // ===========================================================
	// Push and Throw
	// ===========================================================
  
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
			AudioManager.Play("Audio/sfx/splash", 0.05f, Random.Range(0.8f, 1f));
		}
		
		isWater = true;
		velocity.x *= 0.1f;
		velocity.y = 0;

		yield break;
	}


	private IEnumerator SetWaterOut () {
		if (!isWater || velocity.y < 0) { yield break; }

		if (sprite.localScale.x > 0.25f && gameObject.tag != "Fx") {
			AudioManager.Play("Audio/sfx/splash", 0.05f, Random.Range(1f, 1.2f));
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

		transform.SetParent(collector.transform);
		
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

		AudioManager.Play("Audio/sfx/punch", 0.15f, Random.Range(1f, 1.5f));

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


	protected virtual void Bleed (int dmg = 0, int maxBloodSplats = 20) {
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


	protected virtual void PlayAudioStep (float volume = 1f, float pitchMin = 2f, float pitchMax = 3f) {
    // 0.2f, Random.Range(1.5f, 2f));
		AudioManager.Play("Audio/sfx/step", 1f, Random.Range(pitchMin, pitchMax));
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
    // Debug.Log("TriggerLanding");
		PlayAudioStep();
    
		hasAttackedInAir = false;
    jumping = false;
    jumpingDown = false;
    jumpingFromLadder = false;
    jumpCount = 0;
    
    velocity.y = 0; 
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
	
  // ========================================================
	// Animation
	// ========================================================

	protected void SetAnimationStates () {
		if (state == States.IDLE || state == States.JUMP) {
      
      //Debug.Log(IsOnLadder() + " " + Mathf.Abs(velocity.x) + " " + Mathf.Abs(velocity.y));
      
      // ladder animations
      if (IsOnLadder()) {
        if (Mathf.Abs(velocity.y) <= 1f) {
				  PlayAnimation("idle");
				} else {
					PlayAnimation("walk", 1f);
				}
        return;
      }
      
      // grounded animations
			if (controller.grounded) {
				if (Mathf.Abs(velocity.x) <= 1f) {
				  PlayAnimation("idle");
				} else {
					PlayAnimation("walk", 1f);
				}
			} else {
				PlayAnimation(jumpCount == 1 ? "roll" : "jump");
			}
		}
	}


	protected void PlayAnimation (string name, float speed = 1f) {
    if (!animator) {
      return;
    }
    
    //Debug.Log("PlayAnimation" + name);

		// dont play state if does not exist
		if (!animator.HasState( 0, Animator.StringToHash(name))) {
			return;
		}

		// dont play state if is already playing
		if (animator.GetCurrentAnimatorStateInfo(0).IsName(name)) {
			return;
		}

		animator.Play(name);
		animator.speed = speed;
	}		
}
