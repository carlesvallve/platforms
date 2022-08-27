using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class Jump : MonoBehaviour {

    private CharController2D c;

    public float jumpForce = 12;
    public int maxJumps = 2;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 6f;
    public float wallJumpLerp = 10;

    // jump flags
    [HideInInspector] public bool isJumpBeingPressed; // todo: change to isLongJumpEnabled
    [HideInInspector] public bool isBetterJumpEnabled = true;
    [HideInInspector] public int jumpsAvailable;
    [HideInInspector] public bool wallJumped;

    // ground flags
    private bool groundTouch;

    void Start() {
      c = GetComponent<CharController2D>();
    }

    void Update() {
      UpdateGroundTouch();
      UpdateJump();
    }

    // ------------------------------------------------------------------------------
    // Ground checks

    void UpdateGroundTouch() {
      // did we just landed on ground?
      bool isGrounded = c.coll.onGround || c.coll.onWall;

      if (isGrounded && !groundTouch) {
        GroundTouch();
        groundTouch = true;
      }

      if (!isGrounded && groundTouch) {
        groundTouch = false;
      }

      // if on ground we can jump
      if (c.coll.onGround && !c.dash.isDashing) {
        wallJumped = false;
        isBetterJumpEnabled = true;
      }

      void GroundTouch() {
        c.move.canMove = true;
        c.move.side = c.skin.GetSide();

        c.dash.hasDashed = false;
        c.dash.isDashing = false;

        c.particles.jump.Play();
        c.sounds.PlayFootstep();

        if (c.combat.isDead && c.coll.onGround) {
          c.combat.DisableAfterDying();
        }
      }
    }

    // ------------------------------------------------------------------------------
    // Jump

    void UpdateJump() {
      // Better Jump gravity
      if (isBetterJumpEnabled) {
        if (c.rb.velocity.y < 0) {
          c.rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if (c.rb.velocity.y > 0 && !isJumpBeingPressed) {
          c.rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
      }
    }

    public void SetJump(Vector2 dir, bool wall) {
      // multi-jump
      if (c.coll.onGround || c.coll.onWall) jumpsAvailable = maxJumps;
      if (jumpsAvailable == 0) return;
      jumpsAvailable -= 1;

      c.anim.SetTrigger("jump");
      c.sounds.PlayJump();

      c.particles.slide.transform.parent.localScale = new Vector3(c.move.ParticleSide(), 1, 1);
      ParticleSystem particle = wall ? c.particles.wallJump : c.particles.jump;

      c.rb.velocity = new Vector2(c.rb.velocity.x, 0);
      c.rb.velocity += dir * jumpForce;

      particle.Play();
    }

    public void SetWallJump() {
      if ((c.move.side == 1 && c.coll.onRightWall) || c.move.side == -1 && !c.coll.onRightWall) {
        c.move.side *= -1;
        c.skin.Flip(c.move.side);
      }

      StartCoroutine(c.move.DisableMovement(.1f));

      Vector2 wallDir = c.coll.onRightWall ? Vector2.left : Vector2.right;

      SetJump((Vector2.up / 1.5f + wallDir / 1.5f), true);

      wallJumped = true;
    }

  }

}
