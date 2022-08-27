using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Carles.Engine2D {

  [CustomEditor(typeof(CharSkin), true)]
  public class CharSkinEditor : Editor {

    CharSkin charSkin;

    private void Awake() {
      charSkin = (CharSkin)target;
    }

    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      charSkin.SetSpriteLibrary((int)charSkin.characterType);

      EditorGUILayout.Space(2f);
      if (GUILayout.Button("Random Character")) {
        int r = charSkin.SetSpriteLibraryRandom();
        charSkin.characterType = (CharacterType)r;
      }
    }

  }
}
