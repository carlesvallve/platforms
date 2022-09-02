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
      c.move.curMoveInput = context.ReadValue<Vector2>();
    }

    public void OnInputJump(InputAction.CallbackContext context) {
      c.jump.isJumpBeingPressed = context.phase != InputActionPhase.Canceled;

      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {

        if (c.hook.isHookActive) {
          // c.rb.velocity = new Vector2(c.rb.velocity.x, 0);
          c.hook.EndRope();
        }

        if (c.coll.onWall && !c.coll.onGround) {
          c.jump.SetWallJump();
        } else {
          c.jump.SetJump(Vector2.up, false);
        }

        // if (c.roping.attached) {
        //   c.roping.Detach();
        // }
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

    public void OnInpuHook(InputAction.CallbackContext context) {
      if (context.phase == InputActionPhase.Performed) {
        c.hook.StartHook();
      } else if (context.phase == InputActionPhase.Canceled) {
        c.hook.EndHook();
      }
    }

    public void OnInpuSlideUp(InputAction.CallbackContext context) {
      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {
        // c.roping.Slide(1);
        c.hook.RopeSlide(1);
      }
    }

    public void OnInpuSlideDown(InputAction.CallbackContext context) {
      // first frame that button is pressed down
      if (context.phase == InputActionPhase.Performed) {
        // c.roping.Slide(-1);
        c.hook.RopeSlide(-1);
      }
    }


  }
}
