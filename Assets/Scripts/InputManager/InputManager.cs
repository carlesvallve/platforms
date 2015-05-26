using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {


	public bool right;
	public bool left;
	public bool up;
	public bool down;
	public bool A;
	public bool B;
	public bool C;
	public bool select;
	public bool start;
	public bool hd_A; //button is being held down
	public bool hd_B; //button is being held down
	public bool hd_C; //button is being held down

	// Use this for initialization
	void Start () {
		if (Input.GetJoystickNames().Length == 0) {
			print("No Joysticks connected.");
		} else {
			print("Joystick connected: " + Input.GetJoystickNames()[0]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Select")) select = true;
		else if (Input.GetButtonUp("Select")) select = false;

		if (Input.GetButtonDown("Start")) start = true;
		else if (Input.GetButtonUp("Start")) start = false;

		if (Input.GetButtonDown("A") && !hd_A) A = true;
		else if (Input.GetButton("A")){
			A = false;
			hd_A = true;
		} else if (Input.GetButtonUp("A")) {
			A = false;
			hd_A = false;
		}

		if (Input.GetButtonDown("B") && !hd_B) B = true;
		else if (Input.GetButton("B")) {
			B = false;
			hd_B = true;
		} else if (Input.GetButtonUp("B")) {
			B = false;
			hd_B = false;
		}
		
		if (Input.GetButtonDown("C") && !hd_C) C = true;
		else if (Input.GetButton("C")) {
			C = false;
			hd_C = true;
		} else if (Input.GetButtonUp("C")) {
			C = false;
			hd_C = false;
		}

		if (Input.GetAxis("Vertical") < -0.01f || Input.GetKey ("down")) {
			down = true;
			up = false;
		} else if (Input.GetAxis("Vertical") > 0.01f || Input.GetKey ("up")) {
			down = false;
			up = true;
		} else {
			down = false;
			up = false;
		}

		if (Input.GetAxis("Horizontal") < -0.01f || Input.GetKey ("left")){
			left = true;
			right = false;
		} else if (Input.GetAxis("Horizontal") > 0.01f || Input.GetKey ("right")){
			left = false;
			right = true;
		} else {
			left = false;
			right = false;
		}


		// quick fix for resetting the game
		if (start && select) {
			Application.LoadLevel("Dadako");
		}
	}
}
