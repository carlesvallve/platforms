using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Loot : Ent {

	public string path;
	public int value = 1;
	protected bool spawning = false;


	public void Init (Transform container, Ent source, InvItem item) {
		this.name = item.ent.gameObject.name;

		transform.SetParent(container);
		transform.position = source.transform.position + Vector3.up * source.GetHeight() * 0.5f;
		transform.localScale = new Vector3(1, 1, 1);
		
		spawning = true;
		affectedByGravity = false;

		Vector2 vec = new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(6f, 12f));
		StartCoroutine (Spawn(source, vec, item));
	}


	private IEnumerator Spawn (Ent source, Vector2 vec, InvItem item) {

		sprite.gameObject.SetActive(false);
		yield return new WaitForSeconds(Random.Range(0f, 0.5f));
		
		sprite.gameObject.SetActive(true);
		gameObject.GetComponent<BoxCollider2D>().enabled = true;

		float duration = Random.Range(0.25f, 0.5f);

		if (source) {
			transform.position = source.transform.position + Vector3.up * source.GetHeight() * 0.5f;
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

			yield return null;
		}

		spawning = false;

		// remove item from inventory
		if (item != null) { 
			item.num -= 1; 
			if (item.num == 0) {
				source.inv.items.Remove(item);
			}
		}
	}


	public override IEnumerator Pickup (Humanoid collector) {
		if (spawning || collector == null) { yield break; }

		gameObject.GetComponent<BoxCollider2D>().enabled = false;
		affectedByGravity = false;

		Vector3 pos = transform.position + Vector3.up * collector.GetHeight() * 0.5f;
		
		while (Vector2.Distance(transform.position, pos) > 0.2f) {
			if (spawning || collector == null) { 
				gameObject.GetComponent<BoxCollider2D>().enabled = true;
				affectedByGravity = true;
				yield break; 
			}

			pos = collector.transform.position + Vector3.up * collector.GetHeight() * 0.5f;
			transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 15f);
			yield return null;
		}

		// add loot to collector's inventory
		collector.AddLootToInventory(this);
	}
}
