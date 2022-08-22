using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace Carles.Engine2D {

  public class ProceduralMap : MonoBehaviour {

    public static int[,] GenerateArray(int width, int height, bool empty) {
      int[,] map = new int[width, height];

      for (int x = 0; x < map.GetUpperBound(0); x++) {
        for (int y = 0; y < map.GetUpperBound(1); y++) {
          if (empty) {
            map[x, y] = 0;
          } else {
            map[x, y] = 1;
          }
        }
      }
      return map;
    }

    public static int[,] GenerateRandomArray(int width, int height, int prob = 50) {
      int[,] map = new int[width, height];

      for (int x = 0; x < map.GetUpperBound(0); x++) {
        for (int y = 0; y < map.GetUpperBound(1); y++) {
          int r = Random.Range(0, 100);
          int value = r < prob ? 1 : 0;
          Debug.Log(value);
          map[x, y] = value;
        }
      }
      return map;
    }

    public static void RenderMap(int[,] map, Tilemap tilemap, TileBase tile) {
      //Clear the map (ensures we dont overlap)
      tilemap.ClearAllTiles();

      for (int x = 0; x < map.GetUpperBound(0); x++) {
        for (int y = 0; y < map.GetUpperBound(1); y++) {
          // 1 = tile, 0 = no tile
          if (map[x, y] == 1) {
            tilemap.SetTile(new Vector3Int(x, y, 0), tile);
          }
        }
      }
    }

    public static void UpdateMap(int[,] map, Tilemap tilemap) //Takes in our map and tilemap, setting null tiles where needed
{
      for (int x = 0; x < map.GetUpperBound(0); x++) {
        for (int y = 0; y < map.GetUpperBound(1); y++) {
          //We are only going to update the map, rather than rendering again
          //This is because it uses less resources to update tiles to null
          //As opposed to re-drawing every single tile (and collision data)
          if (map[x, y] == 0) {
            tilemap.SetTile(new Vector3Int(x, y, 0), null);
          }
        }
      }
    }

  }
}
