﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Carles.Engine2D {

  public class AnimationScript : MonoBehaviour {

    public Sounds sounds;
    public SpriteLibraryAsset[] spriteLibs;
    public int splIndex;

    private Movement move;
    private Collision coll;
    private Animator anim;
    [HideInInspector] public SpriteRenderer sr;

    void Start() {
      coll = GetComponentInParent<Collision>();
      move = GetComponentInParent<Movement>();
      anim = GetComponent<Animator>();
      sr = GetComponent<SpriteRenderer>();

      SetSpriteLibraryRandom();
    }

    void Update() {
      anim.SetBool("canMove", move.canMove);
      anim.SetBool("onGround", coll.onGround);
      anim.SetBool("onWall", coll.onWall);
      anim.SetBool("onRightWall", coll.onRightWall);
      anim.SetBool("wallGrab", move.wallGrab);
      anim.SetBool("wallSlide", move.wallSlide);
      anim.SetBool("isDashing", move.isDashing);
    }

    public void SetHorizontalMovement(float x, float y, float yVel) {
      anim.SetFloat("HorizontalAxis", x);
      anim.SetFloat("VerticalAxis", y);
      anim.SetFloat("VerticalVelocity", yVel);
    }

    public void SetTrigger(string trigger) {
      anim.SetTrigger(trigger);
    }

    public void Flip(int side) {
      if (move.wallGrab || move.wallSlide) {
        if (side == -1 && sr.flipX) return;
        if (side == 1 && !sr.flipX) return;
      }

      bool state = (side == 1) ? false : true;
      sr.flipX = state;
    }

    public void SetSpriteLibraryRandom() {
      SetSpriteLibrary(Random.Range(0, spriteLibs.Length));
    }

    public void SetSpriteLibrary(int index) {
      splIndex = index;
      SpriteLibrary spl = GetComponent<SpriteLibrary>();
      spl.spriteLibraryAsset = spriteLibs[splIndex];
    }

    public void PlayFootstep() {
      sounds.PlayFootstep();
    }
  }

}
