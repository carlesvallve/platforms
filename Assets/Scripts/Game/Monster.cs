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


	public override void Awake () {
		player = GameObject.Find("Player").GetComponent<Player>();
		base.Awake();

		StartCoroutine(StartThinking());
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
				StartCoroutine(SetAware(true));
			} else if (playerDist > atr.vision && aware) {
				StartCoroutine(SetAware(false));
			}


			if (playerDist < atr.vision) { //2) {
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

		if (aware) {
			float dir = Mathf.Sign(player.transform.position.x - transform.position.x);
			sprite.localScale = new Vector2(dir * Mathf.Abs(sprite.localScale.x), sprite.localScale.y);
		}

		StartCoroutine(UpdateInfo(value ? "!" : "?"));
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(UpdateInfo(null));
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