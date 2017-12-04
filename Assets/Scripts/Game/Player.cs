using UnityEngine;
using System.Collections;
using Kuchen;

public class Player : Humanoid {

	public override void Awake () {
		InitJoystickManager();
		base.Awake();
	}


	protected override void SetInput () {

	}
	
	
	public override IEnumerator Die() {
    yield return StartCoroutine(base.Die());
    // publish gameover
		this.Publish("GameOver");
	}
	
	
	// ========================================================
	// Joystick User Input
	// ========================================================

  void InitJoystickManager () {
    // NOTE: We could do it using Kichen events, but prefer a dependency-free implementation since is already done
    JoystickManager joystickManager = GameObject.Find("JoystickManager").GetComponent<JoystickManager>();
    joystickManager.onDirection += SetDirection;
    joystickManager.onButtonADown += SetButtonADown;
    joystickManager.onButtonBDown += SetButtonBDown;
    joystickManager.onButtonCDown += SetButtonCDown;
    joystickManager.onButtonAUp += SetButtonAUp;
    joystickManager.onButtonBUp += SetButtonBUp;
    joystickManager.onButtonCUp += SetButtonCUp;
		joystickManager.onButtonAHold += SetButtonAHold;
    joystickManager.onButtonBHold += SetButtonBHold;
    joystickManager.onButtonCHold += SetButtonCHold;
	}

  void SetDirection(JoystickAction joystickAction) {
    //Debug.Log(joystickAction.direction);
    input = joystickAction.direction;
  }

  // Buttons Down

  void SetButtonADown(JoystickAction joystickAction) {
    //Debug.Log("A");
    // JUMP
		SetJump(joystickAction.direction.y > 0, joystickAction.direction.y < 0 ? 1.25f : 1f);
  }

  void SetButtonBDown(JoystickAction joystickAction) {
    //Debug.Log("B");
    // ATTACK
		SetAttack(joystickAction.direction.y > 0); 
  }

  void SetButtonCDown(JoystickAction joystickAction) {
    //Debug.Log("C");
    // BLOCK / ROLL
    // if (joystickAction.direction.y > 0.5f) {
		// 	StartCoroutine(Roll());
		// } else {
		// 	StartCoroutine(Block());
		// }
  }


  // Buttons Up

  void SetButtonAUp(JoystickAction joystickAction) {
    //Debug.Log("A UP");
  }

  void SetButtonBUp(JoystickAction joystickAction) {
    //Debug.Log("B UP");
  }

  void SetButtonCUp(JoystickAction joystickAction) {
    //Debug.Log("C UP");
		SetAction();
  }
	
	
	// Buttons Hold

  void SetButtonAHold(JoystickAction joystickAction) {
    //Debug.Log("A Hold");
  }

  void SetButtonBHold(JoystickAction joystickAction) {
    //Debug.Log("B Hold");
  }

  void SetButtonCHold(JoystickAction joystickAction) {
    //Debug.Log("C Hold" + joystickAction.buttonCHold);
		// number of frames we are holding the button
		if (joystickAction.buttonCHold == 10) { 
			SetActionHold();
		}
  }
	
	
}
