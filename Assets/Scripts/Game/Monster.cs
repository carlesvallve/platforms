using UnityEngine;
using System.Collections;

public class Monster : Ent {


	public override void Awake () {
		base.Awake();
	}


	public override IEnumerator Die () {
		StartCoroutine(base.Die());
		SpawnLoot();
		yield break;
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