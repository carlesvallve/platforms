using UnityEngine;
using System.Collections;


[System.Serializable]
public class Ai {
	public float atkSpeed = 1.5f;
}


public class Monster : Ent {

	public Ai ai = new Ai();

	protected Player player;

	protected bool aware = false;
	protected bool willMoveTowards = false;


	public override void Awake () {
		player = GameObject.Find("Player").GetComponent<Player>();
		base.Awake();

		StartCoroutine(StartThinking());
	}


	protected override void SetInput () {
		if (!player) { return; }

		float playerDist = Vector2.Distance(transform.position, player.transform.position);
		if (playerDist < atr.vision && aware) {
			if (willMoveTowards) {
				input.x = Mathf.Sign(player.transform.position.x - transform.position.x);
			} else {
				input.x = 0;
			}
		}
	}


	public override IEnumerator Die () {
		SpawnLoot();
		StartCoroutine(base.Die());
		yield break;
	}


	private bool CanThink () {
		if (!player) { return false; }
		if (state != States.IDLE) { return false; }
		return true;
	}


	// TODO: Refactor this in several methods for each possible though/action

	private IEnumerator StartThinking () {
		yield return new WaitForSeconds(Random.Range(0, ai.atkSpeed));
		StartCoroutine(Think());
	}

	private IEnumerator Think () {

		yield return new WaitForSeconds(ai.atkSpeed + Random.Range(0, ai.atkSpeed));

		if (CanThink()) {
			float playerDist = Vector2.Distance(transform.position, player.transform.position);

			if (playerDist <= atr.vision && !aware) {
				yield return StartCoroutine(SetAware(true));
				
			} else if (playerDist > atr.vision && aware) {
				yield return StartCoroutine(SetAware(false));

			}



			if (playerDist < atr.vision && aware) {
				float r = Random.Range(1, 100);
				willMoveTowards = r <= 50;
			} else {
				float r = Random.Range(1, 25);
				willMoveTowards = r <= 50;
				if (willMoveTowards) {
					input.x = Mathf.Sign(Random.Range(-1, 1));
				} else {
					input.x = 0;
				}
			}

			if (playerDist < 2 && aware) { //atr.vision) { //2) {
				// turn versus player and attack
				float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
				sprite.localScale = new Vector2(dir * Mathf.Abs(sprite.localScale.x), sprite.localScale.y); 
				yield return new WaitForSeconds(0.1f);
				StartCoroutine(Attack());
			}
		}
		
		StartCoroutine(Think());
	}


	private IEnumerator SetAware (bool value) {
		aware = value;

		input.x = 0;
		willMoveTowards = false;

		if (aware) {
			float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
			sprite.localScale = new Vector2(dir * Mathf.Abs(sprite.localScale.x), sprite.localScale.y);
		}

		StartCoroutine(UpdateInfo(value ? "!" : "?"));
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(UpdateInfo(null));

		yield return new WaitForSeconds(ai.atkSpeed);
	}


	// ===========================================================
	// Ai
	// ===========================================================

	/*
	- Vision:
		- Each monster should be able to see any important objects in a semicircle in the direction he is looking at

	- Decision:
		- Each monster will evaluate his own interest depending on interesting objects in view
			- coin, potion, monster, player, 

	- Movement:
		- each monster casts rays where he is going to move next:
			- hit is trap -> he wont move there
			- hit is player -> he will attack or jump over, or escape if he is in bad shape
			- hit is bottom form current -> if distanceY < jumpDist he will fall to it
			- hit is top from current -> if distance < jumpDist he will jump to it
	*/

}