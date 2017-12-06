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
	protected float moveInterval;

	private bool stopMoveCycle = false;


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
		moveInterval = aware ? ai.moveSpeed : ai.moveSpeed * 10;

		// we dont want to check for turn arounds if we are doing anything else than standing or walking
		if (state != States.IDLE) {
			return;
		}

		// wait or turn around if we arrived to the end of the platform
		if (!CheckForFloor(transform.position + Vector3.right * input.x * 0.3f)) {
			input.x = aware ? 0 : Random.Range(0, (int)(-input.x * 2));
			//ResetMoveCycle();
		}

		// turn around if we collided with something not iteractable on the way
		if (CheckForWall(transform.position)) {
			input.x = -input.x;
			//ResetMoveCycle();
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
      SetSpriteDirection(dir);
		} 

		aware = value;
		moveInterval = aware ? ai.moveSpeed : ai.moveSpeed * 10;
		ResetMoveCycle();

		StartCoroutine(UpdateInfo(value ? "!" : "?"));
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(UpdateInfo(null));
	}


	// ===========================================================
	// Ai Movement
	// ===========================================================

	private void ResetMoveCycle () {
		stopMoveCycle = true;
		StartCoroutine(AiMove());
	}


	private IEnumerator AiMove () {
		yield return new WaitForSeconds(moveInterval);
		if (!player) { yield break; }

		if (stopMoveCycle) { 
			stopMoveCycle = false;
			yield break; 
		}

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
        SetSpriteDirection(dir);
				
				yield return StartCoroutine(Attack());
			}
		}

		StartCoroutine(AiAttack());
	}


	// ===========================================================
	// Ai RayCasting
	// ===========================================================

	private bool CheckForFloor (Vector3 pos) {
		float h = GetHeight() / 2;
		Vector2 rayOrigin = pos + Vector3.up * h;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, h + 0.1f, controller.collisionMask);

		if (hit) {
			//Debug.DrawRay(rayOrigin, -Vector2.up * h, Color.yellow);
			return true;
		}

		//Debug.DrawRay(rayOrigin, -Vector2.up * h, Color.magenta);
		return false;
	}


	private bool CheckForWall (Vector3 pos) {
		float w = GetHeight() / 2;
		Vector2 rayOrigin = pos + Vector3.up * w;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * input.x, w + 0.1f, controller.collisionMask);

		if (hit && hit.transform.gameObject.tag != "Player") {
			Debug.DrawRay(rayOrigin, Vector2.right * input.x * (w + 0.1f), Color.black);
			return true;
		}

		Debug.DrawRay(rayOrigin, Vector2.right * input.x * (w + 0.1f), Color.magenta);
		return false;
	}

}