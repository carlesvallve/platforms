using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Carles.Engine2D {

  public class PlayerControls : MonoBehaviour {

    CharController2D c;
    // private Collision coll;

    void Start() {
      c = GetComponent<CharController2D>();
      // coll = GetComponent<Collision>();
    }

    // ------------------------------------------------------------------------------
    // Input (These are called from PlayerInput component events)
    // Debug.Log("OnInput " + context.phase);

    public void OnInputMove(InputAction.CallbackContext context) {
      c.move.curMoveInput = context.ReadValue<Vector2>();
    }

    public void OnInputJump(InputAction.CallbackContext context) {
      c.isJumpBeingPressed = context.phase != InputActionPhase.Canceled;

      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {

        if (c.coll.onWall && !c.coll.onGround) {
          c.jump.SetWallJump();
        } else {
          c.jump.SetJump(Vector2.up, false);
        }
      }
    }

    public void OnInputDash(InputAction.CallbackContext context) {
      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {
        c.Dash();
      }
    }

    public void OnInputAttack(InputAction.CallbackContext context) {
      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {
        c.Attack();
      }
    }

    public void OnInputBlock(InputAction.CallbackContext context) {
      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {
        c.Block();
      } else if (context.phase == InputActionPhase.Canceled) {
        c.Unblock();
      }
    }

    public void OnInputGrab(InputAction.CallbackContext context) {
      c.isGrabBeingPressed = context.phase != InputActionPhase.Canceled;
    }
  }
}
