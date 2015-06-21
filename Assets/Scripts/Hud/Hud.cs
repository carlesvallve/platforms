using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour {

	private Player player;
	private int coinsMax = 0;
	

	void Awake () {
		player = GameObject.Find("Player").GetComponent<Player>();
	}
	

	public void UpdateInventory () {
	}


	public void changeWeapon () {
		Audio.play("Audio/sfx/SFX-b_cancel2", 0.3f, Random.Range(3f, 3f));
	}
}
