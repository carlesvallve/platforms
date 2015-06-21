using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour {

	public void Init (Transform container, Ent source, string str) {
		transform.SetParent(container);
		transform.position = source.transform.position + Vector3.up * (source.GetHeight() + 0.2f);
		
		TextMesh info = GetComponent<TextMesh>();
		info.text = str;

		StartCoroutine (Spawn(source));
	}


	private IEnumerator Spawn (Ent source) {
		Vector2 pos = new Vector2(transform.position.x, transform.position.y + 1f);

		float startTime = Time.time;
		while (Time.time <= startTime + 0.4f) {
			transform.position = Vector2.Lerp(transform.position, pos, Time.deltaTime * 5f);
			yield return null;
		}

		Destroy(gameObject);
	}
}
