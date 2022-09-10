using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Carles.Engine2D {

  public class Trap : MonoBehaviour {

    public TMP_Text label;
    private TrapSounds sounds;

    public float delayActivate = 0.25f;
    public float delayRewind = 0.5f;
    public bool isActive = false;
    public bool isRewinding = false;
    public bool hasHit = false;
    public GameObject target;

    void Start() {
      sounds = GetComponent<TrapSounds>();
      Reset();
    }

    public void SetTarget(GameObject _target) {
      target = _target;
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

    public void Activate() {
      if (isActive) return;
      StopAllCoroutines();
      StartCoroutine(ActivateSeq());
    }

    // todo: interpolate correctly with given times in seconds

    public IEnumerator ActivateSeq() {
      isActive = true;
      isRewinding = false;
      hasHit = false;

      float d;

      // trigger trap
      yield return new WaitForSeconds(0);
      sounds.PlayTrigger();
      StartCoroutine(SetLabel("Click!", 0.35f));

      // activate trap
      yield return new WaitForSeconds(delayActivate);
      sounds.PlayTrap();

      d = (0 - transform.localPosition.y) / 50;
      while (Mathf.Abs(transform.localPosition.y) > 0.01) {
        transform.Translate(0, d, 0);
        yield return null;
      }
      transform.localPosition = Vector2.up * 0;

      // rewind trap
      yield return new WaitForSeconds(delayRewind);
      isRewinding = true;
      sounds.PlayRewind();

      d = (-0.5f - transform.localPosition.y) / 500;
      while (Mathf.Abs(transform.localPosition.y) < 0.49f) {
        transform.Translate(0, d, 0);
        yield return null;
      }
      transform.localPosition = Vector2.up * -0.5f;

      isActive = false;
      isRewinding = false;

      if (target) {
        Activate();
      }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
      if (hasHit) return;
      if (isRewinding) return;

      Combat combat = collision.GetComponent<Combat>();
      if (combat) {
        combat.StartCoroutine(combat.TakeDamage(gameObject, 1, 3f));
      }

      hasHit = true;
    }

    // private void OnTriggerExit2D(Collider2D collision) {

    // }
  }
}
