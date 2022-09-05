using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo;: Enter while onGround or if input y is presing up

namespace Carles.Engine2D {

  public class LadderClimb : MonoBehaviour {

    public LayerMask ladderLayer;
    public float speed = 6f;
    public bool onLadder;
    public Ladder currentLadder;

    private CharController2D c;

    void Start() {
      c = GetComponent<CharController2D>();
    }

    void Update() {
      // if (c.ropeClimb.isActive) return;
      // if (c.hook.isActive) return;

      if (!onLadder) return;

      c.rb.velocity = new Vector2(c.rb.velocity.x, c.move.yRaw * speed);

      // if going down, disable ladder's one-way-platform
      // if (c.move.yRaw < 0) {

      // }

      currentLadder.ToggleOneWayPlatform(c.move.yRaw >= 0);
    }

    public void EnterLadder(GameObject ladder) {
      onLadder = true;
      currentLadder = ladder.GetComponent<Ladder>();
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
