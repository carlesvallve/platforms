using UnityEngine;
using System.Collections;

public class Monster : Ent {


	public override void Awake () {
		base.Awake();
	}


	public override IEnumerator Die () {
		StartCoroutine(base.Die());
		StartCoroutine(SpawnLoot(Random.Range(1, 3)));
		yield break;
	}

}