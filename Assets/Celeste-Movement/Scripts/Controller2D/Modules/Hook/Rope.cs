using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Carles.Engine2D {

  public class Rope : MonoBehaviour {

    public float throwSpeed = 0.1f;
    public float nodeDistance = 0.5f;

    private Vector2 destiny;

    [Space]
    [Header("Prefabs")]
    public GameObject nodePrefab;
    public GameObject player;
    public GameObject lastNode;
    public LineRenderer lr;

    int vertexCount = 2;
    public List<GameObject> Nodes = new List<GameObject>();

    private bool done = false;

    void Start() {
      player = GameObject.FindGameObjectWithTag("Player");
      lr = GetComponent<LineRenderer>();

      Nodes.Add(transform.gameObject);
      lastNode = transform.gameObject;

      done = false;
    }

    public void Init(Vector2 _destiny) {
      destiny = _destiny;
    }


    void Update() {
      // make the rope head move towards the destiny point
      transform.position = Vector2.MoveTowards(transform.position, destiny, throwSpeed);

      // If head is not there yet
      if ((Vector2)transform.position != destiny) {

        // create a node if last node distance to the player is too much
        if (Vector2.Distance(player.transform.position, lastNode.transform.position) > nodeDistance) {
          CreateNode();
        }

      } else if (done == false) {
        // if this is the first time rope head reaches the destiny point
        // create a node if last node distance to the player is too much
        while (Vector2.Distance(player.transform.position, lastNode.transform.position) > nodeDistance) {
          CreateNode();
        }

        // connect the lastNode to the player's rigidbody
        lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
        done = true;
      }

      // render a line though all the nodes
      RenderLine();
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

  }
}
