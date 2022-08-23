﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator {
  [SerializeField]
  protected SimpleRandomWalkSO randomWalkParameters;

  protected override void RunProceduralGeneration() {
    HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
    tilemapVisualizer.Clear();
    tilemapVisualizer.PaintFloorTiles(floorPositions);
    WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);

    // pick a random position in the single room floor
    List<Vector2Int> floor = new List<Vector2Int>(floorPositions);
    enterPos = floor[Random.Range(0, floor.Count)];
    exitPos = floor[Random.Range(0, floor.Count)];
    tilemapVisualizer.PaintEnterExitTiles(enterPos, exitPos);
  }

  protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO parameters, Vector2Int position) {
    var currentPosition = position;
    HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
    for (int i = 0; i < parameters.iterations; i++) {
      var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walkLength);
      floorPositions.UnionWith(path);
      if (parameters.startRandomlyEachIteration)
        currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
    }
    return floorPositions;
  }

}
