using UnityEngine;
using System.Collections;
using Kuchen;


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
		
		// Game Over Event Listener
    this.Subscribe("GameOver", () => {
      Debug.Log("GAME OVER");
      Navigator.instance.Open("Title", true);
    });
	}
	
}
