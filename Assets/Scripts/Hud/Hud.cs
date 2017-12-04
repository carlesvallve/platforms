using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kuchen;

public class Hud : MonoBehaviour {

  private Navigator navigator;

	private Transform header;
  private Text scoreText;

  private Transform options;


	void Start () {
    navigator = Navigator.instance;

    InitHeader();
    InitOptions();

    this.Subscribe("UpdateScore", (int score) => {
      scoreText.text = score.ToString();
    });
	}


  private void InitHeader() {
    header = transform.Find("Header");
    scoreText = header.Find("Score/Value").GetComponent<Text>();
  }


  private void InitOptions() {
    options = transform.Find("Options");
    options.gameObject.SetActive(false);
  }


  public void toggleOptions(bool visible) {
    options.gameObject.SetActive(visible);
  }


  public void ExitGame() {
    navigator.Open("Title", true);
  }

}
