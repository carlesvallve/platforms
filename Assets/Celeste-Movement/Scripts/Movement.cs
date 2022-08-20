﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;


public class Movement : MonoBehaviour {
  private Collision coll;
  [HideInInspector]
  public Rigidbody2D rb;
  private AnimationScript anim;

  [Space]
  [Header("Stats")]
  public float speed = 10;
  public float jumpForce = 50;
  public int maxJumps = 2;
  public float fallMultiplier = 2.5f;
  public float lowJumpMultiplier = 2f;
  public float slideSpeed = 5;
  public float wallJumpLerp = 10;
  public float dashSpeed = 20;

  [Space]
  [Header("Booleans")]
  public bool canMove;
  public bool wallGrab;
  public bool wallJumped;
  public bool wallSlide;
  public bool isDashing;

  [Space]

  private bool groundTouch;
  private bool hasDashed;

  public int side = 1;

  [Space]
  [Header("Polish")]
  public ParticleSystem dashParticle;
  public ParticleSystem jumpParticle;
  public ParticleSystem wallJumpParticle;
  public ParticleSystem slideParticle;

  // 

  private int jumpsAvailable;
  private Vector2 curMoveInput;
  private float xRaw;
  private float yRaw;

  //

  private bool isJumpBeingPressed;
  private bool isDashBeingPressed;
  private bool isGrabBeingPressed;
  private bool isBetterJumpEnabled = true;

  [SerializeField] private AudioSource _source;
  [SerializeField] private AudioClip[] _footsteps;

  // ------------------------------------------------------------------------------
  // Input

  public void OnInputMove(InputAction.CallbackContext context) {
    curMoveInput = context.ReadValue<Vector2>();
  }

  public void OnInputJump(InputAction.CallbackContext context) {
    Debug.Log("OnInputJump " + context.phase);

    isJumpBeingPressed = context.phase != InputActionPhase.Canceled;

    // first frame that button is pressed down
    if (context.phase == InputActionPhase.Performed) {

      // multi-jump
      if (coll.onGround || coll.onWall) jumpsAvailable = maxJumps;
      if (jumpsAvailable == 0) return;
      jumpsAvailable -= 1;

      anim.SetTrigger("jump");

      if (!coll.onWall) Jump(Vector2.up, false); // coll.onGround check is implicit
      if (coll.onWall && !coll.onGround) WallJump();
    }
  }

  public void OnInputDash(InputAction.CallbackContext context) {
    isDashBeingPressed = context.phase != InputActionPhase.Canceled;

    if (hasDashed) return;

    // first frame that button is pressed down
    if (context.phase == InputActionPhase.Performed) {
      if (xRaw != 0 || yRaw != 0) Dash(xRaw, yRaw);
    }
  }

  public void OnInputGrab(InputAction.CallbackContext context) {
    isGrabBeingPressed = context.phase != InputActionPhase.Canceled;
  }

  // ------------------------------------------------------------------------------
  // Start and Update

  // Start is called before the first frame update
  void Start() {
    coll = GetComponent<Collision>();
    rb = GetComponent<Rigidbody2D>();
    anim = GetComponentInChildren<AnimationScript>();
  }

  // Update is called once per frame
  void FixedUpdate() {

    // get movement increments
    float x = curMoveInput.x;
    float y = curMoveInput.y;
    xRaw = x;
    yRaw = y;
    Vector2 dir = new Vector2(x, y);

    Walk(dir);
    anim.SetHorizontalMovement(x, y, rb.velocity.y);

    // Better Jump
    if (isBetterJumpEnabled) {
      if (rb.velocity.y < 0) {
        rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
      } else if (rb.velocity.y > 0 && !isJumpBeingPressed) {
        rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
      }
    }

    // flags for wall grab/slide
    if (coll.onWall && isGrabBeingPressed && canMove) {
      if (side != coll.wallSide) anim.Flip(side * -1);
      wallGrab = true;
      wallSlide = false;
    }
    if (!isGrabBeingPressed || !coll.onWall || !canMove) {
      wallGrab = false;
      wallSlide = false;
    }

    // if on ground we can jump
    if (coll.onGround && !isDashing) {
      wallJumped = false;
      isBetterJumpEnabled = true;
    }

    if (wallGrab && !isDashing) {
      rb.gravityScale = 0;
      if (x > .2f || x < -.2f)
        rb.velocity = new Vector2(rb.velocity.x, 0);

      float speedModifier = y > 0 ? .5f : 1;

      rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
    } else {
      rb.gravityScale = 3;
    }

    if (coll.onWall && !coll.onGround) {
      if (x != 0 && !wallGrab) {
        wallSlide = true;
        WallSlide();
      }
    }

    if (!coll.onWall || coll.onGround) {
      wallSlide = false;
    }

    if (coll.onGround && !groundTouch) {
      GroundTouch();
      groundTouch = true;
    }

    if (!coll.onGround && groundTouch) {
      groundTouch = false;
    }

    WallParticle(y);

    if (wallGrab || wallSlide || !canMove)
      return;

    if (x > 0) {
      side = 1;
      anim.Flip(side);
    }
    if (x < 0) {
      side = -1;
      anim.Flip(side);
    }
  }

