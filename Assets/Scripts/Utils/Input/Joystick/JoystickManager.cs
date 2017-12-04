using UnityEngine;
using System.Collections;

public struct JoystickAction {
  public Vector2 direction;
  
  public bool buttonADown;
  public bool buttonBDown;
  public bool buttonCDown;
  
  public bool buttonAUp;
  public bool buttonBUp;
  public bool buttonCUp;
  
  public int buttonAHold;
  public int buttonBHold;
  public int buttonCHold;
}

public class JoystickManager : MonoBehaviour {
  
  public bool useNormalizedDirection = true; // if normalized, direction magnitude will be always a factor of 1f

  public System.Action<JoystickAction> onDirection;
  public System.Action<JoystickAction> onButtonADown;
  public System.Action<JoystickAction> onButtonBDown;
  public System.Action<JoystickAction> onButtonCDown;
  public System.Action<JoystickAction> onButtonAUp;
  public System.Action<JoystickAction> onButtonBUp;
  public System.Action<JoystickAction> onButtonCUp;
  public System.Action<JoystickAction> onButtonAHold;
  public System.Action<JoystickAction> onButtonBHold;
  public System.Action<JoystickAction> onButtonCHold;
  private JoystickAction currentJoystickAction = new JoystickAction();

  private float lastFrameTime = -1; // avoid innecessary updates
  private bool isVirtual = false;   // so we dont reset direction every frame while also using the virtual jostick
  
  
	void Start () {
		if (Input.GetJoystickNames().Length == 0) {
			print("No Joysticks connected.");
		} else {
			print("Joystick connected: " + Input.GetJoystickNames()[0]);
		}
	}


	void Update () {
    if (lastFrameTime == Time.time) { return; }
		lastFrameTime = Time.time;

    UpdateDirection();
    UpdateButtons();
	}

  public void SetDirection(Vector2 direction, bool _isVirtual = false) {
    isVirtual = _isVirtual;

    // set direction data
    currentJoystickAction.direction = direction;
    
    // if normalized, magnitude will be always a factor of 1f
    if (useNormalizedDirection) {
      currentJoystickAction.direction = direction.normalized;
    }

    // Fire event
    if (onDirection != null) {
      onDirection(currentJoystickAction);
    }
  }

  public void SetButtonDown(string buttonId, bool value) {
    switch(buttonId) {
    case "A":
      currentJoystickAction.buttonADown = value;
      if (value && onButtonADown != null) { onButtonADown(currentJoystickAction); }
      break;
      case "B":
        currentJoystickAction.buttonBDown = value;
        if (value && onButtonBDown != null) { onButtonBDown(currentJoystickAction); }
        break;
      case "C":
        currentJoystickAction.buttonCDown = value;
        if (value && onButtonCDown != null) { onButtonCDown(currentJoystickAction); }
        break;
    }
  }

  public void SetButtonUp(string buttonId, bool value) {
    switch(buttonId) {
    case "A":
      currentJoystickAction.buttonAUp = value;
      if (value && onButtonAUp != null) { onButtonAUp(currentJoystickAction); }
      break;
      case "B":
        currentJoystickAction.buttonBUp = value;
        if (value && onButtonBUp != null) { onButtonBUp(currentJoystickAction); }
        break;
      case "C":
        currentJoystickAction.buttonCUp = value;
        if (value && onButtonCUp != null) { onButtonCUp(currentJoystickAction); }
        break;
    }
  }
  
  public void SetButtonHold(string buttonId, int value) {
    switch(buttonId) {
    case "A":
      if (value > 0 && onButtonAHold != null) { 
        currentJoystickAction.buttonAHold += value;
        onButtonAHold(currentJoystickAction); 
      } else {
        currentJoystickAction.buttonAHold = 0;
      }
      break;
      case "B":
        
        if (value > 0 && onButtonBHold != null) { 
          currentJoystickAction.buttonBHold += value;
          onButtonBHold(currentJoystickAction); 
        } else {
          currentJoystickAction.buttonBHold = 0;
        }
        break;
      case "C":
        
        if (value > 0 && onButtonCHold != null) { 
          currentJoystickAction.buttonCHold += value;
          onButtonCHold(currentJoystickAction); 
        } else {
          currentJoystickAction.buttonCHold = 0;
        }
        break;
    }
  }


  void UpdateDirection() { 
    if (currentJoystickAction.direction.magnitude > 0 && isVirtual) {
      return;
    }

    // direction by joystick
    Vector2 direction = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    direction = (direction.magnitude > 1) ? direction.normalized : direction;

    // direction by keys (vertical)
    if (Input.GetKey ("down")) {
      direction.y = -1f;
		} else if (Input.GetKey ("up")) {
			direction.y = 1f;
		} else {
			direction.y = 0;
		}

    // direction by keys (horizontal)
		if (Input.GetKey ("right")){
			direction.x = 1f;
		} else if (Input.GetKey ("left")){
			direction.x = -1f;
		} else {
			direction.x = 0f;
		}

    SetDirection(direction);
  }


  void UpdateButtons() {
    // ------------ DOWN ----------------
    
    // A Down
    if (Input.GetButtonDown("A") || Input.GetKeyDown ("z")) {
      SetButtonDown("A", true);
      SetButtonUp("A", false);
    }

    // B Down
    if (Input.GetButtonDown("B") || Input.GetKeyDown ("x")) {
      SetButtonDown("B", true);
      SetButtonUp("B", false);
    }

    // C Down
    if (Input.GetButtonDown("C") || Input.GetKeyDown ("c")) {
      SetButtonDown("C", true);
      SetButtonUp("C", false);
    }
    
    // ------------ UP ----------------
    
    // A Up
    if (Input.GetButtonUp("A") || Input.GetKeyUp ("z")) {
      SetButtonDown("A", false);
      SetButtonHold("A", 0);
      SetButtonUp("A", true);
    }

    // B Up
    if (Input.GetButtonUp("B") || Input.GetKeyUp ("x")) {
      SetButtonDown("B", false);
      SetButtonHold("B", 0);
      SetButtonUp("B", true);
    }

    // C Up
    if (Input.GetButtonUp("C") || Input.GetKeyUp ("c")) {
      SetButtonDown("C", false);
      SetButtonHold("C", 0);
      SetButtonUp("C", true);
    }
    
    // ------------ HOLD ----------------
    
    // A Hold
    if (Input.GetButton("A") || Input.GetKeyDown ("z")) {
      SetButtonHold("A", 1);
      SetButtonUp("A", false);
    }

    // B Down
    if (Input.GetButton("B") || Input.GetKey("x")) {
      SetButtonHold("B", 1);
      SetButtonUp("B", false);
    }

    // C Down
    if (Input.GetButton("C") || Input.GetKey ("c")) {
      SetButtonHold("C", 1);
      SetButtonUp("C", false);
    }

  }

}
