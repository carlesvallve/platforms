using UnityEngine;
using System.Collections;
using Carles;

public class Player : Humanoid {

  [HideInInspector]
  public Hud hud;

  private PlayerControls playerControls;
  private int hd_C = 0;

  public override void Awake() {
    playerControls = transform.GetComponent<PlayerControls>();

    GameObject go = GameObject.Find("Hud");
    hud = go ? go.GetComponent<Hud>() : null;
    base.Awake();
  }

  protected override void SetInput() {
    bool isDown = playerControls.Vertical.GetAxis() < -0.5;
    bool isUp = playerControls.Vertical.GetAxis() > 0.5;

    // movement
    input = new Vector2(playerControls.Horizontal.GetAxis(), playerControls.Vertical.GetAxis());

    // jump
    if (playerControls.Jump.GetButtonDown()) {
      SetJump(isDown, isUp ? 1.25f : 1f);
    }

    // attack
    if (playerControls.Attack.GetButtonDown()) {
      SetAttack(isDown);
    }

    // timed action
    if (playerControls.Action.GetButtonUp()) {
      hd_C = 0;
      SetAction();
    }

    if (playerControls.Action.GetButton()) {
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
