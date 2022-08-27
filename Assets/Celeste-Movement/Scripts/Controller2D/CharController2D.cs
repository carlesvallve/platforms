
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo
// one way platforms
// ladders
// ropes
// grappling hook
// slopes
// melee
// shooting
// damage
// detah
// inventory
// souls

// todo
// procedural levels
// - add variable corridor width -> OK
// - add enter and exit nodes -> OK
// - add platforms
// - add one way platforms
// - add movable platforms
// - add destroyable platforms
// - add traps
// - add ladders
// - add elevators
// - add doors
// - add teleporters

namespace Carles.Engine2D {

  public class CharController2D : MonoBehaviour {
    [HideInInspector] public Rigidbody2D rb;
    //
    [HideInInspector] public Collision coll;
    [HideInInspector] public Movement move;
    [HideInInspector] public Jump jump;
    //
    [HideInInspector] public CharConfig ch;
    [HideInInspector] public CharAnimation anim;
    [HideInInspector] public Sounds sounds;

    [Space]
    [Header("Stats")]
    private int health = 3;
    public float dashSpeed = 20;


    [Space]
    [Header("Attack")]
    public LayerMask attackLayer;
    public float attackSpeed = 0.15f;
    public float attackCooldown = 0.15f;
    private Rect attackRect = new Rect(0.5f, 0, 0.8f, 1.6f);
    private int attackDamage = 1;
    // private float knockbackForce = 4f;
    private float dazedDuration = 0.15f;

    [Space]
    [Header("Polish")]
    public ParticleSystem trailParticle;
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;
    public ParticleSystem bloodParticle;
    public ParticleSystem impactParticle;

    // Flags

    // input flags
    [HideInInspector] public bool isJumpBeingPressed; // todo: change to isLongJumpEnabled
    [HideInInspector] public bool isGrabBeingPressed; // todo: change to isGrabEnabled

    [HideInInspector] public bool isDashing;
    [HideInInspector] public bool hasDashed;

    // combat flags
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isBlocking;
    [HideInInspector] public bool isTakingDamage;
    [HideInInspector] public bool isDead;

    // ------------------------------------------------------------------------------
    // Start and Update

    void Start() {
      rb = GetComponent<Rigidbody2D>();
      //
      coll = GetComponent<Collision>();
      move = GetComponent<Movement>();
      jump = GetComponent<Jump>();
      //
      ch = GetComponentInChildren<CharConfig>();
      anim = GetComponentInChildren<CharAnimation>();
      sounds = GetComponentInChildren<Sounds>();
    }

    // ------------------------------------------------------------------------------
    // Dash

    public void Dash() {
      if (hasDashed) return;
      if (move.xRaw == 0 && move.yRaw == 0) return;

      float x = move.xRaw;
      float y = move.yRaw;

      // trigger ripple effect (component in main camera)
      FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));

      hasDashed = true;
      anim.SetTrigger("dash");

      Vector2 dir = new Vector2(x, y);
      rb.velocity = Vector2.zero;
      rb.velocity += dir.normalized * dashSpeed;

