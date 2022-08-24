using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileData : ScriptableObject {

  public TileBase[] tiles;

  public TileType type;
  public float walkingSpeed, poisonous;
}
