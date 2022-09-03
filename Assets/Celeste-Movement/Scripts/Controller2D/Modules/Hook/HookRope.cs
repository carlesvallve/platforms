using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Carles.Engine2D {

  public class HookRope : MonoBehaviour {

    public float ropeLength = 5f;
    public float nodeDistance = 0.5f;

    [Space]
    [Header("Prefabs")]
    public GameObject nodePrefab;
    public GameObject lastNode;
    public List<GameObject> Nodes = new List<GameObject>();

    // public GameObject player;
    private CharController2D c;
    private LineRenderer lr;
    private int vertexCount = 2;

    public void Init(CharController2D _c) {
      c = _c;
      lr = GetComponent<LineRenderer>();
    }

    void Update() {
      RenderLine();
    }

    public void ThrowRopeInstant(Vector2 destiny, float speed = 0.1f) {
      Nodes.Add(transform.gameObject);
      lastNode = transform.gameObject;

      while (UpdateNodes(destiny, speed)) {
        RenderLine();
      }
    }

    public IEnumerator ThrowRope(Vector2 destiny, float speed = 0.1f) {
      Nodes.Add(transform.gameObject);
      lastNode = transform.gameObject;

      while (UpdateNodes(destiny, speed)) {
        RenderLine();
        yield return null;
      }
    }

    private bool UpdateNodes(Vector2 destiny, float speed) {
      // make the rope head move towards the destiny point
      transform.position = Vector2.MoveTowards(transform.position, destiny, speed);

      if ((Vector2)transform.position != destiny) {
        // If head has not reach the destiny point yet
        // create a node if last node's distance to the player is too much
        if (Vector2.Distance(c.transform.position, lastNode.transform.position) > nodeDistance) {
          CreateNode();
        }

      } else {
        // when rope head reaches the destiny point
        // connect the lastNode to the player's rigidbody
        lastNode.GetComponent<HingeJoint2D>().connectedBody = c.GetComponent<Rigidbody2D>();
        return false;
      }

      return true;
    }

    void RenderLine() {
      lr.positionCount = vertexCount;

      // adjust line to each node
      for (int i = 0; i < Nodes.Count; i++) {
        lr.SetPosition(i, Nodes[i].transform.position);
      }

      // adjust line last segment to player's position
      lr.SetPosition(Nodes.Count, c.transform.position);
    }


    void CreateNode() {
      Vector2 pos2Create = c.transform.position - lastNode.transform.position;
      pos2Create.Normalize();
      pos2Create *= nodeDistance;
      pos2Create += (Vector2)lastNode.transform.position;

      GameObject go = (GameObject)Instantiate(nodePrefab, pos2Create, Quaternion.identity);
      go.transform.SetParent(transform);

      lastNode.GetComponent<HingeJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();
      lastNode = go;

      Nodes.Add(lastNode);
      vertexCount++;
    }

  }
}
