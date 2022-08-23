using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator {
  [SerializeField]
  private int minRoomWidth = 4, minRoomHeight = 4;
  [SerializeField]
  private int dungeonWidth = 20, dungeonHeight = 20;
  [SerializeField]
  [Range(0, 10)]
  private int offset = 1;
  [SerializeField]
  private bool randomWalkRooms = false;

  private HashSet<Vector2Int> floor;
  private List<HashSet<Vector2Int>> rooms;

  protected override void RunProceduralGeneration() {
    floor = new HashSet<Vector2Int>();
    CreateRooms();
    CreateEnterExitNodes();
  }

  private void CreateEnterExitNodes() {
    // pick a random room
    // HashSet<Vector2Int> room = rooms[Random.Range(0, rooms.Count)];

    // enter: pick a random position in the first room
    List<Vector2Int> enterRoom = new List<Vector2Int>(rooms[0]);
    enterPos = enterRoom[Random.Range(0, enterRoom.Count)];

    // exit: pick a random position in the last room
    List<Vector2Int> exitRoom = new List<Vector2Int>(rooms[rooms.Count - 1]);
    exitPos = exitRoom[Random.Range(0, exitRoom.Count)];

    tilemapVisualizer.PaintEnterExitTiles(enterPos, exitPos);
  }

  private void CreateRooms() {
    List<BoundsInt> roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

    rooms = new List<HashSet<Vector2Int>>();

    if (randomWalkRooms) {
      floor = CreateRoomsRandomly(roomsList);
    } else {
      floor = CreateSimpleRooms(roomsList);
    }

    List<Vector2Int> roomCenters = new List<Vector2Int>();
    foreach (var room in roomsList) {
      roomCenters.Add((Vector2Int)Vector3Int.RoundToInt(room.center));
    }

    HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
    floor.UnionWith(corridors);

    tilemapVisualizer.PaintFloorTiles(floor);
    WallGenerator.CreateWalls(floor, tilemapVisualizer);
  }

  private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList) {
    HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

    for (int i = 0; i < roomsList.Count; i++) {
      rooms.Add(new HashSet<Vector2Int>());

      var roomBounds = roomsList[i];
      var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
      var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);

      foreach (var position in roomFloor) {
        if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset)) {
          floor.Add(position);
          rooms[i].Add(position);
        }
      }
    }
    return floor;
  }

  private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList) {
    HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

    for (int i = 0; i < roomsList.Count; i++) {
      var room = roomsList[i];
      // foreach (var room in roomsList) {
      rooms.Add(new HashSet<Vector2Int>());

      for (int col = offset; col < room.size.x - offset; col++) {
        for (int row = offset; row < room.size.y - offset; row++) {
          Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
          floor.Add(position);
          rooms[i].Add(position);
        }
      }
    }
    return floor;
  }

  private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters) {
    HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
    var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
    roomCenters.Remove(currentRoomCenter);

    while (roomCenters.Count > 0) {
      Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
      roomCenters.Remove(closest);

      HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
      currentRoomCenter = closest;
      corridors.UnionWith(newCorridor);
    }
    return corridors;
  }

  private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination) {
    // pick a random corridor width preset for this corridor 
    CorridorWidthType corridorWidthType = ProceduralGenerationAlgorithms.GetRandomCorridorWidthType();

    // get width of the corridor depending on corridor width preset
    int width = ProceduralGenerationAlgorithms.GetCorridorWidthByType(corridorWidthType);

    HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
    var position = currentRoomCenter;
    corridor.Add(position);
    while (position.y != destination.y) {
      if (destination.y > position.y) {
        position += Vector2Int.up;
      } else if (destination.y < position.y) {
        position += Vector2Int.down;
      }
      corridor.Add(position);

      // sparsify corridor width
      if (corridorWidthType == CorridorWidthType.RandAll) width = ProceduralGenerationAlgorithms.GetCorridorWidthByType(corridorWidthType);
      if (width > 1) corridor.Add(position - Vector2Int.right);
      if (width > 2) corridor.Add(position + Vector2Int.right);

    }
    while (position.x != destination.x) {
      if (destination.x > position.x) {
        position += Vector2Int.right;
      } else if (destination.x < position.x) {
        position += Vector2Int.left;
      }
      corridor.Add(position);

      // sparsify corridor width
      if (corridorWidthType == CorridorWidthType.RandAll) width = ProceduralGenerationAlgorithms.GetCorridorWidthByType(corridorWidthType);
      if (width > 1) corridor.Add(position - Vector2Int.up);
      if (width > 2) corridor.Add(position + Vector2Int.up);
    }
    return corridor;
  }

  private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters) {
    Vector2Int closest = Vector2Int.zero;
    float distance = float.MaxValue;
    foreach (var position in roomCenters) {
      float currentDistance = Vector2.Distance(position, currentRoomCenter);
      if (currentDistance < distance) {
        distance = currentDistance;
        closest = position;
      }
    }
    return closest;
  }


}
