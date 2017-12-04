using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

  public Vector2 InputDirection;
  private JoystickManager joystickManager;
  private Image joystickContainer;
  private Image joystick;


  void Start() {
    joystickManager = GameObject.Find("JoystickManager").GetComponent<JoystickManager>();
    joystickContainer = GetComponent<Image>();
    joystick = transform.GetChild(0).GetComponent<Image>(); //this command is used because there is only one child in hierarchy
  }


  public void OnDrag(PointerEventData ped) {
    Vector2 position = Vector2.zero;

    // To get InputDirection
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
      joystickContainer.rectTransform,
      ped.position,
      ped.pressEventCamera,
      out position
    );

    position.x = (position.x/joystickContainer.rectTransform.sizeDelta.x);
    position.y = (position.y/joystickContainer.rectTransform.sizeDelta.y);

    float x = (joystickContainer.rectTransform.pivot.x == 1f) ? position.x * 2 + 1 : position.x * 2 - 1;
    float y = (joystickContainer.rectTransform.pivot.y == 1f) ? position.y * 2 + 1 : position.y * 2 - 1;

    InputDirection = new Vector2 (x,y);
    InputDirection = (InputDirection.magnitude > 1) ? InputDirection.normalized : InputDirection;

    // To define the area in which joystick can move around
    joystick.rectTransform.anchoredPosition = new Vector2 (
      InputDirection.x * (joystickContainer.rectTransform.sizeDelta.x / 4), // 3
      InputDirection.y * (joystickContainer.rectTransform.sizeDelta.y) / 4  // 3
    );

    // tell the joystick manager to set the direction event
    joystickManager.SetDirection(InputDirection, true);
  }

  public void OnPointerDown(PointerEventData ped) {
    OnDrag(ped);
  }

  public void OnPointerUp(PointerEventData ped) {
    InputDirection = Vector3.zero;
    joystick.rectTransform.anchoredPosition = Vector3.zero;

    // tell the joystick manager to set the direction event
    joystickManager.SetDirection(InputDirection);
  }
}
