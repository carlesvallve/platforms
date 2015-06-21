using UnityEngine;
using System.Collections;

public enum WeaponTypes {
	ONEHAND_MELEE = 0,
	ONEHAND_RANGED = 1,
	TWOHAND_MELEE = 2,
	TWOHAND_RANGED = 3,
	DRAG_MELEE = 4,
	DRAG_RANGED = 5,
	THROW = 6,
	SHIELD = 7
}

public class Weapon : Loot {

	public WeaponTypes type;

	public override IEnumerator Pickup (Humanoid collector) {
		if (spawning) { yield break; }
		yield return StartCoroutine(base.Pickup(collector));

		Audio.play("Audio/sfx/coin", 0.2f, Random.Range(3f, 5.0f));
	}
}
