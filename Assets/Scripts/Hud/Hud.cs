using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Hud : MonoBehaviour {

	private Player player;
	private Text coinText;
	private List<Image> weaponSlots = new List<Image>();

	void Awake () {
		player = GameObject.Find("Player").GetComponent<Player>();
		coinText = transform.Find("Bar/Bg/CoinText").GetComponent<Text>();
		coinText.text = "0";

		for (int i = 0; i < 4; i++) {
			weaponSlots.Add(transform.Find("Bar/Bg/Weapons/Weapon" + i + "/Icon").GetComponent<Image>());
			weaponSlots[i].sprite = null;	
		}

		ResetWeaponSlots();
	}


	public void UpdateInventory () {
		// reset weapon slots
		ResetWeaponSlots();
		int weaponNum = 0;

		for (int n = 0; n < player.inv.items.Count; n++) {
			InvItem item = player.inv.items[n];

			// if item is coin, display how many we have
			if (item.ent is Coin) {
				DisplayCoin(item.num);
			}

			// if item is weapon, display it on weapon slots
			if (item.ent is Weapon) {
				DisplayWeaponSlot(weaponNum, item.ent.GetSpriteImage());
				weaponNum += 1;
			}
		}
	}


	public void DisplayCoin (int value) {
		coinText.text = value.ToString();
	}


	private void ResetWeaponSlots () {
		for (int i = 0; i < 4; i++) {
			weaponSlots[i].sprite = null;
			weaponSlots[i].gameObject.SetActive(false);

		}
	}


	public void DisplayWeaponSlot (int num, Sprite sprite) {
		weaponSlots[num].sprite = sprite;
		weaponSlots[num].gameObject.SetActive(true);
	}
	


	/*public void changeWeapon () {
		Audio.play("Audio/sfx/SFX-b_cancel2", 0.3f, Random.Range(3f, 3f));
	}*/
}
