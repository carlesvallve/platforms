using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Carles.Engine2D {

  [CustomEditor(typeof(AnimationScript), true)]
  public class RandomCharacterEditor : Editor {
    AnimationScript animationScript;

    private void Awake() {
      animationScript = (AnimationScript)target;
    }

    public override void OnInspectorGUI() {
      base.OnInspectorGUI();
      EditorGUILayout.Space(5f);
      if (GUILayout.Button("Random Character")) {
        animationScript.SetSpriteLibraryRandom();
      }
    }
  }

}
