using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// todo: Create generic ropes that can be placed on maps with different lengths:

// todo: player should be able to 'stick' to a rope when touching it,
// todo: player should be able to climb up and down the rope
// todo: player should be able to swing the rope left and right

namespace Carles.Engine2D {

  public class Rope : MonoBehaviour {

    public float throwSpeed = 0.1f;
    public float nodeDistance = 0.5f;

    [Space]
    [Header("Prefabs")]
    public GameObject nodePrefab;
    public GameObject player;
    private CharController2D c;
    public GameObject lastNode;
    public LineRenderer lr;

    int vertexCount = 2;
    public List<GameObject> Nodes = new List<GameObject>();

    void Awake() {
      player = GameObject.FindGameObjectWithTag("Player");
      c = player.GetComponent<CharController2D>();
      lr = GetComponent<LineRenderer>();
    }

    public void Init(Vector2 destiny) {
      StartCoroutine(ThrowRope(destiny));
    }

    void Update() {
      // render a line though all the nodes
      RenderLine();
    }

    public IEnumerator ThrowRope(Vector2 destiny, float speed = 0.1f) {
      // Debug.Log("ThrowRope");

      Nodes.Add(transform.gameObject);
      lastNode = transform.gameObject;

      while (true) {
        // make the rope head move towards the destiny point
        transform.position = Vector2.MoveTowards(transform.position, destiny, speed);

        if ((Vector2)transform.position != destiny) {
          // If head has not reach the destiny point yet
          // create a node if last node's distance to the player is too much
          if (Vector2.Distance(player.transform.position, lastNode.transform.position) > nodeDistance) {
            CreateNode();
          }

        } else {
          // when rope head reaches the destiny point
          // connect the lastNode to the player's rigidbody
          lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
          yield break;
        }

        yield return null;
      }
    }

    void RenderLine() {
      lr.positionCount = vertexCount;

      // adjust line to each node
      for (int i = 0; i < Nodes.Count; i++) {
        lr.SetPosition(i, Nodes[i].transform.position);
      }

      // adjust line last segment to player's position
      lr.SetPosition(Nodes.Count, player.transform.position);
    }


    void CreateNode() {

      Vector2 pos2Create = player.transform.position - lastNode.transform.position;
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

    // public IEnumerator UnsetRope() {
    //   int c = 0;
    //   while (true) {


    //     if (Nodes.Count == 0) {
    //       isWorking = false;
    //       yield break;
    //     }

    //     // make the rope head move towards the pl
    //     transform.position = Vector2.MoveTowards(transform.position, player.transform.position, throwSpeed);

    //     if ((Vector2)transform.position != (Vector2)player.transform.position) {
    //       // If head has not reach the player point yet
    //       // remove a node if last node's distance to the player is too much
    //       if (Vector2.Distance(player.transform.position, lastNode.transform.position) > nodeDistance) {
    //         DestroyNode(lastNode);
    //         lastNode = Nodes[Nodes.Count - 1];
    //       }

    //     } else {
    //       // when rope head reaches the player point
    //       // unconnect the lastNode to the player's rigidbody
    //       lastNode.GetComponent<HingeJoint2D>().connectedBody = null; // player.GetComponent<Rigidbody2D>();

    //       isWorking = false;
    //       yield break;
    //     }

    //     // just in case
    //     c += 1;
    //     if (c > 200) {
    //       isWorking = false;
    //       yield break;
    //     }

    //     yield return null;
    //     // yield return new WaitForSeconds(0.1f);
    //   }
    // }

    // void DestroyNode(GameObject node) {
    //   Nodes.Remove(node);
    //   Destroy(node);
    //   vertexCount--;
    // }

  }
}
