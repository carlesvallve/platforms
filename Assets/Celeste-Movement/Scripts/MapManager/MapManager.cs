using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour {
  private InputAction leftMouseClick;

  [SerializeField]
  private Tilemap map;

  [SerializeField]
  private List<TileData> tileDatas;

  [SerializeField]
  private SerializableDictionary<TileBase, TileData> dataFromTiles;
  // private Dictionary<TileBase, TileData> dataFromTiles;

  private void Awake() {
    // enable mouse interaction
    leftMouseClick = new InputAction(binding: "<Mouse>/leftButton");
    leftMouseClick.performed += ctx => LeftMouseClicked();
    leftMouseClick.Enable();

    // generate data from tiles
    dataFromTiles = new SerializableDictionary<TileBase, TileData>();
    foreach (var tileData in tileDatas) {
      foreach (var tile in tileData.tiles) {
        dataFromTiles.Add(tile, tileData); // key, value
      }
    }
  }

  private void LeftMouseClicked() {
    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    Vector3Int gridPosition = map.WorldToCell(mousePos);
    print("Map - mousePos" + mousePos + " gridPos:" + gridPosition);

    TileBase clickedTile = map.GetTile(gridPosition);
    if (clickedTile) print("Map - At position " + gridPosition + " there is a tile " + clickedTile);

    if (clickedTile) {
      TileType type = dataFromTiles[clickedTile].type;
      float walkingSpeed = dataFromTiles[clickedTile].walkingSpeed;
      float poisonous = dataFromTiles[clickedTile].poisonous;

      print("Map - Type: " + type + " WalkingSpeed: " + walkingSpeed + " poisonous: " + poisonous);
    }
  }

  private TileType GetTileType(Vector2 worldPos) {
    Vector3Int gridPos = map.WorldToCell(worldPos);
    TileBase tile = map.GetTile(gridPos);
    if (tile == null) return TileType.None;

    return dataFromTiles[tile].type;
  }


}

