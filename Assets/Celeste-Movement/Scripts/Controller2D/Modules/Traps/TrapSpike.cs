using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Carles.Engine2D {

  public class TrapSpike : Trap {

    protected override void Reset() {
      base.Reset();
      transform.localPosition = Vector2.up * -0.5f;
    }

    protected override IEnumerator MoveActivate() {
      // activate trap
      yield return new WaitForSeconds(delayActivate);
      sounds.PlayTrap();

      float d = (0 - transform.localPosition.y) / 50;
      while (Mathf.Abs(transform.localPosition.y) > 0.01) {
        transform.Translate(0, d, 0);
        yield return null;
      }
      transform.localPosition = Vector2.up * 0;
    }

    protected override IEnumerator MoveRewind() {
      // rewind trap
      yield return new WaitForSeconds(delayRewind);
      isRewinding = true;
      sounds.PlayRewind();

      float d = (-0.5f - transform.localPosition.y) / 500;
      while (Mathf.Abs(transform.localPosition.y) < 0.49f) {
        transform.Translate(0, d, 0);
        yield return null;
      }
      transform.localPosition = Vector2.up * -0.5f;
    }

  }
}
