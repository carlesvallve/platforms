using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D.Dungeon {

  public abstract class AbstractDungeonGenerator : MonoBehaviour {
    [SerializeField]
    protected bool randomizeOnStart = false;
    [SerializeField]
    protected TilemapVisualizer tilemapVisualizer = null;
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero;
    //[SerializeField]
    protected Vector2Int enterPos;
    //[SerializeField]
    protected Vector2Int exitPos;

    void Start() {
      if (randomizeOnStart) GenerateDungeon();
    }

    public void GenerateDungeon() {
      tilemapVisualizer.Clear();
      RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
  }

}
