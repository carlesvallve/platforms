using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Carles.Engine2D.Dungeon {

  [CustomEditor(typeof(AbstractDungeonGenerator), true)]
  public class RandomDungeonGeneratorEditor : Editor {
    AbstractDungeonGenerator generator;

    private void Awake() {
      generator = (AbstractDungeonGenerator)target;
    }

    public override void OnInspectorGUI() {
      base.OnInspectorGUI();
      EditorGUILayout.Space(10f);
      if (GUILayout.Button("Create Dungeon")) generator.GenerateDungeon();
      EditorGUILayout.Space(10f);
    }
  }

}
