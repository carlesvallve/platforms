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
    private Vector2 currentDestiny;

    void Start() {
      c = GetComponent<CharController2D>();
    }

    void Update() {
      if (!isHookActive) return;
      UpdateRopePlayerState();
      UpdateRopeSwinging();
      UpdateRopeClimbing();
    }

    private void UpdateRopePlayerState() {
      if (c.coll.onGround || c.coll.onWall) {
        // while being on floor
        c.move.canMove = true;
        c.rb.freezeRotation = true;
        c.rb.angularDrag = originalDrag;
      } else {
        // while being in the air
        c.move.canMove = false;
        c.rb.freezeRotation = false;
        c.rb.angularDrag = swingDrag;
      }
    }

    private void UpdateRopeSwinging() {
      // swing player left and right
      float x = c.move.xRaw;
      c.rb.AddForce(new Vector2(x * swingForce, 0));
    }

    private void UpdateRopeClimbing() {
      float y = c.move.yRaw;
      if (Mathf.Abs(y) < 1f) return;

      c.rb.freezeRotation = true;
      c.rb.angularDrag = originalDrag;

      if (y > 0) {
        // climbing up
        Vector2 vec = (currentDestiny - (Vector2)c.transform.position).normalized;
        c.rb.velocity = vec * 8f;
        c.rb.velocity = new Vector2(c.rb.velocity.x * 0.25f, c.rb.velocity.y);
      } else {
        // climbing down
        c.rb.velocity = new Vector2(c.rb.velocity.x, y * 8f);
      }

      // escape if at max rope length, so we dont break it
      float ropeLength = Vector2.Distance(c.transform.position, currentDestiny);
      if (ropeLength >= maxLength) {
        return;
      }

      // re-create the rope instantly
      EndHook();
      StartCoroutine(CreateRope(c.transform.position, currentDestiny, true));
    }


    public void StartHook() {
      // get hook direction
      Vector2 origin = c.transform.position;
      Vector2 dir = new Vector2(c.move.xRaw, 1.5f).normalized;

      Vector2 destiny = origin + dir * maxLength;

      // cast a ray in direction, and get first hit contact point
      RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, maxLength, collisionLayers);
      if (hit.collider != null) destiny = hit.point;

      StartCoroutine(CreateRope(origin, destiny, false));
    }

    IEnumerator CreateRope(Vector2 origin, Vector2 destiny, bool instant) {
      currentDestiny = destiny;

      // if rope didnt hit a wall, destroy it and escape, before creating a new rope
      float ropeLength = Vector2.Distance(origin, destiny);
      if (ropeLength >= maxLength) {
        EndHook();
        yield break;
      }

      // instantiate the rope
      GameObject go = (GameObject)Instantiate(ropePrefab, c.transform.position, Quaternion.identity);
      rope = go.GetComponent<Rope>();

      // throw the rope
      if (instant) {
        rope.ThrowRopeInstant(destiny);
      } else {
        yield return rope.StartCoroutine(rope.ThrowRope(destiny));
      }

      // remember ridigBody settings
      originalDrag = c.rb.angularDrag;

      // when we are on a rope, we can always jump just once
      c.jump.SetJumpsAvailable(1);

      // activate hooking state
      isHookActive = true;
    }

    public void EndHook() {
      // Debug.Log("EndHook");

      // delete hook rope
      if (rope && rope.gameObject) {
        Destroy(rope.gameObject);

        // restore rigidbody settings
        c.rb.angularDrag = originalDrag;

        // deactivate hooking state
        isHookActive = false;
      }
    }

  }
}
