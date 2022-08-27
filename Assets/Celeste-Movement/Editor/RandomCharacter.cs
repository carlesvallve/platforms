using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Carles.Engine2D {

  [CustomEditor(typeof(CharConfig), true)]
  public class RandomCharacterEditor : Editor {
    CharConfig CharConfig;

    private void Awake() {
      CharConfig = (CharConfig)target;
    }

    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      CharConfig.SetSpriteLibrary((int)CharConfig.characterType);

      EditorGUILayout.Space(10f);
      if (GUILayout.Button("Random Character")) {
        int r = CharConfig.SetSpriteLibraryRandom();
        CharConfig.characterType = (CharacterType)r;
      }


    }


  }

}
