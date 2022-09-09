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

  public class Skin : MonoBehaviour {

    public SpriteLibraryAsset[] spriteLibs;
    public bool randomizeOnStart = true;
    public CharacterType characterType;

    private CharController2D c;
    private Animator anim;
    private SpriteRenderer sprite;
    private int spriteLibIndex;

    void Start() {
      c = GetComponent<CharController2D>();
      anim = GetComponentInChildren<Animator>();
      sprite = GetComponentInChildren<SpriteRenderer>();

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

      SpriteLibrary spl = GetComponentInChildren<SpriteLibrary>();
      spl.spriteLibraryAsset = spriteLibs[spriteLibIndex];
    }

    public CharacterType GetCharacterType() {
      return (CharacterType)spriteLibIndex;
    }

    public GameObject GetProjectilePrefab() {
      if (IsArcher()) return c.combat.arrowPrefab;
      if (GetCharacterType() == CharacterType.DarkWizard) return c.combat.darkMagickPrefab;
      if (GetCharacterType() == CharacterType.FireWizard) return c.combat.fireMagickPrefab;
      if (GetCharacterType() == CharacterType.WhiteWizard) return c.combat.fireMagickPrefab;
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

    public void Flip(int side) {
      if (c.move.wallGrab || c.move.wallSlide) {
        if (side == -1 && sprite.flipX) return;
        if (side == 1 && !sprite.flipX) return;
      }

      bool state = (side == 1) ? false : true;
      sprite.flipX = state;
    }

    public int GetSide() {
      return sprite.flipX ? -1 : 1;
    }
  }

}
