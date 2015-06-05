using UnityEngine;
using System.Collections;

/*
- create weapon class
- player can store and equip weapons
- player can attack using equipped weapon
- if weapon type is melee, attack
- if weapon type is ranged, shoot

- create projectile class


- enemies:
	- hermit
	- skeleton
	- skeleton gold
	- bird
	- fish


- Improvements:

	- Moving platforms, including trap blocks
	- Double tap left/right to run

*/

public class World : MonoBehaviour {

	public static Transform itemContainer;
	public static Transform monsterContainer;
	public static Transform lootContainer;
	public static Transform bloodContainer;


	void Awake () {
		// set world containers
		itemContainer = GameObject.Find("Items").transform;
		monsterContainer = GameObject.Find("Monsters").transform;
		lootContainer = GameObject.Find("Loot").transform;
		bloodContainer = GameObject.Find("Blood").transform;
	}
	
}
