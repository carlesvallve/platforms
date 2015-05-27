using UnityEngine;
using System.Collections;

public class Monster : Ent {

	public GameObject lootPrefab;


	public override void Awake () {
		base.Awake();
	}


	public override IEnumerator Die () {
		StartCoroutine(base.Die());
		SpawnLoot(Random.Range(1, 3));
		yield break;
	}


	protected void SpawnLoot (int maxLoot) {
		if (!lootPrefab) { return; }

		Transform container = GameObject.Find("Loot").transform;
		for (int i = 0; i < maxLoot; i++) {
			Loot loot = ((GameObject)Instantiate(lootPrefab, transform.position, Quaternion.identity)).GetComponent<Loot>();
			loot.Init(container);
		}
	}
}