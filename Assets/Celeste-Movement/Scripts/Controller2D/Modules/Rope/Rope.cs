﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// todo: Create generic ropes that can be placed on maps with different lengths:
// todo: player should be able to 'stick' to a rope when touching it,
// todo: player should be able to climb up and down the rope
// todo: player should be able to swing the rope left and right

namespace Carles.Engine2D {

  public class Rope : MonoBehaviour {

    public float ropeLength = 5f;
    public float nodeDistance = 0.5f;
    public float throwSpeed = 0.1f;

    [Space]
    [Header("Prefabs")]
    public GameObject nodePrefab;
    public GameObject lastNode;
    public LineRenderer lr;

    int vertexCount = 2;
    public List<GameObject> Nodes = new List<GameObject>();

    private Vector2 origin;

    void Awake() {
      transform.position = new Vector2(transform.position.x, transform.position.y - ropeLength);
      origin = transform.position;

      lr = GetComponent<LineRenderer>();

      Vector2 destiny = new Vector2(transform.position.x, transform.position.y + ropeLength);
      ThrowRopeInstant(destiny, 0.5f);
    }

    void OnDrawGizmos() {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireCube(
        (Vector2)transform.position - Vector2.up * ropeLength * 0.5f,
        new Vector2(0.1f, ropeLength)
      );
    }

    void Update() {
      RenderLine(); // render a line though all the nodes
    }

    public void ThrowRopeInstant(Vector2 destiny, float speed = 0.1f) {
      Nodes.Add(transform.gameObject);
      lastNode = transform.gameObject;

      while (UpdateNodes(destiny, speed)) {
        // RenderLine();
      }
    }

    // public IEnumerator ThrowRope(Vector2 destiny, float speed = 0.1f) {
    //   Nodes.Add(transform.gameObject);
    //   lastNode = transform.gameObject;

    //   while (UpdateNodes(destiny, speed)) {
    //     RenderLine();
    //     yield return null;
    //   }
    // }

    private bool UpdateNodes(Vector2 destiny, float speed) {
      // make the rope head move towards the destiny point
      transform.position = Vector2.MoveTowards(transform.position, destiny, speed);

      // If head has not reach the destiny point yet
      // create a node if last node's distance to the player is too much
      if (Vector2.Distance(origin, lastNode.transform.position) > nodeDistance) {
        CreateNode();
      }

      if ((Vector2)transform.position == destiny) {
        lastNode.GetComponent<HingeJoint2D>().enabled = false; // connectedBody = lastNode.GetComponent<Rigidbody2D>();
        return false;
      }

      // if ((Vector2)transform.position != destiny) {
      //   // // If head has not reach the destiny point yet
      //   // // create a node if last node's distance to the player is too much
      //   // if (Vector2.Distance(origin, lastNode.transform.position) > nodeDistance) {
      //   //   CreateNode();
      //   // }

      // } else {
      //   // when rope head reaches the destiny point
      //   // connect the lastNode to the player's rigidbody
      //   // todo
      //   // lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
      //   return false;
      // }

      return true;
    }

    void RenderLine() {
      lr.positionCount = vertexCount - 1;

      // adjust line to each node
      for (int i = 0; i < Nodes.Count; i++) {
        lr.SetPosition(i, Nodes[i].transform.position);
      }

      // adjust line last segment to player's position
      // lr.SetPosition(Nodes.Count, origin);
    }


    void CreateNode() {
      Vector2 pos2Create = origin - (Vector2)lastNode.transform.position;
      pos2Create.Normalize();
      pos2Create *= nodeDistance;
      pos2Create += (Vector2)lastNode.transform.position;

      GameObject go = (GameObject)Instantiate(nodePrefab, pos2Create, Quaternion.identity);
      go.transform.SetParent(transform);

      // Text label = go.GetComponent<Text>();
      // label.text = vertexCount.ToString();

      lastNode.GetComponent<HingeJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();
      lastNode = go;

      Nodes.Add(lastNode);
      vertexCount++;
    }

  }
}
