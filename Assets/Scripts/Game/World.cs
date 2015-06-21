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
	- Display coins
	- display collected weapons
	- Change selected weapon

- Pick Weapon:
	- Add to inventory and autoselect

- Select Weapon:
	- Change stance to selected weapon type
	- Instantiate weapon prefab on arm tag transform

- Attack:
	- use adecuate attack animation depending on current arm stance -> OK!

- Run
	- Implement running mode, by pressing left/right twice

- Animated tags
	- When playing an animation, search for all existing animated tags, and play the same animation on them -> OK!

- Inv refactoring
	- Inv fields are initially prefabs
	- When player picks something, parent it to him and disable it instead of destroying it
	- When rendering on inventory, get sprite and stats from gameobject in player
	- When spawning, reparent and activate gameobjects player is holding

*/




