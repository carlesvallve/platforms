using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class RopeClimb : MonoBehaviour {
    public LayerMask collisionLayers;
    public Rope currentRope;
    public int currentNodeIndex;
    public float swingForce = 1.5f;
    public bool isActive;

    private CharController2D c;

    void Start() {
      c = GetComponent<CharController2D>();
    }

    void Update() {
      if (!isActive) return;

      if (c.coll.onGround || c.coll.onWall) {
        // while being on floor
        c.move.canMove = true;
        c.rb.freezeRotation = true;
      } else {
        // while being in the air
        c.move.canMove = false;
        c.rb.freezeRotation = false;
      }

      Swing();
      Slide();
    }

    private void Swing() {
      // swing player left and right
      float x = c.move.xRaw;
      c.rb.AddForce(new Vector2(x * swingForce, 0));
    }

    private void Slide() {

    }

    public void RopeSlide(int dir) {
    }

    // public void RopeSlide(int dir) {
    //   // float y = c.move.yRaw;
    //   // if (Mathf.Abs(y) < 1f) return;
    //   // int dir = (int)y;

    //   if (dir == 0) return;

    //   // get next node index in direction
    //   int nextIndex = currentNodeIndex - dir;
    //   if (nextIndex < 1) nextIndex = 1;
    //   if (nextIndex > currentRope.Nodes.Count - 1) nextIndex = currentRope.Nodes.Count - 1;
    //   // Debug.Log(currentNodeIndex + " + " + (-dir) + " = " + nextIndex);

    //   // attach character to new node
    //   RopeNode nextNode = currentRope.Nodes[nextIndex];
    //   currentRope.AttachCharacter(c, nextNode);

    //   // update current node index
    //   currentNodeIndex = nextIndex;
    // }

    public void StartRope(Rope rope, RopeNode node) {
      // reset any previous props
      if (currentRope) currentRope.attachedCharacter = null;
      if (c.hook.isActive) c.hook.EndHook();

      // set character props
      isActive = true;
      currentRope = rope;
      currentNodeIndex = node.index;
      c.jump.SetJumpsAvailable(c.jump.maxJumps);

      // attach character to rope joint
      HingeJoint2D hj = c.GetComponent<HingeJoint2D>();
      Rigidbody2D ropeBone = node.transform.GetComponent<Rigidbody2D>();
      hj.connectedBody = ropeBone;
      hj.enabled = true;

      // set new rope props
      currentRope.attachedCharacter = c;
    }

    public void EndRope() {
      // deactivate hooking state
      isActive = false;

      // reset and disable character joint right away
      HingeJoint2D hj = c.GetComponent<HingeJoint2D>();
      hj.connectedBody = null;
      hj.enabled = false;

      // wait to completely reset the current rope 
      // so we can safely de-attach from it
      currentRope.StartCoroutine(currentRope.WaitToResetRope());
    }

  }
}
