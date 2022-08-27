using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Carles.Engine2D {

  public class AnimationScript : MonoBehaviour {

    public Sounds sounds;
    public int spriteLibIndex;
    public SpriteLibraryAsset[] spriteLibs;
    public bool randomizeOnStart = true;

    [Space]
    [Header("Projectiles")]
    public GameObject arrowPrefab;
    public GameObject darkMagickPrefab;
    public GameObject fireMagickPrefab;

    private Movement move;
    private Collision coll;
    private Animator anim;
    [HideInInspector] public SpriteRenderer sr;

    void Start() {
      coll = GetComponentInParent<Collision>();
      move = GetComponentInParent<Movement>();
      anim = GetComponent<Animator>();
      sr = GetComponent<SpriteRenderer>();

      if (randomizeOnStart) SetSpriteLibraryRandom();
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
      if (spriteLibs.Length == 0) return;

      spriteLibIndex = index;
      SpriteLibrary spl = GetComponent<SpriteLibrary>();
      spl.spriteLibraryAsset = spriteLibs[spriteLibIndex];
    }

    public CharacterType GetCharacterType() {
      return (CharacterType)spriteLibIndex;
    }

    public GameObject GetProjectilePrefab() {
      if (IsArcher()) return arrowPrefab;
      if (GetCharacterType() == CharacterType.DarkWizard) return darkMagickPrefab;
      if (GetCharacterType() == CharacterType.FireWizard) return fireMagickPrefab;
      if (GetCharacterType() == CharacterType.WhiteWizard) return fireMagickPrefab;
      return null;
    }

    public bool IsMelee() {
      return !IsArcher() && !IsWizard();
    }

    public bool IsArcher() {
      CharacterType type = GetCharacterType();
      return type == CharacterType.Archer || type == CharacterType.GoblinArcher;
    }

    public bool IsWizard() {
      CharacterType type = GetCharacterType();
      return type == CharacterType.DarkWizard || type == CharacterType.FireWizard || type == CharacterType.WhiteWizard;
    }

    public bool hasShield() {
      CharacterType type = GetCharacterType();
      return type == CharacterType.Knight || type == CharacterType.Pirate || type == CharacterType.Spearman || type == CharacterType.Viking;
    }

    public void PlayFootstep() {
      sounds.PlayFootstep();
    }
  }

}
