using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour {

	Player player;
	Text coins;

	private int coinsMax = 0;

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
		// reset coins
		coinsMax = 0;

		// reset weapons
		int i;
		for (i = 0; i < weapons.Count; i ++) {
			weapons[i].gameObject.SetActive(false);
		}

		// iterate on player's inv
		i = 0;
		for (int n = 0; n < player.inv.items.Count; n++) {
			InvItem item = player.inv.items[n];

			// get folder / item type
			string folder = item.path.Split('/')[0];
			
			if (folder == "Treasure") {
				// update coins
				coinsMax += item.num;
			} else if (folder == "Weapons") {
				// update weapons
				weapons[i].Find("Image").GetComponent<Image>().sprite = item.sprite;
				weapons[i].Find("Text").GetComponent<Text>().text = item.num.ToString();
				weapons[i].gameObject.SetActive(true);

				i++;
			}
		}

		coins.text = coinsMax.ToString();

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
