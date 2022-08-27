using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Carles.Engine2D {

  public class PlayerControls : MonoBehaviour {

    CharController2D c;

    void Start() {
      c = GetComponent<CharController2D>();
    }

    // ------------------------------------------------------------------------------
    // Input (These are called from PlayerInput component events)
    // Debug.Log("OnInput " + context.phase);

    public void OnInputMove(InputAction.CallbackContext context) {
      c.move.curMoveInput = context.ReadValue<Vector2>();
    }

    public void OnInputJump(InputAction.CallbackContext context) {
      c.jump.isJumpBeingPressed = context.phase != InputActionPhase.Canceled;

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
        c.dash.SetDash();
      }
    }

    public void OnInputAttack(InputAction.CallbackContext context) {
      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {
        c.combat.Attack();
      }
    }

    public void OnInputBlock(InputAction.CallbackContext context) {
      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {
        c.combat.Block();
      } else if (context.phase == InputActionPhase.Canceled) {
        c.combat.Unblock();
      }
    }

    public void OnInputGrab(InputAction.CallbackContext context) {
      c.move.isGrabBeingPressed = context.phase != InputActionPhase.Canceled;
    }
  }
}
