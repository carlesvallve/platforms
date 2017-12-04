using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickControllerExample : MonoBehaviour {

  public JoystickManager joystickManager;

	void Start () {
    if (!joystickManager) {
      Debug.LogWarning("Joystick Manager not defined!");
      return;
    }

    joystickManager.onDirection += SetDirection;
    joystickManager.onButtonADown += SetButtonADown;
    joystickManager.onButtonBDown += SetButtonBDown;
    joystickManager.onButtonCDown += SetButtonCDown;
    joystickManager.onButtonAUp += SetButtonAUp;
    joystickManager.onButtonBUp += SetButtonBUp;
    joystickManager.onButtonCUp += SetButtonCUp;
	}

  void SetDirection(JoystickAction joystickAction) {
    //Debug.Log(joystickAction.direction);
  }


  void SetButtonADown(JoystickAction joystickAction) {
    Debug.Log("A");
  }

  void SetButtonBDown(JoystickAction joystickAction) {
    Debug.Log("B");
  }

  void SetButtonCDown(JoystickAction joystickAction) {
    Debug.Log("C");
  }


  void SetButtonAUp(JoystickAction joystickAction) {
    Debug.Log("A UP");
  }

  void SetButtonBUp(JoystickAction joystickAction) {
    Debug.Log("B UP");
  }

  void SetButtonCUp(JoystickAction joystickAction) {
    Debug.Log("C UP");
  }

}
