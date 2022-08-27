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

    public void Init(CharController2D _shooter, int _side) {
      shooter = _shooter;
      side = _side;

      rb = GetComponent<Rigidbody2D>();
      rb.AddForce(new Vector2(side * speed, Random.Range(variationBottom, variationTop)), ForceMode2D.Impulse);
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

      transform.Translate(rb.velocity.normalized * side * penetration);

      rb.velocity = Vector2.zero;
      transform.SetParent(collision.transform);
      rb.constraints = RigidbodyConstraints2D.FreezeAll;
      rb.simulated = false;

      CharController2D enemy = collision.GetComponent<CharController2D>();
      // Debug.Log(collision.tag + " " + collision.transform + " " + enemy);

      if (enemy) {
        enemy.StartCoroutine(enemy.TakeDamage(shooter, damage, 3f));
      }

      Destroy(gameObject, Random.Range(1f, 3f));
    }
  }
}