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
	- Fix colliders

- Ladders:
	- We have to be able to pass through platforms while in a ladder, unless we are on the bottom of it
	- if we are on the top of a ladder, we need to be able to walk normally




*/