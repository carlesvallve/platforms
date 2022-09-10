using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Carles.Engine2D {

  public class Trap : MonoBehaviour {

    public TMP_Text label;
    protected TrapSounds sounds;

    public int damage = 1;
    public float delayActivate = 0.25f;
    public float delayRewind = 0.5f;
    public bool isActive = false;
    public bool isRewinding = false;
    public bool hasHit = false;
    public GameObject target;

    void Start() {
      sounds = GetComponentInParent<TrapSounds>();
      label.transform.localPosition = Vector2.up * 1.5f;
      Reset();
    }

    public virtual void SetTarget(GameObject _target) {
      target = _target;
    }


    public virtual IEnumerator SetLabel(string str, float duration) {
      label.text = str;
      yield return new WaitForSeconds(duration);
      label.text = "";
    }

    protected virtual void Reset() {
      label.text = "";
      transform.localPosition = Vector2.up * -0.5f;
    }

    public virtual void Activate() {
      if (isActive) return;
      StopAllCoroutines();
      StartCoroutine(ActivateSeq());
    }

    // todo: interpolate correctly with given times in seconds

    protected virtual IEnumerator MoveTrigger() {
      isActive = true;
      isRewinding = false;
      hasHit = false;

      // trigger trap
      yield return new WaitForSeconds(0);
      sounds.PlayTrigger();
      StartCoroutine(SetLabel("Click!", 0.35f));
    }

    protected virtual IEnumerator MoveActivate() {
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

    protected virtual IEnumerator MoveRewind() {
      // rewind trap
      yield return new WaitForSeconds(delayRewind);
      isRewinding = true;
      sounds.PlayRewind();

      float d = (-0.5f - transform.localPosition.y) / 500;
      while (Mathf.Abs(transform.localPosition.y) < 0.49f) {
        transform.Translate(0, d, 0);
        yield return null;
      }

      Reset();
      // transform.localPosition = Vector2.up * -0.5f;
    }

    protected virtual IEnumerator MoveWait() {
      isActive = false;
      isRewinding = false;

      yield return new WaitForSeconds(0);

      if (target) Activate();
    }

    public virtual IEnumerator ActivateSeq() {
      yield return StartCoroutine(MoveTrigger());
      yield return StartCoroutine(MoveActivate());
      yield return StartCoroutine(MoveRewind());
      yield return StartCoroutine(MoveWait());
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision) {
      if (hasHit) return;
      if (isRewinding) return;

      Combat combat = collision.GetComponent<Combat>();
      if (combat) {
        combat.StartCoroutine(combat.TakeDamage(gameObject, damage, 3f));
      }

      hasHit = true;
    }
  }
}
