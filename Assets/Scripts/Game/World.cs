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

HAWKEN

- OneWayPlatforms
	- Fix colliders (Hawken)

- Tilemap:
	- Fix pink renders (Hawken)

- Title: 
	- Export title scene propperly (Hawken)

- Pete:
	- Finish Pete animations and layers (hurt, hair, hat, etc...) (Hawken)

- Ent info text:
	- Render them with pixel pirate bitmap text


CARLES

- Hud:
	- Display coins -> OK
	- display collected weapons -> OK

- Select Weapon:
	- Change stance to selected weapon type
	- Instantiate weapon prefab on arm tag transform
	- Select weapon slot on hud

- Attack -> OK!
	- use adecuate attack animation depending on current arm stance -> OK!

- Animated tags -> OK!
	- When playing an animation, search for all existing animated tags, and play the same animation on them -> OK!

- Inv refactoring -> OK!
	- Inv fields are initially prefabs
	- When player picks something, parent it to him and disable it instead of destroying it
	- When spawning, reparent and activate gameobjects player is holding


- Run
	- Implement running mode, by pressing left/right twice

*/




