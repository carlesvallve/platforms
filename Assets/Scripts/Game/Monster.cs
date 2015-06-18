using UnityEngine;
using System.Collections;


[System.Serializable]
public class Ai {
	public float visionSpeed = 0.5f;
	public float moveSpeed = 0.5f;
	public float attackSpeed = 2f;

	public float visionRange = 10f;
}


public class Monster : Humanoid {

	public Ai ai = new Ai();

	protected Player player;
	protected bool aware = false;
	protected bool isThereAnEmptySpace;
	protected float moveInterval;


	public override void Awake () {
		player = GameObject.Find("Player").GetComponent<Player>();
		base.Awake();

		moveInterval = ai.moveSpeed;

		// initialize ai loops
		StartCoroutine(AiVision());
		StartCoroutine(AiMove());
		StartCoroutine(AiAttack());
	}


	protected override void SetInput () {
		// ai move speed is different when we are aware or at ease
		moveInterval = aware && input.x != 0 ? ai.moveSpeed : ai.moveSpeed * 10;

		// turn around if we collided with something not iteractable on the way
		if (controller.collisions.left && input.x < 0) { input.x = 1; }
		if (controller.collisions.right && input.x > 0) { input.x = -1; }

		// wait or turn around if we arrived to the end of the platform
		isThereAnEmptySpace = CheckForEmptySpace(transform.position + Vector3.right * input.x * 0.3f);
		if (isThereAnEmptySpace) {
			input.x = aware ? 0 : Random.Range(0, (int)(-input.x * 2)); // -input.x; //
		}
	}


	// ===========================================================
	// Ai Vision
	// ===========================================================

	private IEnumerator AiVision () {
		yield return new WaitForSeconds(ai.visionSpeed);
		if (!player) { yield break; }

		if (state == States.IDLE) {
			// cast a ray to player to check if we become aware/unaware of him
			Vector2 rayOrigin = new Vector2 (transform.position.x, transform.position.y + GetHeight() / 2);
			Vector2 direction = (new Vector2 (player.transform.position.x, player.transform.position.y + GetHeight() / 2) - rayOrigin).normalized;
			float distance = ai.visionRange;

			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, distance, controller.collisionMask);
			Debug.DrawRay(rayOrigin, direction * distance, Color.cyan);

			yield return StartCoroutine(SetAware(hit && hit.transform.gameObject.tag == "Player"));
		}
		
		StartCoroutine(AiVision());
	}


	private IEnumerator SetAware (bool value) {
		if (aware == value) { yield break; }

		input.x = 0;
		velocity.x = 0;

		if (value) {
			float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
			sprite.localScale = new Vector2(dir * Mathf.Abs(sprite.localScale.x), sprite.localScale.y);
		} else {
			//aware = value;
		}

		StartCoroutine(UpdateInfo(value ? "!" : "?"));
		yield return new WaitForSeconds(0.2f);

		aware = value;
		if (aware) {
			StopCoroutine(AiMove());
			StartCoroutine(AiMove());
		}

		yield return new WaitForSeconds(0.3f);
		StartCoroutine(UpdateInfo(null));

		//aware = value;
	}


	// ===========================================================
	// Ai Movement
	// ===========================================================

	private IEnumerator AiMove () {
		yield return new WaitForSeconds(moveInterval);
		if (!player) { yield break; }

		if (state == States.IDLE) {
			float d = player.transform.position.x - transform.position.x;
			input.x = aware && Mathf.Abs(d) > 1.0f ? Mathf.Sign(d) : input.x = Random.Range(-1, 1);
		}

		StartCoroutine(AiMove());
	}


	// ===========================================================
	// Ai Attack
	// ===========================================================

	private IEnumerator AiAttack () {
		yield return new WaitForSeconds(ai.attackSpeed);
		if (!player) { yield break; }

		if (state == States.IDLE) {
			float playerDist = Vector2.Distance(transform.position, player.transform.position);
			if (playerDist <= 1.5f) {
				// turn versus player and attack
				float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
				sprite.localScale = new Vector2(dir * Mathf.Abs(sprite.localScale.x), sprite.localScale.y); 
				
				yield return StartCoroutine(Attack());
			}
		}

		StartCoroutine(AiAttack());
	}


	// ===========================================================
	// Ai RayCasting
	// ===========================================================

	private bool CheckForEmptySpace (Vector3 pos) {
		Vector2 rayOrigin = pos + Vector3.up * GetHeight() / 2; //(directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 0.6f, controller.collisionMask);

		if (hit) {
			Debug.DrawRay(rayOrigin, -Vector2.up * 0.6f, Color.yellow);
			return false;
		}

		Debug.DrawRay(rayOrigin, -Vector2.up * 0.6f, Color.magenta);
		return true;
	}

}