using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour {


  public void Start() {
		DontDestroyOnLoad(Navigator.instance.gameObject);
  }


	public void StartGame() {
    Navigator.instance.Open("Game", true);
  }
}
