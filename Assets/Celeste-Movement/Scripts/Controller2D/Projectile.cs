using UnityEngine;

namespace Carles.Engine2D {
  public class Projectile : MonoBehaviour {

    private Rigidbody2D rb;
    private CharController2D shooter;
    private int side;

    [SerializeField] private float speed = 4f;
    [SerializeField] private float variationTop = 0;
    [SerializeField] private float variationBottom = 0;
    [SerializeField] private float penetration = 0.15f;
    [SerializeField] private int damage = 1;

    void Awake() {
      rb = GetComponent<Rigidbody2D>();
    }

    public void Init(CharController2D _shooter, int _side) {
      shooter = _shooter;
      side = _side;

      // get arrow direction vector
      Vector2 dir = shooter.transform.rotation * Vector3.right * side;

      // place arrow on muzzle
      transform.position = new Vector3(
       shooter.transform.position.x + 0.3f * dir.x,
       shooter.transform.position.y + 0.1f * dir.y
      );

      // shoot arrow
      float diff = Random.Range(variationBottom, variationTop);
      rb.AddForce(new Vector2(dir.x * speed, dir.y + diff), ForceMode2D.Impulse);
    }

    void Update() {
      if (rb && rb.velocity.magnitude > 0) {
        Vector2 v = rb.velocity;
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
      }
    }

    public void OnTriggerEnter2D(Collider2D collision) {
      if (!rb) return;
      if (collision.transform == shooter.transform) return;
      if (collision.tag == "Projectile") return;
      if (collision.tag == "Player") return;

      transform.Translate(rb.velocity.normalized * side * penetration);

      rb.velocity = Vector2.zero;
      transform.SetParent(collision.transform);
      rb.constraints = RigidbodyConstraints2D.FreezeAll;
      rb.simulated = false;

      CharController2D enemy = collision.GetComponent<CharController2D>();
      // Debug.Log(collision.tag + " " + collision.transform + " " + enemy);

      if (enemy) {
        enemy.StartCoroutine(enemy.combat.TakeDamage(shooter, damage, 3f));
      }

      Destroy(gameObject, Random.Range(1f, 3f));
    }
  }
}