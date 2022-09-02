using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Carles.Engine2D {

  public class Rope : MonoBehaviour {

    public float ropeLength = 5f;
    public float nodeDistance = 0.5f;
    public float throwSpeed = 0.1f;

    [Space]
    [Header("Prefabs")]
    public GameObject nodePrefab;
    public RopeNode lastNode;
    private LineRenderer lr;

    public CharController2D attachedCharacter;

    int vertexCount = 2;
    public List<RopeNode> Nodes = new List<RopeNode>();

    private Vector2 origin;

    void Awake() {
      transform.position = new Vector2(transform.position.x, transform.position.y - ropeLength);
      lr = GetComponent<LineRenderer>();

      origin = transform.position;
      Vector2 destiny = new Vector2(transform.position.x, transform.position.y + ropeLength);
      ThrowRopeInstant(destiny, 0.5f);
    }

    void OnDrawGizmos() {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireCube(
        (Vector2)transform.position - Vector2.up * (ropeLength - 0.5f) * 0.5f,
        new Vector2(0.1f, ropeLength - 0.5f)
      );
    }

    void Update() {
      RenderLine();

      // if (attachedCharacter) {
      //   CharController2D c = attachedCharacter;
      //   if (c.coll.onGround) {
      //     if (c.hook.currentRope) {
      //       attachedCharacter = null;
      //       c.hook.currentRope = null;
      //     }
      //   }
      // }
    }

    public void ThrowRopeInstant(Vector2 destiny, float speed = 0.1f) {
      lastNode = transform.GetComponent<RopeNode>();
      Nodes.Add(lastNode);

      while (UpdateNodes(destiny, speed)) {
        // RenderLine();
      }
    }

    private bool UpdateNodes(Vector2 destiny, float speed) {
      // make the rope head move towards the destiny point
      transform.position = Vector2.MoveTowards(transform.position, destiny, speed);

      // If head has not reach the destiny point yet
      // create a node if last node's distance to the player is too much
      if (Vector2.Distance(origin, lastNode.transform.position) > nodeDistance) {
        CreateNode();
      }

      if ((Vector2)transform.position == destiny) {
        lastNode.GetComponent<HingeJoint2D>().enabled = false;
        return false;
      }

      return true;
    }

    void RenderLine() {
      lr.positionCount = vertexCount - 1;

      // adjust line to each node
      for (int i = 0; i < Nodes.Count; i++) {
        lr.SetPosition(i, Nodes[i].transform.position);
      }
    }


    void CreateNode() {
      Vector2 pos2Create = origin - (Vector2)lastNode.transform.position;
      pos2Create.Normalize();
      pos2Create *= nodeDistance;
      pos2Create += (Vector2)lastNode.transform.position;

      GameObject go = (GameObject)Instantiate(nodePrefab, pos2Create, Quaternion.identity);
      go.transform.SetParent(transform);

      RopeNode ropeNode = go.GetComponent<RopeNode>();
      ropeNode.index = Nodes.Count; // - 1;

      lastNode.GetComponent<HingeJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();
      lastNode = go.GetComponent<RopeNode>();

      Nodes.Add(lastNode);
      vertexCount++;
    }

    public void AttachCharacter(CharController2D c, RopeNode node) {
      c.hook.EndHook(); // destroy hook if we are hooking

      // reset any previous rope props
      if (c.hook.currentRope) {
        c.hook.currentRope.attachedCharacter = null;
      }

      // set current rope props
      attachedCharacter = c;

      // set character props
      c.hook.isHookActive = true;
      c.hook.currentRope = this;
      c.hook.currentNodeIndex = node.index;

      // when we are on a rope, reset jumps // we can always jump just once
      c.jump.SetJumpsAvailable(c.jump.maxJumps);

      // attach character to rope joint
      HingeJoint2D hj = c.GetComponent<HingeJoint2D>();
      Rigidbody2D ropeBone = node.transform.GetComponent<Rigidbody2D>();
      hj.connectedBody = ropeBone;
      hj.enabled = true;

      // disable physics on rope vs character
      // Physics2D.IgnoreLayerCollision(
      //   LayerMask.NameToLayer("Player"),
      //   LayerMask.NameToLayer("Rope"),
      //   true
      // );
    }

    public void DetachCharacter(CharController2D c) {
      // reset and disable character joint right away
      HingeJoint2D hj = c.GetComponent<HingeJoint2D>();
      hj.connectedBody = null;
      hj.enabled = false;

      // enable physcis on rope vs character
      // Physics2D.IgnoreLayerCollision(
      //   LayerMask.NameToLayer("Player"),
      //   LayerMask.NameToLayer("Rope"),
      //   false
      // );

      // wait to completely reset the current rope 
      // so we can safely de-attach from it
      StartCoroutine(WaitToResetRope());
    }

    private IEnumerator WaitToResetRope() {
      yield return new WaitForSeconds(0.15f);

      if (attachedCharacter) {
        attachedCharacter.hook.currentRope = null;
        attachedCharacter = null;
      }
    }

  }
}
