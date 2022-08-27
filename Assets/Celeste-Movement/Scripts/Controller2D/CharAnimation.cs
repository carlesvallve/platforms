using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Carles.Engine2D {

  public class CharAnimation : MonoBehaviour {

    // public Sounds sounds;

    // [Space]
    // public CharacterType characterType;
    // public int spriteLibIndex;
    // public SpriteLibraryAsset[] spriteLibs;
    // public bool randomizeOnStart = true;

    // [Space]
    // [Header("Projectiles")]
    // public GameObject arrowPrefab;
    // public GameObject darkMagickPrefab;
    // public GameObject fireMagickPrefab;

    private CharController2D c;
    // private Collision coll;
    private Animator anim;
    // [HideInInspector] public SpriteRenderer sr;

    void Start() {
      // coll = GetComponentInParent<Collision>();
      c = GetComponentInParent<CharController2D>();
      anim = GetComponent<Animator>();
      // sr = GetComponent<SpriteRenderer>();

      // if (randomizeOnStart) SetSpriteLibraryRandom();
    }

    void Update() {
      anim.SetBool("canMove", c.move.canMove);
      anim.SetBool("onGround", c.coll.onGround);
      anim.SetBool("wallGrab", c.move.wallGrab);
      anim.SetBool("wallSlide", c.move.wallSlide);
      anim.SetBool("isDashing", c.isDashing);
      anim.SetBool("isAttacking", c.isAttacking);
      anim.SetBool("isBlocking", c.isBlocking);
      anim.SetBool("isTakingDamage", c.isTakingDamage);
      anim.SetBool("isDead", c.isDead);
    }

    public void SetHorizontalCharController2D(float x, float y, float yVel) {
      anim.SetFloat("HorizontalAxis", x);
      anim.SetFloat("VerticalAxis", y);
      anim.SetFloat("VerticalVelocity", yVel);
    }

    public void SetTrigger(string trigger) {
      anim.SetTrigger(trigger);
    }




  }

}
