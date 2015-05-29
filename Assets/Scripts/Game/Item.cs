using UnityEngine;
using System.Collections;

public class Item : Ent {

	public override void Awake () {
		base.Awake();
	}


	public IEnumerator Pickup (Ent collector) {

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

}
