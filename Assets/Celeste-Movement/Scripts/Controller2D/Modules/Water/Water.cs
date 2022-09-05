using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {
  public class Water : MonoBehaviour {

    void Start() { }

    private void EnterWater(GameObject target) {
      StartCoroutine(WaitToSink(target));
    }

    private IEnumerator WaitToSink(GameObject target) {
      yield return new WaitForSeconds(0.25f);
      CharController2D c = target.GetComponent<CharController2D>();
      if (c) c.coll.onWater = true;
    }

    private void ExitWater(GameObject target) {
      CharController2D c = target.GetComponent<CharController2D>();
      if (c) {
        c.coll.onWater = false;
        c.jump.SetJump(Vector2.up, false, true);
      }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
      if (collision.tag != "Player") return;
      EnterWater(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision) {
      if (collision.tag != "Player") return;
      ExitWater(collision.gameObject);
    }

  }
}
