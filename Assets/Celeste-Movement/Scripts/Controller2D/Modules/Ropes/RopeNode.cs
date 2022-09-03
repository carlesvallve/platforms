using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {
  public class RopeNode : MonoBehaviour {

    public int index;

    void OnTriggerEnter2D(Collider2D col) {
      if (col.gameObject.tag != "Player") return;

      CharController2D c = col.GetComponent<CharController2D>();

      // escape trigger if conditions are not met
      if (c.ropeClimb.isActive) return;
      if (c.hook.isActive) return;
      if (c.move.yRaw < -0.5f) return; // if pressing down

      // escape trigger if this rope already has an attached character
      Rope rope = transform.GetComponentInParent<Rope>();
      if (rope.attachedCharacter == c) return;

      // start rope climbing
      c.ropeClimb.StartRope(rope, this);
    }
  }
}
