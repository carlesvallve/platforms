using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

namespace Carles.Engine2D.Dungeon {

  public class TilemapVisualizer : MonoBehaviour {
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;
    [SerializeField]
    private TileBase floorTile, wallTop, wallSideRight, wallSiderLeft, wallBottom, wallFull,
        wallInnerCornerDownLeft, wallInnerCornerDownRight,
        wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;
    [SerializeField]
    private TileBase enterTile, exitTile;

    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions) {
      PaintTiles(floorPositions, floorTilemap, floorTile);
    }

    public void PaintEnterExitTiles(Vector2Int enterPos, Vector2Int exitPos) {
      // todo: this actually should be single tiles with special trigger colliders

      // paint enter and exit tiles
      PaintSingleTile(floorTilemap, enterTile, enterPos);
      PaintSingleTile(floorTilemap, exitTile, exitPos);

      // locate player at entrance
      PlayerInput pi = FindObjectOfType<PlayerInput>();
      if (pi) pi.transform.position = new Vector3(enterPos.x, enterPos.y, 0);
      // Debug.Log(pi + " at pos " + enterPos);
    }



    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile) {
      foreach (var position in positions) {
        PaintSingleTile(tilemap, tile, position);
      }
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType) {
      int typeAsInt = Convert.ToInt32(binaryType, 2);
      TileBase tile = null;
      if (WallTypesHelper.wallTop.Contains(typeAsInt)) {
        tile = wallTop;
      } else if (WallTypesHelper.wallSideRight.Contains(typeAsInt)) {
        tile = wallSideRight;
      } else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt)) {
        tile = wallSiderLeft;
      } else if (WallTypesHelper.wallBottm.Contains(typeAsInt)) {
        tile = wallBottom;
      } else if (WallTypesHelper.wallFull.Contains(typeAsInt)) {
        tile = wallFull;
      }

      if (tile != null)
        PaintSingleTile(wallTilemap, tile, position);
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position) {
      var tilePosition = tilemap.WorldToCell((Vector3Int)position);
      tilemap.SetTile(tilePosition, tile);
    }

    public void Clear() {
      floorTilemap.ClearAllTiles();
      wallTilemap.ClearAllTiles();
    }

    internal void PaintSingleCornerWall(Vector2Int position, string binaryType) {
      int typeASInt = Convert.ToInt32(binaryType, 2);
      TileBase tile = null;

      if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeASInt)) {
        tile = wallInnerCornerDownLeft;
      } else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeASInt)) {
        tile = wallInnerCornerDownRight;
      } else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeASInt)) {
        tile = wallDiagonalCornerDownLeft;
      } else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeASInt)) {
        tile = wallDiagonalCornerDownRight;
      } else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeASInt)) {
        tile = wallDiagonalCornerUpRight;
      } else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeASInt)) {
        tile = wallDiagonalCornerUpLeft;
      } else if (WallTypesHelper.wallFullEightDirections.Contains(typeASInt)) {
        tile = wallFull;
      } else if (WallTypesHelper.wallBottmEightDirections.Contains(typeASInt)) {
        tile = wallBottom;
      }

      if (tile != null)
        PaintSingleTile(wallTilemap, tile, position);
    }
  }

}
