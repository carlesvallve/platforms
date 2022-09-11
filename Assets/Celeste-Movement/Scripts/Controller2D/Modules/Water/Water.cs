using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {
  public class Water : MonoBehaviour {

    public GameObject splashPrefab;

    private bool initialized = false;

    void Awake() {
      StartCoroutine(WaitToInitialize());
    }

    private IEnumerator WaitToInitialize() {
      yield return new WaitForEndOfFrame();
      initialized = true;
    }

    private IEnumerator WaitToMove(CharController2D c, float duration, bool value) {
      yield return StartCoroutine(c.move.DisableMovement(duration));
      c.jump.StopJump();
      // yield return new WaitForSeconds(duration);
      // c.coll.onWater = value;
      //c.move.canMove = true;
      // c.jump.StopJump();
    }

    private void PlaySplashParticles(GameObject target) {
      Vector2 pos = new Vector2(target.transform.position.x, transform.position.y);
      GameObject go = Instantiate(splashPrefab, pos, Quaternion.identity);
      go.transform.SetParent(transform);

      ParticleSystem splashParticle = go.GetComponent<ParticleSystem>();
      splashParticle.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
      // objects already on water when game starts should not trigger
      if (!initialized) return;

      PlaySplashParticles(collision.gameObject);
      GameSounds.instance.PlaySplash();

      CharController2D c = collision.gameObject.GetComponent<CharController2D>();
      if (c) {
        c.coll.onWater = true;

        // StartCoroutine(WaitToToggle(c, 0.25f, true));
        StartCoroutine(WaitToMove(c, 0.3f, true));
      }
    }

    private void OnTriggerExit2D(Collider2D collision) {
      // objects already on water when game starts should not trigger
      if (!initialized) return;

      PlaySplashParticles(collision.gameObject);
      GameSounds.instance.PlaySplash();

      CharController2D c = collision.gameObject.GetComponent<CharController2D>();
      if (c) {
        c.jump.SetJump(Vector2.up, false, true);
        c.coll.onWater = false;
      }
    }

  }
}
