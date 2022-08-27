using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Carles.Engine2D {

  public enum CharacterType {
    Archer,
    DarkWizard,
    FireWizard,
    Goblin,
    GoblinArcher,
    Knight,
    Naked,
    Orc,
    Pirate,
    Skeleton,
    Spearman,
    Thief,
    Viking,
    WhiteWizard,
  }

  public class CharConfig : MonoBehaviour {

    public Sounds sounds;

    [Space]
    public CharacterType characterType;
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
    [HideInInspector] public SpriteRenderer sprite;

    void Start() {
      coll = GetComponentInParent<Collision>();
      move = GetComponentInParent<Movement>();
      anim = GetComponent<Animator>();
      sprite = GetComponent<SpriteRenderer>();

      if (randomizeOnStart) SetSpriteLibraryRandom();
    }

    public int SetSpriteLibraryRandom() {
      int r = Random.Range(0, spriteLibs.Length);
      SetSpriteLibrary(r);
      return r;
    }

    public void SetSpriteLibrary(int index) {
      if (spriteLibs.Length == 0) return;

      characterType = (CharacterType)index;
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

    public void Flip(int side) {
      if (move.wallGrab || move.wallSlide) {
        if (side == -1 && sprite.flipX) return;
        if (side == 1 && !sprite.flipX) return;
      }

      bool state = (side == 1) ? false : true;
      sprite.flipX = state;
    }
  }

}
