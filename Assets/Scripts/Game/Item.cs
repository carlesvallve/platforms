using UnityEngine;
using System.Collections;

public class Item : Ent {

	public bool opening = false;
	private bool opened = false;


	public override void Awake () {
		base.Awake();
	}


	public IEnumerator Pickup (Ent collector) {
		//gameObject.GetComponent<BoxCollider2D>().enabled = false;

		input = Vector2.zero;
		velocity = Vector2.zero;
		affectedByGravity = false;

		Vector2 sc = transform.localScale;
		transform.SetParent(collector.transform);
		transform.localScale = new Vector2(sc.x / (Mathf.Abs(collector.transform.localScale.x)), sc.y / (collector.transform.localScale.y));

		float duration = 0.2f;
		Vector3 pos = Vector3.up * 1f;
		
		float startTime = Time.time;
		while (Time.time <= startTime + duration) {
			transform.localPosition = Vector3.Lerp(transform.localPosition, pos, Time.deltaTime * 25f);
			yield return null;
		}

		transform.localPosition = pos;
	}

	
	public IEnumerator Opening (Ent collector) {
		if (opened) { yield break; }

		opening = true;

		float t = 5;

		float duration = 0.5f;
		float startTime = Time.time;
		while (Time.time <= startTime + duration) {
			if (opening == false) { yield break; }

			StartCoroutine(UpdateInfo(t.ToString()));
			t--;

			yield return new WaitForSeconds(0.1f);
		}

		StartCoroutine(UpdateInfo(null));
		Open(collector);
	}


	public void CancelOpening () {
		opening = false;
	}


	protected void Open (Ent collector) {
		opened = true;
		
		StartCoroutine(SpawnLoot(Random.Range(8, 16)));
		Destroy(gameObject);
	}

}
