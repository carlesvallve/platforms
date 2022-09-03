using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class Ladder : MonoBehaviour {
    public int ladderHeight = 3;
    public GameObject ladderSegment;
    private GameObject ladderTop;
    private GameObject ladderBottom;
    private BoxCollider2D col;


    void Start() {
      ladderTop = transform.Find("Ladder-Top").gameObject;
      ladderBottom = transform.Find("Ladder-Bottom").gameObject;

      ladderTop.transform.localPosition = Vector3.up * (ladderHeight - 1);

      for (int i = 1; i < ladderHeight - 1; i++) {
        GameObject go = Instantiate(ladderSegment, Vector3.zero, Quaternion.identity);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.up * i;
      }

      col = transform.GetComponent<BoxCollider2D>();
      col.offset = new Vector2(0, ladderHeight * 0.5f);
      col.size = new Vector2(0.25f, ladderHeight);
    }

    void OnDrawGizmos() {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireCube(
        (Vector2)transform.position + Vector2.up * (ladderHeight * 0.5f),
        new Vector2(0.25f, ladderHeight)
      );
    }


  }

}
