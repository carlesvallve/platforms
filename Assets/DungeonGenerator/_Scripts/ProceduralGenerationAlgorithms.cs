using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CorridorWidthType {
  One = 1,
  Two = 2,
  Three = 3,
  Rand = 4,
  RandAll = 5
}

public static class ProceduralGenerationAlgorithms {

  public static CorridorWidthType GetRandomCorridorWidthType() {
    // return a random corridor width preset for this corridor 
    return (CorridorWidthType)Random.Range(0, System.Enum.GetValues(typeof(CorridorWidthType)).Length + 1);
  }

  public static int GetCorridorWidthByType(CorridorWidthType width) {
    if (width == CorridorWidthType.One) return 1;
    if (width == CorridorWidthType.One) return 2;
    if (width == CorridorWidthType.One) return 3;
    return Random.Range(1, 3 + 1);
  }

  public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength) {
    HashSet<Vector2Int> path = new HashSet<Vector2Int>();

    path.Add(startPosition);
    var previousPosition = startPosition;

    for (int i = 0; i < walkLength; i++) {
      var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection();
      path.Add(newPosition);
      previousPosition = newPosition;
    }
    return path;
  }

  public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength) {
    // pick a random corridor width preset for this corridor 
    CorridorWidthType corridorWidthType = ProceduralGenerationAlgorithms.GetRandomCorridorWidthType();

    // get width of the corridor depending on corridor width preset
    int width = ProceduralGenerationAlgorithms.GetCorridorWidthByType(corridorWidthType);

    List<Vector2Int> corridor = new List<Vector2Int>();
    var direction = Direction2D.GetRandomCardinalDirection();
    var currentPosition = startPosition;
    corridor.Add(currentPosition);

    for (int i = 0; i < corridorLength; i++) {
      currentPosition += direction;
      corridor.Add(currentPosition);

      Debug.Log(direction + " " + (direction == Vector2Int.up) + " " + (direction == Vector2Int.right));

      // sparsify corridor width (vertical corridors)
      if (direction == Vector2Int.up) {
        if (corridorWidthType == CorridorWidthType.RandAll) width = ProceduralGenerationAlgorithms.GetCorridorWidthByType(corridorWidthType);
        if (width > 1) corridor.Add(currentPosition - Vector2Int.right);
        if (width > 2) corridor.Add(currentPosition + Vector2Int.right);
      }

      if (direction == Vector2Int.right) {
        // sparsify corridor width (horizontal corridors)
        if (corridorWidthType == CorridorWidthType.RandAll) width = ProceduralGenerationAlgorithms.GetCorridorWidthByType(corridorWidthType);
        if (width > 1) corridor.Add(currentPosition - Vector2Int.up);
        if (width > 2) corridor.Add(currentPosition + Vector2Int.up);
      }
    }
    return corridor;
  }

  public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit, int minWidth, int minHeight) {
    Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();
    List<BoundsInt> roomsList = new List<BoundsInt>();
    roomsQueue.Enqueue(spaceToSplit);
    while (roomsQueue.Count > 0) {
      var room = roomsQueue.Dequeue();
      if (room.size.y >= minHeight && room.size.x >= minWidth) {
        if (Random.value < 0.5f) {
          if (room.size.y >= minHeight * 2) {
            SplitHorizontally(minHeight, roomsQueue, room);
          } else if (room.size.x >= minWidth * 2) {
            SplitVertically(minWidth, roomsQueue, room);
          } else if (room.size.x >= minWidth && room.size.y >= minHeight) {
            roomsList.Add(room);
          }
        } else {
          if (room.size.x >= minWidth * 2) {
            SplitVertically(minWidth, roomsQueue, room);
          } else if (room.size.y >= minHeight * 2) {
            SplitHorizontally(minHeight, roomsQueue, room);
          } else if (room.size.x >= minWidth && room.size.y >= minHeight) {
            roomsList.Add(room);
          }
        }
      }
    }
    return roomsList;
  }

  private static void SplitVertically(int minWidth, Queue<BoundsInt> roomsQueue, BoundsInt room) {
    var xSplit = Random.Range(1, room.size.x);
    BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(xSplit, room.size.y, room.size.z));
    BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + xSplit, room.min.y, room.min.z),
        new Vector3Int(room.size.x - xSplit, room.size.y, room.size.z));
    roomsQueue.Enqueue(room1);
    roomsQueue.Enqueue(room2);
  }

  private static void SplitHorizontally(int minHeight, Queue<BoundsInt> roomsQueue, BoundsInt room) {
    var ySplit = Random.Range(1, room.size.y);
    BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x, ySplit, room.size.z));
    BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x, room.min.y + ySplit, room.min.z),
        new Vector3Int(room.size.x, room.size.y - ySplit, room.size.z));
    roomsQueue.Enqueue(room1);
    roomsQueue.Enqueue(room2);
  }
}

public static class Direction2D {
  public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
  {
        new Vector2Int(0,1), //UP
        new Vector2Int(1,0), //RIGHT
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(-1, 0) //LEFT
    };

  public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
  {
        new Vector2Int(1,1), //UP-RIGHT
        new Vector2Int(1,-1), //RIGHT-DOWN
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 1) //LEFT-UP
    };

  public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
  {
        new Vector2Int(0,1), //UP
        new Vector2Int(1,1), //UP-RIGHT
        new Vector2Int(1,0), //RIGHT
        new Vector2Int(1,-1), //RIGHT-DOWN
        new Vector2Int(0, -1), // DOWN
        new Vector2Int(-1, -1), // DOWN-LEFT
        new Vector2Int(-1, 0), //LEFT
        new Vector2Int(-1, 1) //LEFT-UP

    };

  public static Vector2Int GetRandomCardinalDirection() {
    return cardinalDirectionsList[UnityEngine.Random.Range(0, cardinalDirectionsList.Count)];
  }
}