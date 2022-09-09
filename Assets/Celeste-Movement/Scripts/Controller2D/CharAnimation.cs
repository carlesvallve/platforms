using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class CharAnimation : MonoBehaviour {

    private CharController2D c;
    private Animator anim;

    void Start() {
      // Note: This component needs to be on the same gameobject as the animator
      c = GetComponentInParent<CharController2D>();
      anim = GetComponent<Animator>();
    }

    void Update() {
      anim.SetBool("canMove", c.move.canMove);
      anim.SetBool("onGround", c.coll.onGround);
      anim.SetBool("onWater", c.coll.onWater);
      anim.SetBool("wallGrab", c.move.wallGrab);
      anim.SetBool("wallSlide", c.move.wallSlide);
      anim.SetBool("onLadder", c.ladderClimb.onLadder);
      anim.SetBool("isRoping", c.hook.isActive);
      anim.SetBool("isDashing", c.dash.isDashing);
      anim.SetBool("isAttacking", c.combat.isAttacking);
      anim.SetBool("isBlocking", c.combat.isBlocking);
      anim.SetBool("isTakingDamage", c.combat.isTakingDamage);
      anim.SetBool("isDead", c.combat.isDead);
    }

    public void SetHorizontalCharController2D(float x, float y, float yVel) {
      anim.SetFloat("HorizontalAxis", x);
      anim.SetFloat("VerticalAxis", y);
      anim.SetFloat("VerticalVelocity", yVel);
    }

    public void SetTrigger(string trigger) {
      anim.SetTrigger(trigger);
    }

    public void PlayFootstep() {
      // this method is called by animation events
      c.sounds.PlayFootstep();
    }

  }
}
