using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class LadderClimb : MonoBehaviour {

    public LayerMask ladderLayer;
    public float speed = 6f;
    public bool onLadder;

    private CharController2D c;

    void Start() {
      c = GetComponent<CharController2D>();
    }

    void Update() {
      // if (c.ropeClimb.isActive) return;
      // if (c.hook.isActive) return;

      if (onLadder) {
        c.rb.velocity = new Vector2(c.rb.velocity.x, c.move.yRaw * speed);
        // if (c.move.yRaw != 0) isClimbing = true;
      }
    }

    public void EnterLadder(GameObject ladder) {
      onLadder = true;
    }

    public void ExitLadder() {
      onLadder = false;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
      if (collision.tag != "Ladder") return;
      EnterLadder(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision) {
      if (collision.tag != "Ladder") return;
      ExitLadder();
    }

  }
}
