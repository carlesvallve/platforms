using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
      if (!onLadder) return;

      // snap to center of ladder
      if (c.move.yRaw != 0) {
        float d = currentLadder.transform.position.x - c.rb.position.x;
        transform.Translate(d * 0.1f, 0, 0);
      }

      c.rb.velocity = new Vector2(c.rb.velocity.x, c.move.yRaw * speed);

      currentLadder.ToggleOneWayPlatform(c.move.yRaw >= 0);
    }

    public void EnterLadder(GameObject ladder) {
      onLadder = true;
      currentLadder = ladder.GetComponent<Ladder>();
    }

    public void ExitLadder() {
      onLadder = false;
    }

    private void OnTriggerStay2D(Collider2D collision) {
      if (collision.tag != "Ladder") return;
      if (c.jump.isJumping && c.jump.ladderJumped) return;
      EnterLadder(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision) {
      if (collision.tag != "Ladder") return;
      ExitLadder();
    }

  }
}
