using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class VirtualJoystickButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

  public string buttonType = "A";
  public bool pressed = false;
  private JoystickManager joystickManager;


  void Start() {
    joystickManager = GameObject.Find("JoystickManager").GetComponent<JoystickManager>();
  }


  public void OnPointerDown(PointerEventData ped) {
    pressed = true;
    // tell the joystick manager to set the direction event
    joystickManager.SetButtonDown(buttonType, true);
    joystickManager.SetButtonUp(buttonType, false);
    //Debug.Log("ButtonDown")
  }


  public void OnPointerUp(PointerEventData ped) {
    pressed = false;
    // tell the joystick manager to set the direction event
    joystickManager.SetButtonDown(buttonType, false);
    joystickManager.SetButtonUp(buttonType, true);
  }

}
