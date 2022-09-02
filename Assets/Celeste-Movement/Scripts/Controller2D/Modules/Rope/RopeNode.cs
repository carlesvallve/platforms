using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {
  public class RopeNode : MonoBehaviour {

    public int index;

    void OnTriggerEnter2D(Collider2D col) {
      if (col.gameObject.tag != "Player") return;

      CharController2D c = col.GetComponent<CharController2D>();
      if (c.hook.isHookActive) return;
      if (c.move.yRaw < -0.5f) return; // if pressing down skip colliding

      Rope rope = transform.GetComponentInParent<Rope>();
      if (rope.attachedCharacter == c) return;

      // Debug.Log("Player contacted with rope trigger!");
      rope.AttachCharacter(c, this);
    }
  }
}
