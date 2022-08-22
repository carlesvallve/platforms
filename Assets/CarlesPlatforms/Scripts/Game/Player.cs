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

    GameObject go = GameObject.Find("Hud");
    hud = go ? go.GetComponent<Hud>() : null;
    base.Awake();
  }

  protected override void SetInput() {
    bool isDown = playerInput.Vertical.GetAxis() < -0.5;
    bool isUp = playerInput.Vertical.GetAxis() > 0.5;

    // movement
    input = new Vector2(playerInput.Horizontal.GetAxis(), playerInput.Vertical.GetAxis());

    // jump
    if (playerInput.Jump.GetButtonDown()) {
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
}
