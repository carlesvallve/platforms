using UnityEngine;
using System.Collections;

public class Chest : Ent {

	public bool opening = false;
	public bool opened = false;
	//public GameObject bloodPrefab;
	//public GameObject damagePrefab;


	public override void Awake () {
		base.Awake();
	}

	
	public IEnumerator Opening (Ent collector) {
		if (opened) { yield break; }

		opening = true;

		float t = 5;
		float duration = 1f;
		float startTime = Time.time;

		while (Time.time <= startTime + duration) {
			if (opening == false) { yield break; }
			StartCoroutine(UpdateInfo(t.ToString()));
			t--;
			yield return new WaitForSeconds(0.2f);
		}
		
		Open(collector);

		StartCoroutine(UpdateInfo("OPEN"));
		yield return new WaitForSeconds(0.2f);
		StartCoroutine(UpdateInfo(null));

		Destroy(gameObject);
	}


	public void CancelOpening () {
		opening = false;
		StartCoroutine(UpdateInfo(null));
	}


	protected void Open (Ent collector) {
		opened = true;
		Audio.play("Audio/sfx/chest-open", 0.4f, Random.Range(0.9f, 1.1f));
		SpawnLoot();
	}


	public override IEnumerator Die () {
		Audio.play("Audio/sfx/bite", 0.5f, Random.Range(3f, 3f));
		SpawnLoot();

		// instantiate blood splats
		Bleed(Random.Range(32, 32));
		
		// destroy entity
		yield return null;
		Destroy(gameObject);
	}


	protected override void Bleed (int maxBloodSplats) {
		if (!prefabs.bloodPrefab) { return; }

		for (int i = 0; i < maxBloodSplats; i++) {
			Blood blood = ((GameObject)Instantiate(prefabs.bloodPrefab)).GetComponent<Blood>();
			blood.Init(World.bloodContainer, this);
		}
	}

}
