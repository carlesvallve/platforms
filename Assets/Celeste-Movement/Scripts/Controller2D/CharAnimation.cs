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

    private CharController2D move;
    private Collision coll;
    private Animator anim;
    // [HideInInspector] public SpriteRenderer sr;

    void Start() {
      coll = GetComponentInParent<Collision>();
      move = GetComponentInParent<CharController2D>();
      anim = GetComponent<Animator>();
      // sr = GetComponent<SpriteRenderer>();

      // if (randomizeOnStart) SetSpriteLibraryRandom();
    }

    void Update() {
      anim.SetBool("canMove", move.canMove);
      anim.SetBool("onGround", coll.onGround);
      anim.SetBool("wallGrab", move.wallGrab);
      anim.SetBool("wallSlide", move.wallSlide);
      anim.SetBool("isDashing", move.isDashing);
      anim.SetBool("isAttacking", move.isAttacking);
      anim.SetBool("isBlocking", move.isBlocking);
      anim.SetBool("isTakingDamage", move.isTakingDamage);
      anim.SetBool("isDead", move.isDead);
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
