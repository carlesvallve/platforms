using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class TrapBoulder : Trap {

    void Awake() {
      trapMode = TrapMode.Boulder;
    }

    protected override void Reset() {
      base.Reset();
      transform.localPosition = Vector2.up * 3f;
    }

    protected override IEnumerator MoveActivate() {
      // activate trap
      yield return new WaitForSeconds(delayActivate);
      sounds.PlayTrap();

      // transform.GetComponent<Collider2D>().enabled = false;

      // move to 0 pos
      float d = (0 - transform.localPosition.y) / 50;
      while (Mathf.Abs(transform.localPosition.y) > 0.01) {
        transform.Translate(0, d, 0);
        yield return null;
      }
      transform.localPosition = Vector2.up * 0;

      sounds.PlayBoulder();
    }

    protected override IEnumerator MoveRewind() {
      // rewind trap
      yield return new WaitForSeconds(delayRewind);
      isRewinding = true;
      sounds.PlayRewind();

      // transform.GetComponent<Collider2D>().enabled = true;

      float d = (3f - transform.localPosition.y) / 1000;
      while (Mathf.Abs(transform.localPosition.y) < 2.99f) {
        transform.Translate(0, d, 0);
        yield return null;
      }

      Reset();
    }

  }
}
