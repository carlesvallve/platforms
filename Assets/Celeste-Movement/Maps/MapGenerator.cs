using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Carles.Engine2D {

  public class MapGenerator : MonoBehaviour {

    public Tilemap tilemap;
    public TileBase tileBase;

    public int width;
    public int height;

    public int[,] map;


    void Start() {
      map = ProceduralMap.GenerateRandomArray(width, height, 33);
      ProceduralMap.RenderMap(map, tilemap, tileBase);
    }


  }
}
