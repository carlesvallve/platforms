using UnityEngine;
using System.Collections;


public class World : MonoBehaviour {

	public static Transform itemContainer;
	public static Transform monsterContainer;
	public static Transform lootContainer;
	public static Transform fxContainer;


	void Awake () {
		// set world containers
		itemContainer = GameObject.Find("Items").transform;
		monsterContainer = GameObject.Find("Monsters").transform;
		lootContainer = GameObject.Find("Loot").transform;
		fxContainer = GameObject.Find("Fx").transform;
	}
}


/* 
- OneWayPlatforms
	- Fix colliders (Hawken)

- Tilemap:
	- Fix pink renders (Hawken)

- Title: 
	- Export title scene propperly (Hawken)

- Ent info text:
	- Render them with pixel pirate bitmap text

- Hud:
	- Display coins
	- display collected weapons
	- Change selected weapon

- Pick Weapon:
	- Add to inventory and autoselect

- Select Weapon:
	- Change stance to selected weapon type
	- Instantiate weapon prefab on arm tag transform

- Attack:
	- use adecuate attack animation depending on current arm stance

- Run
	- Implement running mode, by pressing left/right twice

*/