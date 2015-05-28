using UnityEngine;
using System.Collections;


public class Coin : Loot {


	public IEnumerator Pickup (Ent collector) {
		gameObject.GetComponent<BoxCollider2D>().enabled = false;
		affectedByGravity = false;

		float duration = 0.5f;
		Vector3 pos = transform.position;;
		
		float startTime = Time.time;
		while (Time.time <= startTime + duration) {
			pos = collector.transform.position + Vector3.up * collector.transform.localScale.y * 0.5f;
			transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 5f);
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 5f);
			yield return null;
		}

		Destroy(gameObject);
	}
}
