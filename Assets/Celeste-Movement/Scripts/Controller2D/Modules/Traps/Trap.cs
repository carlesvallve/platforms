using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Carles.Engine2D {

  public class Trap : MonoBehaviour {

    public TMP_Text label;
    private TrapSounds sounds;

    public float delayActivate = 0.3f;
    public float delayReset = 2f;
    public bool active = false;

    void Start() {
      sounds = GetComponent<TrapSounds>();
      Reset();
    }

    public IEnumerator SetLabel(string str, float duration) {
      label.text = str;
      yield return new WaitForSeconds(duration);
      label.text = "";
    }

    public void Reset() {
      label.text = "";
      transform.localPosition = Vector2.up * -0.5f;
    }

    public IEnumerator Activate() {
      // trigger trap
      yield return new WaitForSeconds(0);
      sounds.PlayTrigger();
      StartCoroutine(SetLabel("Click!", 0.15f));

      // activate trap
      yield return new WaitForSeconds(delayActivate);
      sounds.PlayTrap();
      transform.localPosition = Vector2.up * 0;

      // reset trap
      yield return new WaitForSeconds(delayReset);
      Reset();

    }
  }
}
