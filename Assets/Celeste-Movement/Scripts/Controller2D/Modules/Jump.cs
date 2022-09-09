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

    public bool isJumping;

    [Space] // debug
    public bool groundTouch;
    public bool ladderJumped;
    public bool wallJumped;
    public bool oneWayPlatformJumped;
    public int jumpsAvailable;


    // jump flags
    [HideInInspector] public bool isJumpBeingPressed; // todo: change to isLongJumpEnabled
    [HideInInspector] public bool isBetterJumpEnabled = true;

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
      // did we just landed on ground or touched a wall?
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
    }

    void GroundTouch() {
      isJumping = false;
      ladderJumped = false;

      c.move.canMove = true;
      c.move.side = c.skin.GetSide();

      c.dash.hasDashed = false;
      c.dash.isDashing = false;

      c.transform.rotation = Quaternion.identity;
      c.rb.freezeRotation = true;

      // if (c.rb.velocity.y < 0 && !c.coll.currentOneWayPlatform) {
      if (!oneWayPlatformJumped) {

        if (c.rb.velocity.y <= 0) {
          c.particles.jump.Play();
          c.sounds.PlayFootstep();
        }

      }

      if (c.combat.isDead && c.coll.onGround) {
        c.combat.DisableAfterDying();
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

    public void SetJumpsAvailable(int _maxJumps) {
      jumpsAvailable = _maxJumps;
    }

    // private IEnumerator StartJumping() {
    //   isJumpBeingPressed = true;
    //   yield return new WaitForSeconds(0.3f);
    //   isJumping 
    // }

    public void SetJump(Vector2 dir, bool fromWall, bool fromWater = false) {
      // one-way-platform jump?
      if (c.coll.currentOneWayPlatform && c.move.yRaw < 0) {
        SetOneWayPlatformJump(dir, fromWall, fromWater);
        return;
      }

      // multi-jump
      if (c.coll.onGround || c.coll.onWall || fromWater) SetJumpsAvailable(maxJumps);
      if (jumpsAvailable == 0) return;
      jumpsAvailable -= 1;

      // exit ladder if we are on it
      if (c.ladderClimb.onLadder) {
        ladderJumped = true;
        c.ladderClimb.ExitLadder();
      }

      c.anim.SetTrigger("jump");

      // todo: still triggering when we are jumping from water...
      if (!c.coll.onWater || fromWater) {
        c.sounds.PlayJump();
        c.particles.slide.transform.parent.localScale = new Vector3(c.move.ParticleSide(), 1, 1);
        ParticleSystem particle = fromWall ? c.particles.wallJump : c.particles.jump;
        particle.Play();
      }

      float finalJumpForce = c.move.xRaw == 0 && c.move.yRaw < 0 ? jumpForce * 0.5f : jumpForce;

      c.rb.velocity = new Vector2(c.rb.velocity.x, 0);
      c.rb.velocity += dir * finalJumpForce;

      isJumping = true;
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
      isJumping = true;
    }

    private void SetOneWayPlatformJump(Vector2 dir, bool fromWall, bool fromWater) {
      c.anim.SetTrigger("jump");

      if (!c.coll.onWater || fromWater) {
        c.sounds.PlayJump();
        c.particles.slide.transform.parent.localScale = new Vector3(c.move.ParticleSide(), 1, 1);
        ParticleSystem particle = fromWall ? c.particles.wallJump : c.particles.jump;
        particle.Play();
      }

      c.rb.velocity = new Vector2(c.rb.velocity.x, 0);
      c.rb.velocity += dir * jumpForce * 0.5f;

      StartCoroutine(DisableOneWayPlatform());

      isJumping = true;
    }

    private IEnumerator DisableOneWayPlatform() {
      Collider2D playerCollider = c.GetComponent<Collider2D>();
      Collider2D platformCollider = c.coll.currentOneWayPlatform.GetComponent<Collider2D>();

      oneWayPlatformJumped = true;
      Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
      yield return new WaitForSeconds(0.4f);
      Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
      oneWayPlatformJumped = false;
    }

  }

}
