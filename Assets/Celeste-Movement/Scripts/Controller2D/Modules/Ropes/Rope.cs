﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Carles.Engine2D {

  public class Rope : MonoBehaviour {

    public Color color = Color.white;
    public GameObject nodePrefab;
    public int ropeLength = 7;
    public float nodeDistance = 0.5f;
    public bool displayNodes = true;
    public List<RopeNode> Nodes = new List<RopeNode>();
    public CharController2D attachedCharacter;

    private LineRenderer lineRenderer;
    private int vertexCount = 2;

    void Awake() {
      lineRenderer = transform.GetComponent<LineRenderer>();
      lineRenderer.startColor = color;
      lineRenderer.endColor = color;
      lineRenderer.startWidth = 0.08f;
      lineRenderer.endWidth = 0.08f;

      GenerateRope();
    }

    void OnDrawGizmos() {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireCube(
        (Vector2)transform.position - Vector2.up * (ropeLength * nodeDistance * 0.5f),
        new Vector2(0.1f, ropeLength * nodeDistance)
      );
    }

    void GenerateRope() {
      // first node is the rope root
      RopeNode lastNode = transform.GetComponent<RopeNode>();
      Nodes.Add(lastNode);

      for (int i = 0; i < ropeLength; i++) {
        Vector2 pos = (Vector2)lastNode.transform.position - Vector2.up * nodeDistance;
        lastNode = CreateNode(lastNode, pos);
      }

      // disable last node, if not, it will be anchored in space
      lastNode.GetComponent<HingeJoint2D>().enabled = false;
    }

    void Update() {
      RenderLine();
    }

    void RenderLine() {
      lineRenderer.positionCount = vertexCount - 1;

      // adjust line to each node
      for (int i = 0; i < Nodes.Count; i++) {
        lineRenderer.SetPosition(i, Nodes[i].transform.position);
      }
    }

    private RopeNode CreateNode(RopeNode lastNode, Vector2 pos) {
      GameObject go = (GameObject)Instantiate(nodePrefab, pos, Quaternion.identity);
      go.transform.SetParent(transform);

      SpriteRenderer sprite = go.GetComponent<SpriteRenderer>();
      sprite.color = color; //sharedMaterial.SetColor("_Color", color);
      sprite.enabled = displayNodes;

      RopeNode ropeNode = go.GetComponent<RopeNode>();
      ropeNode.index = Nodes.Count;

      lastNode.GetComponent<HingeJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();
      RopeNode newNode = go.GetComponent<RopeNode>();

      Nodes.Add(newNode);
      vertexCount++;

      return newNode;
    }

    public IEnumerator WaitToResetRope() {
      yield return new WaitForSeconds(0.15f);

      if (attachedCharacter) {
        attachedCharacter = null;
      }
    }

  }
}
