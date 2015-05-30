using UnityEngine;
using System.Collections;

public class Monster : Ent {


	public override void Awake () {
		base.Awake();

		inv.coins = Random.Range(1, 3);
	}


	public override IEnumerator Die () {
		StartCoroutine(base.Die());
		StartCoroutine(SpawnLoot(inv.coins));
		yield break;
	}

}