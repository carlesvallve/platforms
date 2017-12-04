using UnityEngine;
using System.Collections;


public class Coin : Loot {


	public override IEnumerator Pickup (Humanoid collector) {
		if (spawning) { yield break; }
		yield return StartCoroutine(base.Pickup(collector));

		AudioManager.Play("Audio/sfx/coin", 0.2f, Random.Range(3f, 5.0f));
	}


}
