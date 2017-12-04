using UnityEngine;
using System.Collections;

public class InputManagerOriginal : MonoBehaviour {


	public bool right;
	public bool left;
	public bool up;
	public bool down;
	public bool A;
	public bool B;
	public bool C;
	public bool D;
	public bool select;
	public bool start;
	public bool hd_A; //button is being held down
	public bool hd_B; //button is being held down
	public bool hd_C; //button is being held down
	public bool hd_D; //button is being held down

  public bool up_A; // button has been released
	public bool up_C; // button has been released

  private float elapsedTimeDuration = 0.125f;
  public float time_A;

  private float lastFrameTime = -1;


	void Start () {
		if (Input.GetJoystickNames().Length == 0) {
			print("No Joysticks connected.");
		} else {
			print("Joystick connected: " + Input.GetJoystickNames()[0]);
		}
	}


	void Update () {
		Refresh();
	}


	public void Refresh () {
		if (lastFrameTime == Time.time)
			return;
		lastFrameTime = Time.time;

		if (Input.GetButtonDown("Select")) {
			print ("You Pressed select");
			select = true;
		} else if  (Input.GetButtonUp("Select")) {
		 	select = false;
		}

		if (Input.GetButtonDown("Start")) {
			print ("You Pressed start");
			start = true;
		} else if (Input.GetButtonUp("Start")) {
			start = false;
		}

    up_A = false;
		if (Input.GetButtonDown("A") && !hd_A) {
      A = true;
      time_A = Time.time;
    }
		else if (Input.GetButton("A")){
			A = false;
			hd_A = true;
		} else if (Input.GetButtonUp("A")) {
			A = false;
			hd_A = false;

      float elapsedTime = (Time.time - time_A);
      if (elapsedTime < elapsedTimeDuration) {
        up_A = true;
      }
		}

		if (Input.GetButtonDown("B") && !hd_B) B = true;
		else if (Input.GetButton("B")) {
			B = false;
			hd_B = true;
		} else if (Input.GetButtonUp("B")) {
			B = false;
			hd_B = false;
		}

		/*if (Input.GetButtonDown("C") && !hd_C) C = true;
		else if (Input.GetButton("C")) {
			C = false;
			hd_C = true;
		} else if (Input.GetButtonUp("C")) {
			C = false;
			hd_C = false;
		}*/


		up_C = false;
		if (Input.GetAxis("Vertical") > 0.01f || Input.GetKeyDown("up")) {
			C = false;
			hd_C = true;
			up_C = false;
		} else if ((Input.GetAxis("Vertical") == 0 || Input.GetKeyUp ("up")) && hd_C) {
			C = false;
			hd_C = false;
			up_C = true;
		}



		if (Input.GetButtonDown("D") && !hd_D) D = true;
		else if (Input.GetButton("D")) {
			D = false;
			hd_D = true;
		} else if (Input.GetButtonUp("D")) {
			D = false;
			hd_D = false;
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

    updateMouse();
	}

  private void updateMouse() {
    // if (Input.GetButtonDown(0)) {
    //
    // }
  }
}
