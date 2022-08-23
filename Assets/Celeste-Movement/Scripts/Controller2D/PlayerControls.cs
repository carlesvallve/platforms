using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Carles.Engine2D {

  public class PlayerControls : MonoBehaviour {

    Movement controller;
    private Collision coll;

    void Start() {
      controller = GetComponent<Movement>();
      coll = GetComponent<Collision>();
    }

    // ------------------------------------------------------------------------------
    // Input (These are called from PlayerInput component events)
    // Debug.Log("OnInput " + context.phase);

    public void OnInputMove(InputAction.CallbackContext context) {
      controller.curMoveInput = context.ReadValue<Vector2>();
    }

    public void OnInputJump(InputAction.CallbackContext context) {
      controller.isJumpBeingPressed = context.phase != InputActionPhase.Canceled;

      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {

        if (coll.onWall && !coll.onGround) {
          controller.WallJump();
        } else {
          controller.Jump(Vector2.up, false);
        }
      }
    }

    public void OnInputDash(InputAction.CallbackContext context) {
      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {
        controller.Dash();
      }
    }

    public void OnInputGrab(InputAction.CallbackContext context) {
      controller.isGrabBeingPressed = context.phase != InputActionPhase.Canceled;
    }
  }
}
