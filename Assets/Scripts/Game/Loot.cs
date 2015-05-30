using UnityEngine;
using System.Collections;

public class Loot : Ent {

	protected bool spawning = false;

	public void Init (Transform container, Ent source) {
		transform.SetParent(container);
		transform.position = source.transform.position + Vector3.up * source.sprite.localScale.y * 0.5f;
		spawning = true;
		affectedByGravity = false;

		Vector2 vec = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(6f, 12f));
		StartCoroutine (Spawn(source, vec));
	}


	private IEnumerator Spawn (Ent source, Vector2 vec) {

		sprite.gameObject.SetActive(false);
		yield return new WaitForSeconds(Random.Range(0f, 0.5f));
		sprite.gameObject.SetActive(true);

		float duration = Random.Range(0.25f, 0.5f);

		if (source) {
			transform.position = source.transform.position + Vector3.up * source.sprite.localScale.y * 0.5f;
		}
		
		affectedByGravity = true;
		velocity.y = vec.y;
		Vector2 pos = new Vector2(transform.position.x + vec.x, transform.position.y);

		float startTime = Time.time;
		while (Time.time <= startTime + duration) {
			float targetVelocityX = (pos.x - transform.position.x) * 10f;
			velocity.x = Mathf.Lerp(targetVelocityX, 0, Time.deltaTime * 5f);
			ApplyGravity();
			controller.Move (velocity * Time.deltaTime, jumpingDown);

			if (Time.time >= startTime + duration / 2) {
				spawning = false;
			}

			yield return null;
		}

		spawning = false;
	}
}
