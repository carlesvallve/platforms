using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class Hud : MonoBehaviour {

	Text coins;

	void Awake () {
		coins = transform.Find("Coins/Text").GetComponent<Text>();
	}
	
	public void UpdateCoins (int num) {
		coins.text = num.ToString();
	}
}
