using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class LadderClimb : MonoBehaviour {

    public float speed = 6f;
    public bool isLadder;
    public bool isClimbing;

    private CharController2D c;

    void Start() {
      c = GetComponent<CharController2D>();
    }

    void Update() {
      // if (isLadder && c.move.yRaw != 0) {
      //   isClimbing = true;
      // }

      if (isLadder) {
        c.rb.velocity = new Vector2(c.rb.velocity.x, c.move.yRaw * speed);

        if (c.move.yRaw != 0) {
          isClimbing = true;
        }
      }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
      if (collision.tag != "Ladder") return;
      isLadder = true;
    }

    private void OnTriggerExit2D(Collider2D collision) {
      if (collision.tag != "Ladder") return;
      isLadder = false;
      isClimbing = false;
    }

  }
}
