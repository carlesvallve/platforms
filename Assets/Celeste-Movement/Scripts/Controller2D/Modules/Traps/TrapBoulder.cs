using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class TrapBoulder : Trap {

    public LayerMask collisionLayers;

    private Rigidbody2D rb;
    private float originalGravityScale;

    void Awake() {
      trapMode = TrapMode.Boulder;
      rb = GetComponent<Rigidbody2D>();
      originalGravityScale = rb.gravityScale;
    }

    protected override void Reset() {
      base.Reset();
      transform.localPosition = origin;
      rb.gravityScale = 0;
      rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    protected override IEnumerator MoveActivate() {
      // activate trap
      yield return new WaitForSeconds(delayActivate);

      rb.gravityScale = originalGravityScale;

      sounds.PlayTrap();
    }

    protected override IEnumerator MoveRewind() {
      // rewind trap
      yield return new WaitForSeconds(delayRewind);
      isRewinding = true;
      sounds.PlayRewind();

      rb.constraints = RigidbodyConstraints2D.FreezeAll;

      float d = (origin.y - transform.localPosition.y) / 1000;
      while (Mathf.Abs(transform.localPosition.y) < Mathf.Abs(origin.y - 0.01f)) {
        transform.Translate(0, d, 0);
        yield return null;
      }

      Reset();

      yield return StartCoroutine(MoveWait());
    }

    public override IEnumerator ActivateSeq() {
      yield return StartCoroutine(MoveTrigger());
      yield return StartCoroutine(MoveActivate());
      // yield return StartCoroutine(MoveRewind());
      // yield return StartCoroutine(MoveWait());
    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
      if (hasHit) return;
      if (isRewinding) return;
      if (!isActive) return;

      Combat combat = collision.GetComponent<Combat>();
      if (combat) {
        combat.StartCoroutine(combat.TakeDamage(gameObject, damage, knockback));
      }

      sounds.PlayBoulder();

      StartCoroutine(MoveRewind());

      hasHit = true;
    }

  }
}
