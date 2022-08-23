using UnityEngine;
using System.Collections;

public class Weapon : Loot {

	public override IEnumerator Pickup (Humanoid collector) {
		if (spawning) { yield break; }
		yield return StartCoroutine(base.Pickup(collector));

		Audio.play("Audio/sfx/coin", 0.2f, Random.Range(3f, 5.0f));
	}
}
