using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {

	public void Init (Vector3 pos, float dir) {
		transform.SetParent(World.fxContainer);
		float sc = Random.Range(0.5f, 1f);
		transform.localScale = new Vector3(sc, sc, 1);
		transform.position = pos;
		StartCoroutine(Spawn(dir));
	}

	
	private IEnumerator Spawn (float dir) {
		float startTime = Time.time;
		while (Time.time <= startTime + 0.2f) {
			transform.Rotate(0, 0, 5 * dir);
			yield return null;
		}

		Destroy(gameObject);
	}
}
