using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class Dash : MonoBehaviour {

    private CharController2D c;

    public float dashSpeed = 30;

    [Space] // debug
    public bool isDashing;
    public bool hasDashed;

    void Start() {
      c = GetComponent<CharController2D>();
    }

    // ------------------------------------------------------------------------------
    // Dash

    public void SetDash() {
      if (hasDashed) return;
      if (c.move.xRaw == 0 && c.move.yRaw == 0) return;

      float x = c.move.xRaw;
      float y = c.move.yRaw;

      // trigger ripple effect (component in main camera)
      // FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

      hasDashed = true;
      c.anim.SetTrigger("dash");

      Vector2 dir = new Vector2(x, y);
      c.rb.velocity = Vector2.zero;
      c.rb.velocity += dir.normalized * dashSpeed;

      // for some reason vertical velocity is huge compared to horizontal, so dump it
      c.rb.velocity = new Vector2(c.rb.velocity.x, c.rb.velocity.y * 0.35f);

      StartCoroutine(DashWait());
    }

    IEnumerator DashWait() {
      StartCoroutine(GroundDash());

      // progressively dump rigidbody drag
      // StartCoroutine(LerpRigidbodyDrag(14, 0, 0.8f));

      c.particles.dash.Play();
      c.rb.gravityScale = 0;
      c.jump.isBetterJumpEnabled = false;
      c.jump.wallJumped = true;
      isDashing = true;

      c.sounds.PlayDash();

      yield return new WaitForSeconds(.3f);

      c.particles.dash.Stop();
      c.rb.gravityScale = 1f; // 3;
      c.jump.isBetterJumpEnabled = true;
      c.jump.wallJumped = false;
      isDashing = false;
    }

    IEnumerator GroundDash() {
      yield return new WaitForSeconds(.15f);
      if (c.coll.onGround) hasDashed = false;
    }

    // IEnumerator LerpRigidbodyDrag(float startValue, float endValue, float duration) {
    //   float time = 0;
    //   while (time < duration) {
    //     rb.drag = Mathf.Lerp(startValue, endValue, time / duration);
    //     time += Time.deltaTime;
    //     yield return null;
    //   }
    //   rb.drag = endValue;
    // }
  }
}
