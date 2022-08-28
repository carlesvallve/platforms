using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class Combat : MonoBehaviour {

    private CharController2D c;

    [Header("Stats")]
    public int health = 3;

    [Space]
    [Header("Attack")]
    public LayerMask attackLayer;
    public float attackSpeed = 0.15f;
    public float attackCooldown = 0.15f;
    private Rect attackRect = new Rect(0.5f, 0, 0.8f, 1.6f);
    public int attackDamage = 1;
    private float dazedDuration = 0.15f;

    [Space] // debug
    public bool isAttacking;
    public bool isBlocking;
    public bool isTakingDamage;
    public bool isDead;

    [Space]
    [Header("Projectiles")]
    public GameObject arrowPrefab;
    public GameObject darkMagickPrefab;
    public GameObject fireMagickPrefab;

    void Start() {
      c = GetComponent<CharController2D>();
    }


    // ------------------------------------------------------------------------------
    // Attack

    public void Attack() {
      if (isAttacking) return;
      if (c.move.wallGrab) return;
      if (c.move.wallSlide) return;

      if (c.skin.IsMelee()) {
        StartCoroutine(AttackSeq());
      } else {
        StartCoroutine(ShootSeq());
      }
    }

    IEnumerator AttackSeq() {
      c.anim.SetTrigger("attack");
      c.sounds.PlayAttack();
      isAttacking = true;

      // get which direction character is looking at
      int side = c.skin.GetSide();

      // reset attacker's velocity
      c.rb.velocity = new Vector2(0, c.rb.velocity.y);
      if (Mathf.Abs(c.rb.velocity.x) < 1) {
        Vector2 vec = new Vector2(2.5f * side, 2.5f);
        c.rb.AddForce(vec, ForceMode2D.Impulse);
      }

      // disable CharController2D while attacking
      StartCoroutine(c.move.DisableMovement(attackCooldown));

      yield return new WaitForSeconds(attackSpeed);

      // todo: We may want to switch to CircleCastAll, it will give you contact points in world space coordinates
      // todo: https://docs.unity3d.com/ScriptReference/Physics2D.CircleCastAll.html

      // detect enemies to hit
      Vector2 pos = (Vector2)transform.position + new Vector2(attackRect.x, attackRect.y) * Vector2.right * side;
      Collider2D[] enemiesToDamage = Physics2D.OverlapBoxAll(
        pos, new Vector2(attackRect.width, attackRect.height), 0, attackLayer
      );

      //  make each enemy take damage
      for (int i = 0; i < enemiesToDamage.Length; i++) {
        if (enemiesToDamage[i].transform.root == transform) continue;
        CharController2D enemy = enemiesToDamage[i].GetComponent<CharController2D>();
        if (!enemy.combat.isDead) {
          StartCoroutine(enemy.combat.TakeDamage(c, attackDamage, 4f));
        }
      }

      // wait for attack cooldown to recover
      yield return new WaitForSeconds(attackCooldown);
      isAttacking = false;
    }

    void OnDrawGizmos() {
      if (!isAttacking) return;
      Gizmos.color = Color.red;
      int side = c.skin.GetSide();
      Vector2 pos = (Vector2)transform.position + new Vector2(attackRect.x, attackRect.y) * Vector2.right * side;
      Gizmos.DrawWireCube(pos, new Vector2(attackRect.width, attackRect.height));
    }

    // ------------------------------------------------------------------------------
    // Shoot

    IEnumerator ShootSeq() {
      c.anim.SetTrigger("attack");
      c.sounds.PlayAttack();
      isAttacking = true;

      // reset attacker's velocity
      c.rb.velocity = new Vector2(0, c.rb.velocity.y);
      if (Mathf.Abs(c.rb.velocity.x) < 1) {
        Vector2 vec = new Vector2(0, 2.5f);
        c.rb.AddForce(vec, ForceMode2D.Impulse);
      }

      // disable CharController2D while attacking
      StartCoroutine(c.move.DisableMovement(attackCooldown));

      yield return new WaitForSeconds(attackSpeed);

      // Instantiate projectile
      GameObject projectilePrefab = c.skin.GetProjectilePrefab();
      Vector2 pos = new Vector2(transform.position.x + 0.3f, transform.position.y - 0.1f);
      GameObject go = Instantiate(projectilePrefab, pos, Quaternion.identity);
      Projectile projectile = go.GetComponent<Projectile>();
      projectile.Init(c, c.skin.GetSide());

      // wait for attack cooldown to recover
      yield return new WaitForSeconds(attackCooldown);
      isAttacking = false;
    }

    // ------------------------------------------------------------------------------
    // Block

    public void Block() {
      if (isBlocking) return;
      if (c.move.wallGrab) return;
      if (c.move.wallSlide) return;



      isBlocking = true;
      c.sounds.PlayBlock();
    }

    public void Unblock() {
      if (!isBlocking) return;

      isBlocking = false;
      c.sounds.PlayBlock();
    }

    // ------------------------------------------------------------------------------
    // Take Damage

    public IEnumerator TakeDamage(CharController2D attacker, int damage, float knockbackForce = 0) {
      isTakingDamage = true;
      c.move.canMove = false;

      c.sounds.PlayDamage();
      SpawnBlood(damage);

      Knockback(attacker, knockbackForce);

      health -= damage;
      if (health <= 0) {
        StartCoroutine(Die());
        yield break;
      }

      // stop CharController2D wile taking damage (dazed)
      yield return new WaitForSeconds(dazedDuration);

      isTakingDamage = false;
      c.move.canMove = true;
    }

    public void Knockback(CharController2D attacker, float knockbackForce = 0) {
      Vector2 dir = (transform.position - attacker.transform.position).normalized;

      c.rb.AddForce(dir * knockbackForce * 1, ForceMode2D.Impulse);
      c.rb.AddForce(Vector2.up * 2.5f, ForceMode2D.Impulse);
    }

    // ------------------------------------------------------------------------------
    // Die

    public IEnumerator Die() {
      c.move.canMove = false;
      isTakingDamage = false;
      isDead = true;
      health = 0;

      c.sounds.PlayDie();

      yield return new WaitForSeconds(dazedDuration * 2);

      if (c.coll.onGround) DisableAfterDying();
    }

    public void DisableAfterDying() {
      GetComponent<Collider2D>().enabled = false;
      GetComponentInChildren<Collider2D>().enabled = false;
      c.rb.constraints = RigidbodyConstraints2D.FreezeAll;
      c.particles.trail.Stop();
    }

    // ------------------------------------------------------------------------------
    // Combat Particles

    public void SpawnBlood(float damage) {
      if (!c.particles.blood) {
        Debug.Log("Blood partices not found in " + name);
        return;
      }

      c.particles.blood.transform.localPosition = Vector3.zero;

      // Generate a particle burst 
      // (Should work, as we can see burstCount updates correctly in Emmission, 
      // but for some reason always spawns same multiple amount of particles)
      var em = c.particles.blood.emission;
      em.enabled = true;
      em.rateOverTime = 0;
      em.SetBursts(new ParticleSystem.Burst[] {
        new ParticleSystem.Burst(0f, damage),
      });

      c.particles.blood.Play();
    }

    public void SpawnImpact(float damage) {
      if (!c.particles.impact) {
        Debug.Log("Impact partices not found in " + name);
        return;
      }

      c.particles.impact.transform.localPosition = Vector3.zero;
      c.particles.impact.Play();
    }

  }
}
