using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class Hook : MonoBehaviour {

    public GameObject ropePrefab;
    public LayerMask collisionLayers;
    public float maxLength = 5f;
    public float swingForce = 1f;
    public float swingDrag = 3f;

    [Space] // debug
    public bool isHookActive;

    private CharController2D c;
    private Rope rope;

    private float originalDrag = 0.05f;

    void Start() {
      c = GetComponent<CharController2D>();
    }

    void Update() {
      if (!isHookActive) return;

      // while hooking, player moves and rotate differently
      c.move.canMove = false;
      c.rb.freezeRotation = false;
      c.rb.angularDrag = swingDrag;

      // swing player left and right
      float x = c.move.xRaw;
      float y = c.move.yRaw;
      c.rb.AddForce(new Vector2(x * swingForce, 0));

      // move player up and down through the rope
      // c.rb.velocity = new Vector2(c.rb.velocity.x, y * 10f);
    }

    public void StartHook() {
      // Debug.Log("StartHook");

      // get hook direction
      Vector2 origin = c.transform.position;
      Vector2 dir = new Vector2(c.move.xRaw, 1.5f).normalized; // c.move.yRaw +

      Vector2 destiny = origin + dir * maxLength;

      // cast a ray in direction, and get first hit contact point
      RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, maxLength, collisionLayers);
      if (hit.collider != null) {
        destiny = hit.point;
      } else {
        EndHook();
        return;
      }

      float ropeLength = Vector2.Distance(origin, destiny);
      if (ropeLength >= maxLength) { // ropeLength < 2 || 
        EndHook();
        return;
      }

      // create hook rope
      GameObject go = (GameObject)Instantiate(ropePrefab, c.transform.position, Quaternion.identity);
      rope = go.GetComponent<Rope>();
      rope.Init(destiny);

      // record original ridigBody settings
      originalDrag = c.rb.angularDrag;

      // activate hooking state
      isHookActive = true;
    }

    public void EndHook() {
      // Debug.Log("EndHook");

      // delete hook rope
      if (rope && rope.gameObject) {
        Destroy(rope.gameObject);
      }

      // restore rigidbody settings
      c.rb.angularDrag = originalDrag;

      // deactivate hooking state
      isHookActive = false;
    }
  }
}
