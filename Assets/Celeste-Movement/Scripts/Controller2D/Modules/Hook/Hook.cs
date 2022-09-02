using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class Hook : MonoBehaviour {

    public GameObject ropePrefab;
    public LayerMask collisionLayers;
    public float maxLength = 5f;
    public float swingForce = 1f;

    [Space] // debug
    public bool isHookActive;

    private CharController2D c;
    private HookRope hookRope;
    private Vector2 currentDestiny;

    [Space]
    [Header("Rope")]
    public Rope currentRope;
    public int currentNodeIndex;

    void Start() {
      c = GetComponent<CharController2D>();
    }

    void Update() {
      if (!isHookActive) return;
      UpdateRopePlayerState();
      Swing();
      HookSlide();

      // float y = c.move.yRaw;
      // if (Mathf.Abs(y) < 1f) return;
      // int dir = y > 0 ? 1 : -1;
      // Slide(dir);
    }

    private void UpdateRopePlayerState() {
      if (c.coll.onGround || c.coll.onWall) {
        // while being on floor
        c.move.canMove = true;
        c.rb.freezeRotation = true;
      } else {
        // while being in the air
        c.move.canMove = false;
        c.rb.freezeRotation = false;
      }
    }

    private void Swing() {
      // swing player left and right
      float x = c.move.xRaw;
      c.rb.AddForce(new Vector2(x * swingForce, 0));
    }

    public void RopeSlide(int dir) {
      // float y = c.move.yRaw;
      // if (Mathf.Abs(y) < 1f) return;
      // int dir = (int)y;

      if (dir == 0) return;

      // get next node index in direction
      int nextIndex = currentNodeIndex - dir;
      if (nextIndex < 1) nextIndex = 1;
      if (nextIndex > currentRope.Nodes.Count - 1) nextIndex = currentRope.Nodes.Count - 1;
      // Debug.Log(currentNodeIndex + " + " + (-dir) + " = " + nextIndex);

      // attach character to new node
      RopeNode nextNode = currentRope.Nodes[nextIndex];
      currentRope.AttachCharacter(c, nextNode);

      // update current node index
      currentNodeIndex = nextIndex;
    }

    private void HookSlide() {
      if (currentRope) return;

      float y = c.move.yRaw;
      if (Mathf.Abs(y) < 1f) return;

      c.rb.freezeRotation = true;

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
      StartCoroutine(CreateHookRope(c.transform.position, currentDestiny, true));
    }


    public void StartHook() {
      // get hook direction
      Vector2 origin = c.transform.position;
      Vector2 dir = new Vector2(c.move.xRaw, 1.5f).normalized;

      Vector2 destiny = origin + dir * maxLength;

      // cast a ray in direction, and get first hit contact point
      RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, maxLength, collisionLayers);
      if (hit.collider != null) destiny = hit.point;

      StartCoroutine(CreateHookRope(origin, destiny, false));
    }

    IEnumerator CreateHookRope(Vector2 origin, Vector2 destiny, bool instant) {
      currentDestiny = destiny;

      // if rope didnt hit a wall, destroy it and escape, before creating a new rope
      float ropeLength = Vector2.Distance(origin, destiny);
      if (ropeLength >= maxLength) {
        EndHook();
        yield break;
      }

      // instantiate the rope
      GameObject go = (GameObject)Instantiate(ropePrefab, c.transform.position, Quaternion.identity);
      hookRope = go.GetComponent<HookRope>();

      // throw the rope
      if (instant) {
        hookRope.ThrowRopeInstant(destiny);
      } else {
        yield return hookRope.StartCoroutine(hookRope.ThrowRope(destiny));
      }

      // when we are on a rope, reset jumps
      c.jump.SetJumpsAvailable(c.jump.maxJumps);

      // if starting hood from a rope, end the current rope
      if (currentRope) EndRope();

      // activate hooking state
      isHookActive = true;


    }

    public void EndHook() {
      // delete hook rope
      if (hookRope && hookRope.gameObject) {
        Destroy(hookRope.gameObject);

        // deactivate hooking state
        isHookActive = false;

        currentRope = null;
      }
    }

    public void EndRope() {
      // deactivate hooking state
      isHookActive = false;

      if (currentRope) currentRope.DetachCharacter(c);
    }

  }
}
