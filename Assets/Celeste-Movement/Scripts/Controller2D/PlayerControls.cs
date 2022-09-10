using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Carles.Engine2D {

  public class PlayerControls : MonoBehaviour {

    CharController2D c;

    private InputAction leftMouseClick;

    void Start() {
      c = GetComponent<CharController2D>();
    }

    private bool isInputEnabled() {
      return !c.combat.isDead;
    }

    // ------------------------------------------------------------------------------
    // Mouse

    // public void SetMouse() {
    //   leftMouseClick = new InputAction(binding: "<Mouse>/leftButton");
    //   leftMouseClick.performed += ctx => LeftMouseClicked();
    //   leftMouseClick.Enable();
    // }

    public void OnLeftMouse(InputAction.CallbackContext context) {
      // Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
      // print("LeftMouseClicked" + mousePos);

      Debug.Log("LeftMouse " + context);
    }

    // ------------------------------------------------------------------------------
    // Input (These are called from PlayerInput component events)
    // Debug.Log("OnInput " + context.phase);

    public void OnInputMove(InputAction.CallbackContext context) {
      if (!isInputEnabled()) return;

      c.move.curMoveInput = context.ReadValue<Vector2>();
    }

    public void OnInputJump(InputAction.CallbackContext context) {
      if (!isInputEnabled()) return;

      c.jump.isJumpBeingPressed = context.phase != InputActionPhase.Canceled;

      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {

        if (c.ropeClimb.isActive) {
          // c.rb.velocity = new Vector2(c.rb.velocity.x, 0);
          c.ropeClimb.EndRope();
        }

        if (c.coll.onWall && !c.coll.onGround) {
          c.jump.SetWallJump();
        } else {
          c.jump.SetJump(Vector2.up, false);
        }
      }
    }

    public void OnInputDash(InputAction.CallbackContext context) {
      if (!isInputEnabled()) return;

      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {
        c.dash.SetDash();
      }
    }

    public void OnInputAttack(InputAction.CallbackContext context) {
      if (!isInputEnabled()) return;

      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {
        c.combat.Attack();
      }
    }

    public void OnInputBlock(InputAction.CallbackContext context) {
      if (!isInputEnabled()) return;

      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {
        c.combat.Block();
      } else if (context.phase == InputActionPhase.Canceled) {
        c.combat.Unblock();
      }
    }

    public void OnInputGrab(InputAction.CallbackContext context) {
      if (!isInputEnabled()) return;

      c.move.isGrabBeingPressed = context.phase != InputActionPhase.Canceled;
    }

    public void OnInpuHook(InputAction.CallbackContext context) {
      if (!isInputEnabled()) return;

      if (context.phase == InputActionPhase.Performed) {
        c.hook.StartHook();
      } else if (context.phase == InputActionPhase.Canceled) {
        c.hook.EndHook();
      }
    }

  }
}
