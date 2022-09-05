using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {
  public class Water : MonoBehaviour {

    public GameObject splashPrefab;

    void Start() { }

    private void PlaySplashParticles(GameObject target) {
      Vector2 pos = new Vector2(target.transform.position.x, transform.position.y);
      GameObject go = Instantiate(splashPrefab, pos, Quaternion.identity);
      go.transform.SetParent(transform);

      ParticleSystem splashParticle = go.GetComponent<ParticleSystem>();
      splashParticle.Play();
    }

    private IEnumerator WaitToToggle(CharController2D c, float duration, bool value) {
      yield return new WaitForSeconds(duration);
      c.coll.onWater = value;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
      PlaySplashParticles(collision.gameObject);
      // if (collision.tag != "Player") return;

      CharController2D c = collision.gameObject.GetComponent<CharController2D>();
      if (c) {
        c.sounds.PlaySplash();

        StartCoroutine(WaitToToggle(c, 0.25f, true));
      }
    }

    private void OnTriggerExit2D(Collider2D collision) {
      PlaySplashParticles(collision.gameObject);
      // if (collision.tag != "Player") return;

      CharController2D c = collision.gameObject.GetComponent<CharController2D>();
      if (c) {
        c.sounds.PlaySplash();

        c.jump.SetJump(Vector2.up, false, true);

        c.coll.onWater = false;
      }
    }

  }
}