  void GroundTouch() {
    hasDashed = false;
    isDashing = false;

    side = anim.sr.flipX ? -1 : 1;

    jumpParticle.Play();

    _source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
  }

  // ------------------------------------------------------------------------------
  // Dash

  private void Dash(float x, float y) {
    FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

    hasDashed = true;

    anim.SetTrigger("dash");

    rb.velocity = Vector2.zero;
    Vector2 dir = new Vector2(x, y);

    rb.velocity += dir.normalized * dashSpeed;
    StartCoroutine(DashWait());
  }

  IEnumerator DashWait() {
    // FindObjectOfType<GhostTrail>().ShowGhost();
    StartCoroutine(GroundDash());

    // progressively dump rigidbody drag
    StartCoroutine(LerpRigidbodyDrag(14, 0, 0.8f));

    dashParticle.Play();
    rb.gravityScale = 0;
    isBetterJumpEnabled = false;
    wallJumped = true;
    isDashing = true;

    yield return new WaitForSeconds(.3f);

    dashParticle.Stop();
    rb.gravityScale = 3;
    isBetterJumpEnabled = true;
    wallJumped = false;
    isDashing = false;
  }

  IEnumerator GroundDash() {
    yield return new WaitForSeconds(.15f);
    if (coll.onGround)
      hasDashed = false;
  }

  IEnumerator LerpRigidbodyDrag(float startValue, float endValue, float duration) {
    float time = 0;
    // float startValue = startValue;
    while (time < duration) {
      rb.drag = Mathf.Lerp(startValue, endValue, time / duration);
      time += Time.deltaTime;
      yield return null;
    }
    rb.drag = endValue;
  }

  // ------------------------------------------------------------------------------

  private void WallJump() {
    if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall) {
      side *= -1;
      anim.Flip(side);
    }

    StopCoroutine(DisableMovement(0));
    StartCoroutine(DisableMovement(.1f));

    Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

    Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

    wallJumped = true;
  }

  private void WallSlide() {
    if (coll.wallSide != side)
      anim.Flip(side * -1);

    if (!canMove)
      return;

    bool pushingWall = false;
    if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall)) {
      pushingWall = true;
    }
    float push = pushingWall ? 0 : rb.velocity.x;

    rb.velocity = new Vector2(push, -slideSpeed);
  }

  private void Walk(Vector2 dir) {
    if (!canMove)
      return;

    if (wallGrab)
      return;

    if (!wallJumped) {
      rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
    } else {
      rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
    }
  }

  private void Jump(Vector2 dir, bool wall) {
    slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
    ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

    rb.velocity = new Vector2(rb.velocity.x, 0);
    rb.velocity += dir * jumpForce;

    particle.Play();
  }

  IEnumerator DisableMovement(float time) {
    canMove = false;
    yield return new WaitForSeconds(time);
    canMove = true;
  }

  void WallParticle(float vertical) {
    var main = slideParticle.main;

    if (wallSlide || (wallGrab && vertical < 0)) {
      slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
      main.startColor = Color.white;
    } else {
      main.startColor = Color.clear;
    }
  }

  int ParticleSide() {
    int particleSide = coll.onRightWall ? 1 : -1;
    return particleSide;
  }
}
