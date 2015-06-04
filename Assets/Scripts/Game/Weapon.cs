using UnityEngine;
using System.Collections;

public class Weapon : Loot {

	public override IEnumerator Pickup (Ent collector) {
		if (spawning) { yield break; }
		yield return StartCoroutine(base.Pickup(collector));

		Audio.play("Audio/sfx/coin", 0.2f, Random.Range(3f, 5.0f)); // chimes
		
		
		/*if (collector is Player) {
			Player player = (Player)collector;
			player.hud.UpdateCoins(player.inv.coins);
		}*/
		


		//yield return null;
		Destroy(gameObject);
	}
}
