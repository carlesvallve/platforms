using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour {

	Player player;
	Text coins;

	public GameObject weaponPrefab;
	private List<Transform> weapons = new List<Transform>();

	void Awake () {
		player = GameObject.Find("Player").GetComponent<Player>();
		coins = transform.Find("Coins/Coin/Text").GetComponent<Text>();
		
		for (int i = 0; i < 7; i ++) {
			Transform weaponBox = transform.Find("Weapons/WeaponBox " + i);
			weapons.Add(weaponBox);
		}

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
	}
}
