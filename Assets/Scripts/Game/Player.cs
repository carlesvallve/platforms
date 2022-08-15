using UnityEngine;
using System.Collections;
using Carles;

public class Player : Humanoid {

  [HideInInspector]
  public Hud hud;

  private PlayerInput playerInput;
  private int hd_C = 0;

  public override void Awake() {
    playerInput = transform.GetComponent<PlayerInput>();
    hud = GameObject.Find("Hud").GetComponent<Hud>();
    base.Awake();
  }

  protected override void SetInput() {
    bool isDown = playerInput.Vertical.GetAxis() < -0.5;
    bool isUp = playerInput.Vertical.GetAxis() > 0.5;

    // movement
    input = new Vector2(playerInput.Horizontal.GetAxis(), playerInput.Vertical.GetAxis());

    // jump
    if (playerInput.Jump.GetButtonDown()) {
      Debug.Log("A - Jump " + playerInput.Vertical.GetAxis());
      SetJump(isDown, isUp ? 1.25f : 1f);
    }

    // attack
    if (playerInput.Attack.GetButtonDown()) {
      SetAttack(isDown);
    }

    // timed action
    if (playerInput.Action.GetButtonUp()) {
      hd_C = 0;
      SetAction();
    }

    if (playerInput.Action.GetButton()) {
      hd_C += 1;
      if (hd_C == 10) {
        SetActionHold();
      }
    }

    // change weapon
    if (Input.GetKeyDown(KeyCode.LeftShift)) {
      hud.changeWeapon();
    }
  }


  // protected override void SetInput () {
  // 	input = Vector2.zero;
  // 	if (inputManager.left) { input.x = -1f; }
  // 	if (inputManager.right) { input.x = 1f; }
  // 	if (inputManager.up) { input.y = 1f; }
  // 	if (inputManager.down) { input.y = -1f; }

  // 	if (inputManager.A) {
  // 		SetJump(inputManager.down, inputManager.up ? 1.25f : 1f);
  // 	}

  // 	if (inputManager.B) { 
  // 		SetAttack(inputManager.down); 
  // 	}

  // 	if (Input.GetButtonUp("C")) { 
  // 		hd_C = 0;
  // 		SetAction();
  // 	}

  // 	if (Input.GetButton("C")) { 
  // 		hd_C += 1;
  // 		if (hd_C == 10) {
  // 			SetActionHold();
  // 		}
  // 	}

  // 	if (Input.GetKeyDown(KeyCode.LeftShift)) {
  // 		hud.changeWeapon();
  // 	}
  // }
}
