using UnityEngine;
using System.Collections;

public class Chest : Ent {

	public bool opening = false;
	public bool opened = false;


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
		AudioManager.Play("Audio/sfx/chest-open", 0.4f, Random.Range(0.9f, 1.1f));
		SpawnLoot();
	}


	public override IEnumerator Die () {
		AudioManager.Play("Audio/sfx/bite", 0.5f, Random.Range(3f, 3f));
		Bleed(Random.Range(8, 16));
		SpawnLoot();

		yield return null;
		Destroy(gameObject);
	}
}
