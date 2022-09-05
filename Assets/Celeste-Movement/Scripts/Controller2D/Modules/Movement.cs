using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class Movement : MonoBehaviour {

    private CharController2D c;

    public float speed = 6;
    public float slideSpeed = 2.5f;

    [Space] // debug
    public bool canMove;
    public bool wallGrab;
    public bool wallSlide;
    public int side = 1;
    public Vector2 incs;

    // move
    [HideInInspector] public Vector2 curMoveInput;
    [HideInInspector] public float xRaw;
    [HideInInspector] public float yRaw;

    // walls
    [HideInInspector] public bool isGrabBeingPressed; // todo: change to isGrabEnabled

    void Start() {
      c = GetComponent<CharController2D>();
    }

    void Update() {
      // get gravity scale
      c.rb.gravityScale = GetGravityScale();

      // get movement increments
      float x = curMoveInput.x;
      float y = curMoveInput.y;

      // if on water, swim
      if (c.coll.onWater) {
        if (x != 0 || y != 0) {
          float down = 0; //-0.05f; // c.jump.isJumping ? 0 : -0.2f;
          c.rb.velocity = new Vector2(x * 0.7f, (y + down) * 0.7f) * speed;
        }
        return;
      }

      // move slower while blocking
      if (c.combat.isBlocking && c.coll.onGround) {
        x *= 0.5f;
        y *= 0.5f;
      }

      UpdateCharSide(x);
      UpdateWalk(x, y);
      UpdateWalls(x, y);


    }

    private float GetGravityScale() {
      if (wallGrab) return 0;
      if (c.dash.isDashing) return 0;
      if (c.ladderClimb.onLadder) return 0;

      return 3f;
    }

    // ------------------------------------------------------------------------------
    // Walk

    private void UpdateWalk(float x, float y) {
      xRaw = x;
      yRaw = y;
      Vector2 dir = new Vector2(x, y);
      incs = dir;

      // escape walking if we are on any kind of rope
      if (!c.coll.onGround) {
        if (c.hook.isActive) return;
        // if (c.hook.isActive && !c.coll.onGround && !c.coll.onWall) return;
      }

      Walk(dir);
      c.anim.SetHorizontalCharController2D(x, y, c.rb.velocity.y);
    }

    private void Walk(Vector2 dir) {
      if (!canMove) return;
      if (wallGrab) return;

      if (!c.jump.wallJumped) {
        c.rb.velocity = new Vector2(dir.x * speed, c.rb.velocity.y);
      } else {
        c.rb.velocity = Vector2.Lerp(c.rb.velocity, (new Vector2(dir.x * speed, c.rb.velocity.y)), c.jump.wallJumpLerp * Time.deltaTime);
      }
    }

    public IEnumerator DisableMovement(float time) {
      canMove = false;
      yield return new WaitForSeconds(time);
      canMove = true;
    }

    // ------------------------------------------------------------------------------
    // Walls

    private void UpdateWalls(float x, float y) {
      // wall grab flags

      if (c.coll.onWall && isGrabBeingPressed && canMove && !c.combat.isBlocking) {
        if (side != c.coll.wallSide) c.skin.Flip(side * -1);
        wallGrab = true;
        wallSlide = false;
      }

      if (!isGrabBeingPressed || !c.coll.onWall || !canMove || c.combat.isBlocking) {
        wallGrab = false;
        wallSlide = false;
      }

      // wall grab

      if (wallGrab && !c.dash.isDashing) {
        c.rb.gravityScale = 0;
        if (x > .2f || x < -.2f) {
          c.rb.velocity = new Vector2(c.rb.velocity.x, 0);
        }

        float speedModifier = y > 0 ? .5f : 1;
        c.rb.velocity = new Vector2(c.rb.velocity.x, y * (speed * speedModifier));
      } else {
        // c.rb.gravityScale = 3; // todo: expose default gravityScale prop
      }

      // wall slide

      if (c.coll.onWall && !c.coll.onGround && !c.combat.isAttacking) {
        if (x != 0 && !wallGrab && c.rb.velocity.y < 0) {
          wallSlide = true;
          WallSlide();
        }
      }

      if (!c.coll.onWall || c.coll.onGround || c.combat.isAttacking) {
        wallSlide = false;
      }

      WallParticle(y);
    }

    private void WallSlide() {
      if (!canMove) return;

      if (c.coll.wallSide != side) c.skin.Flip(side * -1);

      bool pushingWall = false;
      if ((c.rb.velocity.x > 0 && c.coll.onRightWall) || (c.rb.velocity.x < 0 && c.coll.onLeftWall)) {
        pushingWall = true;
      }
      float push = pushingWall ? 0 : c.rb.velocity.x;

      c.rb.velocity = new Vector2(push, -slideSpeed);

      c.sounds.PlaySlide();
    }

    void WallParticle(float vertical) {
      var main = c.particles.slide.main;

      if (wallSlide || (wallGrab && vertical < 0)) {
        c.particles.slide.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        main.startColor = Color.white;
      } else {
        main.startColor = Color.clear;
      }
    }

    public int ParticleSide() {
      int particleSide = c.coll.onRightWall ? 1 : -1;
      return particleSide;
    }

    // ------------------------------------------------------------------------------
    // Side

    void UpdateCharSide(float x) {
      // escape if on walls or cannot move for some reason
      if (wallGrab || wallSlide || !canMove) return;

      // turn right
      if (x > 0) {
        side = 1;
        c.skin.Flip(side);
      }

      // turn left
      if (x < 0) {
        side = -1;
        c.skin.Flip(side);
      }
    }

  }
}
