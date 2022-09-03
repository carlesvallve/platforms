using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// rope segments can be of different length, 
// and this will make sure that each segment is connected at the right spot

namespace Carles.Engine2D {
  public class RopeSegment : MonoBehaviour {

    public GameObject connectedAbove, connectedBelow;

    public bool isPlayerAttached;

    void Start() {
      ResetAnchor();
    }

    // public void SetCollisionTrigger(bool value) {
    //   CircleCollider2D col = GetComponent<CircleCollider2D>();
    //   col.isTrigger = value;
    // }

    public void ResetAnchor() {
      connectedAbove = GetComponent<HingeJoint2D>().connectedBody.gameObject;
      RopeSegment aboveSegment = connectedAbove.GetComponent<RopeSegment>();

      if (aboveSegment != null) {
        aboveSegment.connectedBelow = gameObject;
        float spriteBottom = connectedAbove.GetComponent<SpriteRenderer>().bounds.size.y;
        GetComponent<HingeJoint2D>().connectedAnchor = new Vector2(0, spriteBottom * -1);
      } else {
        GetComponent<HingeJoint2D>().connectedAnchor = Vector2.zero;
      }
    }
  }
}
