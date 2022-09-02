using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class Roping : MonoBehaviour {

    private CharController2D c;

    private HingeJoint2D hj;

    public float swingForce = 10f;

    public bool attached = false;
    public Transform attachedTo; // which rope the player is attached to
    private GameObject disregard; // disregard the rope that we are currently attached to so that it doesnt re-attach

    public GameObject pulleySelected = null;

    public GameObject newSeg;

    void Awake() {
      c = GetComponent<CharController2D>();
      hj = gameObject.GetComponent<HingeJoint2D>();
    }

    void Update() {
      // c.hook.isHookActive = attached;

      if (attached) {
        UpdateRopePlayerState();
        // Swing();
        // Slide();
      }


    }

    private void UpdateRopePlayerState() {
      if (c.coll.onGround || c.coll.onWall) {
        // while being on floor
        c.move.canMove = true;
        // c.rb.freezeRotation = true;
        // c.rb.angularDrag = 0.05f; // originalDrag;
        c.rb.gravityScale = 3f;
      } else {
        // while being in the air
        c.move.canMove = false;
        // c.rb.freezeRotation = false;
        // c.rb.angularDrag = 3f; // swingDrag;
        c.rb.gravityScale = 0;
      }
    }

    private void Attach(Rigidbody2D ropeBone) {
      ropeBone.gameObject.GetComponent<RopeSegment>().isPlayerAttached = true;
      hj.connectedBody = ropeBone;
      hj.enabled = true;
      attached = true;
      attachedTo = ropeBone.gameObject.transform.parent;

      // Rope2 rope = ropeBone.GetComponentInParent<Rope2>();
      // rope.EnableTriggerMode(false);
    }

    public void Detach() {
      hj.connectedBody.gameObject.GetComponent<RopeSegment>().isPlayerAttached = false;
      attached = false;
      hj.enabled = false;
      hj.connectedBody = null;

      // Rope2 rope = hj.connectedBody.GetComponentInParent<Rope2>();
      // rope.EnableTriggerMode(true);
    }

    private void Swing() {
      // swing player left and right
      float x = c.move.xRaw;
      c.rb.AddForce(new Vector2(x * swingForce, 0));
    }

    public void Slide(int direction) {
      // float y = c.move.yRaw;
      // if (y == 0) return;
      // float direction = y; // > 0 ? 1 : -1;

      RopeSegment myConnection = hj.connectedBody.gameObject.GetComponent<RopeSegment>();
      // GameObject newSeg = null;
      newSeg = null;
      if (direction > 0) {
        // up
        Debug.Log("Up " + myConnection + " " + myConnection.connectedAbove);
        if (myConnection.connectedAbove != null) {
          if (myConnection.connectedAbove.gameObject.GetComponent<RopeSegment>() != null) {
            newSeg = myConnection.connectedAbove;
          }
        }
      } else {
        // down
        Debug.Log("Down " + myConnection + " " + myConnection.connectedBelow);
        if (myConnection.connectedBelow != null) {
          newSeg = myConnection.connectedBelow;
        }
      }

      if (newSeg != null) {
        transform.position = newSeg.transform.position;
        myConnection.isPlayerAttached = false;
        newSeg.GetComponent<RopeSegment>().isPlayerAttached = true;
        hj.connectedBody = newSeg.GetComponent<Rigidbody2D>();
      }
    }

    void OnTriggerEnter2D(Collider2D col) {
      // Debug.Log()
      if (!attached) {
        if (col.gameObject.tag == "Rope") {
          if (attachedTo != col.gameObject.transform.parent) {
            if (disregard != null || col.gameObject.transform.parent.gameObject != disregard) {
              Attach(col.gameObject.GetComponent<Rigidbody2D>());
            }
          }
        }
      }
    }

  }
}
