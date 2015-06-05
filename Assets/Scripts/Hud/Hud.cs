using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour {

	Player player;
	Text coins;

	//public GameObject weaponPrefab;
	private List<Transform> weapons = new List<Transform>();
	private int weaponMax = 0;
	private int weaponNum = 0;

	private Transform selector;


	void Awake () {
		player = GameObject.Find("Player").GetComponent<Player>();
		
		coins = transform.Find("Coins/Coin/Text").GetComponent<Text>();
		for (int i = 0; i < 7; i ++) {
			Transform weaponBox = transform.Find("Weapons/WeaponBox " + i);
			weapons.Add(weaponBox);
		}

		selector = transform.Find("Weapons/Selector");
		selector.gameObject.SetActive(false);

		UpdateInventory();
	}
	

	public void UpdateInventory () {
		// hide all weapons
		int i;
		for (i = 0; i < weapons.Count; i ++) {
			weapons[i].gameObject.SetActive(false);
		}

		// iterate on player's inv
		i = 0;
		for (int n = 0; n < player.inv.items.Count; n++) {
			InvItem item = player.inv.items[n];

			if (item.path == "Treasure/Coin") {
				// display coins
				coins.text = item.num.ToString();
			} else {
				// display weapons
				weapons[i].Find("Image").GetComponent<Image>().sprite = item.sprite;
				weapons[i].Find("Text").GetComponent<Text>().text = item.num.ToString();
				weapons[i].gameObject.SetActive(true);

				i++;
			}
		}

		weaponMax = i - 1;
		selector.gameObject.SetActive(weaponMax > 0);
	}


	public void changeWeapon () {
		if (weaponMax == 0) { return; }

		weaponNum += 1;
		if (weaponNum > weaponMax) { 
			weaponNum = 0;
		}

		RectTransform rect = selector.GetComponent<RectTransform>();
		rect.anchoredPosition = new Vector2(5 + weaponNum * 50, rect.anchoredPosition.y);

		Audio.play("Audio/sfx/SFX-b_cancel2", 0.3f, Random.Range(3f, 3f));
	}
}
