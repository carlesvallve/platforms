using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Carles.Engine2D {
  public class PulleyInput : MonoBehaviour {
    private InputAction leftMouseClick;

    public GameObject pulleySelected;

    // private void Awake() {
    //   leftMouseClick = new InputAction(binding: "<Mouse>/leftButton");
    //   leftMouseClick.performed += ctx => LeftMouseClicked();
    //   leftMouseClick.Enable();
    // }

    private void LeftMouseClicked() {
      Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
      print("MousePos " + mousePos);

      // Select a rope crank by clicking on it

      RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);
      Debug.Log(mousePos + " " + GameObject.Find("Rope2").transform.position + " " + hit.collider);
      if (hit.collider != null && hit.transform.gameObject.tag == "Crank") {
        if (pulleySelected != hit.transform.gameObject) {

          // Hit something that isnt what is selected already
          if (pulleySelected != null) {
            // Deselecting current crank
            pulleySelected.GetComponent<Crank>().Deselect();
          }

          // Setting pulley to what was just hit
          pulleySelected = hit.transform.gameObject;
          pulleySelected.GetComponent<Crank>().Select();

        } else if (pulleySelected == hit.transform.gameObject) {
          // Hit a pulley and is the one we already hit so deselecting it
          pulleySelected.GetComponent<Crank>().Deselect();
          pulleySelected = null;
        }
      } else {
        if (pulleySelected != null) {
          // Clicked somehwere that had no hit so deselecting the pulley
          pulleySelected.GetComponent<Crank>().Deselect();
          pulleySelected = null;
        }
      }
    }

    public void OnInputRopeLeft(InputAction.CallbackContext context) {
      if (context.performed) {
        Debug.Log("Left " + pulleySelected);
        pulleySelected.GetComponent<Crank>().Rotate(-1);
      }

    }

    public void OnInputRopeRight(InputAction.CallbackContext context) {
      if (context.performed) {
        Debug.Log("Right " + pulleySelected);
        pulleySelected.GetComponent<Crank>().Rotate(1);
      }
    }

    // public void RotateLeft() {
    //   pulleySelected.GetComponent<Crank>().Rotate(-1);
    // }

    // public void RotateRight() {
    //   pulleySelected.GetComponent<Crank>().Rotate(1);
    // }

  }
}
