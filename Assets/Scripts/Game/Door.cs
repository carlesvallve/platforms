using UnityEngine;
using System.Collections;

public class Door : Ent {

	public bool opening = false;
	private bool opened = false;


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

}
