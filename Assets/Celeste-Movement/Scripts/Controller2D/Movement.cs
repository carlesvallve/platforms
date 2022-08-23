
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

  public class Movement : MonoBehaviour {
    private Collision coll;
    private Rigidbody2D rb;
    private AnimationScript anim;
    private Sounds sounds;

    [Space]
    [Header("Stats")]
    private int health = 3;
    public float speed = 10;
    public float jumpForce = 50;
    public int maxJumps = 2;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 6f;
    public float slideSpeed = 2.5f;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;


    [Space]
    [Header("Attack")]
    public LayerMask attackLayer;
    public float attackSpeed = 0.15f;
    public float attackCooldown = 0.15f;
    private Rect attackRect = new Rect(0.5f, 0, 1, 2);
    // private Vector2 attackRect = new Vector2(1, 2);
    // private Vector2 attackPos = new Vector2(0.5f, 0);
    private int attackDamage = 1;
    private float knockbackForce = 10f;
    private float dazedDuration = 0.15f;

    [Space]
    [Header("Skills")]
    public bool canJump = true;
    public bool canDash = true;
    public bool canWallSlide = true;
    public bool canWallGrab = true;
    public bool canAttack = true;
    public bool canBlock = true;

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

    //  movement flags
    [HideInInspector] public bool canMove;
    [HideInInspector] public Vector2 curMoveInput;
    private float xRaw;
    private float yRaw;

    // side flags
    private int side = 1;

    // ground flags
    private bool groundTouch;

    // jump flags
    private bool isBetterJumpEnabled = true;
    private int jumpsAvailable;

    // wall flags
    [HideInInspector] public bool wallGrab;
    [HideInInspector] public bool wallJumped;
    [HideInInspector] public bool wallSlide;
    [HideInInspector] public bool isDashing;
    private bool hasDashed;

    // combat flags
    [HideInInspector] public bool isAttacking;
    [HideInInspector] public bool isBlocking;
    [HideInInspector] public bool isDead;

    // ------------------------------------------------------------------------------
    // Start and Update

    void Start() {
      coll = GetComponent<Collision>();
      rb = GetComponent<Rigidbody2D>();
      anim = GetComponentInChildren<AnimationScript>();
      sounds = GetComponentInChildren<Sounds>();
    }

    void Update() {
      // get movement increments
      float x = curMoveInput.x;
      float y = curMoveInput.y;

      UpdateWalk(x, y);
      UpdateGroundTouch();
      UpdateJump();
      UpdateWalls(x, y);
      UpdateCharSide(x);
    }

    // ------------------------------------------------------------------------------
    // Walk

    void UpdateWalk(float x, float y) {
      xRaw = x;
      yRaw = y;
      Vector2 dir = new Vector2(x, y);

      Walk(dir);
      anim.SetHorizontalMovement(x, y, rb.velocity.y);
    }

    private void Walk(Vector2 dir) {
      if (!canMove) return;
      if (wallGrab) return;

      if (!wallJumped) {
        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
      } else {
        rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
      }
    }

    IEnumerator DisableMovement(float time) {
      canMove = false;
      yield return new WaitForSeconds(time);
      canMove = true;
    }

    // ------------------------------------------------------------------------------
    // Ground checks

    void UpdateGroundTouch() {
      // did we just landed on ground?
      bool isGrounded = coll.onGround || coll.onWall;

      if (isGrounded && !groundTouch) {
        GroundTouch();
        groundTouch = true;
      }

      if (!isGrounded && groundTouch) {
        groundTouch = false;
      }

      // if on ground we can jump
      if (coll.onGround && !isDashing) {
        wallJumped = false;
        isBetterJumpEnabled = true;
      }

      void GroundTouch() {
        hasDashed = false;
        isDashing = false;
        side = anim.sr.flipX ? -1 : 1;
        jumpParticle.Play();
        sounds.PlayFootstep();
      }
    }

    // ------------------------------------------------------------------------------
    // Jump

    void UpdateJump() {
      // Better Jump gravity
      if (isBetterJumpEnabled) {
        if (rb.velocity.y < 0) {
          rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if (rb.velocity.y > 0 && !isJumpBeingPressed) {
          rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
      }
    }

    public void Jump(Vector2 dir, bool wall) {
      if (!canJump) return;

      // multi-jump
      if (coll.onGround || coll.onWall) jumpsAvailable = maxJumps;
      if (jumpsAvailable == 0) return;
      jumpsAvailable -= 1;

      anim.SetTrigger("jump");
      sounds.PlayJump();

      slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
      ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

      rb.velocity = new Vector2(rb.velocity.x, 0);
      rb.velocity += dir * jumpForce;

      particle.Play();
    }

    public void WallJump() {
      if (!canJump) return;

      if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall) {
        side *= -1;
        anim.Flip(side);
      }

      StartCoroutine(DisableMovement(.1f));

      Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

      Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

      wallJumped = true;
    }

    // ------------------------------------------------------------------------------
    // Walls

    void UpdateWalls(float x, float y) {
      // wall grab flags

      if (coll.onWall && isGrabBeingPressed && canMove && canWallGrab) {
        if (side != coll.wallSide) anim.Flip(side * -1);
        wallGrab = true;
        wallSlide = false;
      }

      if (!isGrabBeingPressed || !coll.onWall || !canMove || !canWallGrab) {
        wallGrab = false;
        wallSlide = false;
      }

      // wall grab

      if (wallGrab && !isDashing) {
        rb.gravityScale = 0;
        if (x > .2f || x < -.2f) {
          rb.velocity = new Vector2(rb.velocity.x, 0);
        }

        float speedModifier = y > 0 ? .5f : 1;
        rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
      } else {
        rb.gravityScale = 3;
      }

      // wall slide

      if (coll.onWall && !coll.onGround && canWallSlide) {
        if (x != 0 && !wallGrab && rb.velocity.y < 0) {
          wallSlide = true;
          WallSlide();
        }
      }

      if (!coll.onWall || coll.onGround || !canWallSlide) {
        wallSlide = false;
      }

      WallParticle(y);
    }

    private void WallSlide() {
      if (!canMove) return;
      if (!canWallSlide) return;

      if (coll.wallSide != side) anim.Flip(side * -1);

      bool pushingWall = false;
      if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall)) {
        pushingWall = true;
      }
      float push = pushingWall ? 0 : rb.velocity.x;

      rb.velocity = new Vector2(push, -slideSpeed);

      sounds.PlaySlide();
    }

    void WallParticle(float vertical) {
      var main = slideParticle.main;

      if (wallSlide || (wallGrab && vertical < 0)) {
        slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        main.startColor = Color.white;
      } else {
        main.startColor = Color.clear;
      }
    }

    int ParticleSide() {
      int particleSide = coll.onRightWall ? 1 : -1;
      return particleSide;
    }

    // ------------------------------------------------------------------------------
    // Char Side

    void UpdateCharSide(float x) {
      // escape if on walls or cannot move for some reason
      if (wallGrab || wallSlide || !canMove)
        return;

      // turn right
      if (x > 0) {
        side = 1;
        anim.Flip(side);
      }

      // turn left
      if (x < 0) {
        side = -1;
        anim.Flip(side);
      }
    }

    // ------------------------------------------------------------------------------
    // Dash

    public void Dash() {
      if (!canDash) return;
      if (hasDashed) return;
      if (xRaw == 0 && yRaw == 0) return;

      float x = xRaw;
      float y = yRaw;

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
      isBetterJumpEnabled = false;
      wallJumped = true;
      isDashing = true;

      sounds.PlayDash();

      yield return new WaitForSeconds(.3f);

      dashParticle.Stop();
      rb.gravityScale = 3;
      isBetterJumpEnabled = true;
      wallJumped = false;
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
      if (!canAttack) return;
      if (isAttacking) return;
      if (wallGrab) return;
      if (wallSlide) return;

      StartCoroutine(AttackSeq());
    }

    IEnumerator AttackSeq() {
      anim.SetTrigger("attack");
      sounds.PlayAttack();
      isAttacking = true;

      yield return new WaitForSeconds(attackSpeed);

      // todo: We may want to switch to CircleCastAll, it will give you contact points in world space coordinates
      // todo: https://docs.unity3d.com/ScriptReference/Physics2D.CircleCastAll.html

      int side = anim.sr.flipX ? -1 : 1;

      Vector2 pos = (Vector2)transform.position + new Vector2(attackRect.x, attackRect.y) * Vector2.right * side;
      Collider2D[] enemiesToDamage = Physics2D.OverlapBoxAll(
        pos, new Vector2(attackRect.width, attackRect.height), 0, attackLayer
      );

      for (int i = 0; i < enemiesToDamage.Length; i++) {
        if (enemiesToDamage[i].transform.root == transform) continue;
        Movement enemy = enemiesToDamage[i].GetComponent<Movement>();
        if (!enemy.isDead) StartCoroutine(enemy.TakeDamage(attackDamage, this));
      }

      yield return new WaitForSeconds(attackCooldown);

      isAttacking = false;
    }

    void OnDrawGizmos() {
      if (!isAttacking) return;
      Gizmos.color = Color.red;
      int side = anim.sr.flipX ? -1 : 1;
      Vector2 pos = (Vector2)transform.position + new Vector2(attackRect.x, attackRect.y) * Vector2.right * side;
      Gizmos.DrawWireCube(pos, new Vector2(attackRect.width, attackRect.height));
    }

    // ------------------------------------------------------------------------------
    // Block

    public void Block() {
      if (!canBlock) return;
      if (wallGrab) return;
      if (wallSlide) return;

      isBlocking = true;
      sounds.PlayBlock();
    }

    public void Unblock() {
      if (!canBlock) return;
      isBlocking = false;
      sounds.PlayBlock();
    }

    // ------------------------------------------------------------------------------
    // Damage

    public IEnumerator TakeDamage(int damage, Movement attacker) {
      anim.SetTrigger("damage");
      sounds.PlayDamage();
      SpawnBlood(damage);

      health -= damage;
      Knockback(attacker);

      if (health > 0) {
        // stop movement wile taking damage (dazed)
        canMove = false;
        yield return new WaitForSeconds(dazedDuration);
        canMove = true;
      } else {
        StartCoroutine(Die());
      }
    }

    public void Knockback(Movement attacker) {
      // float knockbackForce = 10f;
      Vector2 dir = (transform.position - attacker.transform.position).normalized;
      rb.velocity += dir * knockbackForce;
      // rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);

      // progressively dump rigidbody drag
      // StartCoroutine(LerpRigidbodyDrag(14, 0, 0.8f));
    }

    public IEnumerator Die() {
      canMove = false;
      isDead = true;
      health = 0;

      anim.SetTrigger("die");
      sounds.PlayDie();

      yield return new WaitForSeconds(dazedDuration);

      GetComponent<Collider2D>().enabled = false;
      rb.constraints = RigidbodyConstraints2D.FreezeAll;


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