      StartCoroutine(DashWait());
    }

    IEnumerator DashWait() {
      StartCoroutine(GroundDash());

      // progressively dump rigidbody drag
      // StartCoroutine(LerpRigidbodyDrag(14, 0, 0.8f));

      dashParticle.Play();
      rb.gravityScale = 0;
      jump.isBetterJumpEnabled = false;
      jump.wallJumped = true;
      isDashing = true;

      sounds.PlayDash();

      yield return new WaitForSeconds(.3f);

      dashParticle.Stop();
      rb.gravityScale = 3;
      jump.isBetterJumpEnabled = true;
      jump.wallJumped = false;
      isDashing = false;
    }

    IEnumerator GroundDash() {
      yield return new WaitForSeconds(.15f);
      if (coll.onGround) hasDashed = false;
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

    // ------------------------------------------------------------------------------
    // Attack

    public void Attack() {
      if (isAttacking) return;
      if (move.wallGrab) return;
      if (move.wallSlide) return;

      if (ch.IsMelee()) {
        StartCoroutine(AttackSeq());
      } else {
        StartCoroutine(ShootSeq());
      }
    }

    IEnumerator AttackSeq() {
      anim.SetTrigger("attack");
      sounds.PlayAttack();
      isAttacking = true;

      // get which direction character is looking at
      int side = ch.GetSide();

      // reset attacker's velocity
      rb.velocity = new Vector2(0, rb.velocity.y);
      if (Mathf.Abs(rb.velocity.x) < 1) {
        Vector2 vec = new Vector2(2.5f * side, 2.5f);
        rb.AddForce(vec, ForceMode2D.Impulse);
      }

      // disable CharController2D while attacking
      StartCoroutine(move.DisableMovement(attackCooldown));

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
        if (!enemy.isDead) {
          StartCoroutine(enemy.TakeDamage(this, attackDamage, 4f));
        }
      }

      // wait for attack cooldown to recover
      yield return new WaitForSeconds(attackCooldown);
      isAttacking = false;
    }

    void OnDrawGizmos() {
      if (!isAttacking) return;
      Gizmos.color = Color.red;
      int side = ch.GetSide();
      Vector2 pos = (Vector2)transform.position + new Vector2(attackRect.x, attackRect.y) * Vector2.right * side;
      Gizmos.DrawWireCube(pos, new Vector2(attackRect.width, attackRect.height));
    }

    // ------------------------------------------------------------------------------
    // Shoot

    IEnumerator ShootSeq() {
      anim.SetTrigger("attack");
      sounds.PlayAttack();
      isAttacking = true;

      // reset attacker's velocity
      rb.velocity = new Vector2(0, rb.velocity.y);
      if (Mathf.Abs(rb.velocity.x) < 1) {
        Vector2 vec = new Vector2(0, 2.5f);
        rb.AddForce(vec, ForceMode2D.Impulse);
      }

      // disable CharController2D while attacking
      StartCoroutine(move.DisableMovement(attackCooldown));

      yield return new WaitForSeconds(attackSpeed);

      // Instantiate projectile
      GameObject projectilePrefab = ch.GetProjectilePrefab();
      Vector2 pos = new Vector2(transform.position.x + 0.3f, transform.position.y - 0.1f);
      GameObject go = Instantiate(projectilePrefab, pos, Quaternion.identity);
      Projectile projectile = go.GetComponent<Projectile>();
      projectile.Init(this, ch.GetSide());

      // wait for attack cooldown to recover
      yield return new WaitForSeconds(attackCooldown);
      isAttacking = false;
    }

    // ------------------------------------------------------------------------------
    // Block

    public void Block() {
      if (isBlocking) return;
      if (move.wallGrab) return;
      if (move.wallSlide) return;



      isBlocking = true;
      sounds.PlayBlock();
    }

    public void Unblock() {
      if (!isBlocking) return;

      isBlocking = false;
      sounds.PlayBlock();
    }

    // ------------------------------------------------------------------------------
    // Take Damage

    public IEnumerator TakeDamage(CharController2D attacker, int damage, float knockbackForce = 0) {
      isTakingDamage = true;
      move.canMove = false;

      sounds.PlayDamage();
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
      move.canMove = true;
    }

    public void Knockback(CharController2D attacker, float knockbackForce = 0) {
      Vector2 dir = (transform.position - attacker.transform.position).normalized;

      rb.AddForce(dir * knockbackForce * 1, ForceMode2D.Impulse);
      rb.AddForce(Vector2.up * 2.5f, ForceMode2D.Impulse);
    }

    // ------------------------------------------------------------------------------
    // Die

    public IEnumerator Die() {
      move.canMove = false;
      isTakingDamage = false;
      isDead = true;
      health = 0;

      sounds.PlayDie();

      yield return new WaitForSeconds(dazedDuration * 2);

      if (coll.onGround) DisableAfterDying();
    }

    public void DisableAfterDying() {
      GetComponent<Collider2D>().enabled = false;
      GetComponentInChildren<Collider2D>().enabled = false;
      rb.constraints = RigidbodyConstraints2D.FreezeAll;
      trailParticle.Stop();
    }

    // ------------------------------------------------------------------------------
    // Combat Particles

    public void SpawnBlood(float damage) {
      if (!bloodParticle) {
        Debug.Log("Blood partices not found in " + name);
        return;
      }

      bloodParticle.transform.localPosition = Vector3.zero;

      // Generate a particle burst 
      // (Should work, as we can see burstCount updates correctly in Emmission, 
      // but for some reason always spawns same multiple amount of particles)
      var em = bloodParticle.emission;
      em.enabled = true;
      em.rateOverTime = 0;
      em.SetBursts(new ParticleSystem.Burst[] {
        new ParticleSystem.Burst(0f, damage),
      });

      bloodParticle.Play();
    }

    public void SpawnImpact(float damage) {
      if (!impactParticle) {
        Debug.Log("Impact partices not found in " + name);
        return;
      }

      impactParticle.transform.localPosition = Vector3.zero;
      impactParticle.Play();
    }

  }
}
