using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {
  public class Rope2 : MonoBehaviour {

    public Rigidbody2D hook;
    public HingeJoint2D top;
    public GameObject[] prefabRopeSegs;
    public int numLinks = 5;

    public CharController2D c;

    void Start() {
      c = GameObject.FindGameObjectWithTag("Player").GetComponent<CharController2D>();
      GenerateRope();
      // EnableTriggerMode(true);
    }

    // public void EnableTriggerMode(bool value) {
    //   RopeSegment[] ropeSegs = GetComponentsInChildren<RopeSegment>();
    //   for (int i = 0; i < ropeSegs.Length; i++) {

    //     BoxCollider2D col = ropeSegs[i].GetComponent<BoxCollider2D>();
    //     Debug.Log(i + " - Changing trigger of " + col + " to " + value);
    //     col.isTrigger = value;
    //   }
    // }

    void GenerateRope() {
      Rigidbody2D prevBody = hook;

      for (int i = 0; i < numLinks; i++) {
        int index = Random.Range(0, prefabRopeSegs.Length);
        GameObject newSeg = Instantiate(prefabRopeSegs[index]);
        newSeg.transform.parent = transform;
        newSeg.transform.position = (Vector2)transform.position; // - Vector2.up * 0.5f * i;  // transform.position
        HingeJoint2D hj = newSeg.GetComponent<HingeJoint2D>();
        hj.connectedBody = prevBody;

        prevBody = newSeg.GetComponent<Rigidbody2D>();

        if (i == 0) top = hj;
      }
    }

    public void AddLink() {
      // Debug.Log("AddLink");

      int index = Random.Range(0, prefabRopeSegs.Length);
      GameObject newLink = Instantiate(prefabRopeSegs[index]);
      newLink.transform.parent = transform;
      newLink.transform.position = transform.position; // (Vector2)transform.position - Vector2.up * 0.5f * i;
      HingeJoint2D hj = newLink.GetComponent<HingeJoint2D>();
      hj.connectedBody = hook;

      newLink.GetComponent<RopeSegment>().connectedBelow = top.gameObject;
      top.connectedBody = newLink.GetComponent<Rigidbody2D>();
      top.GetComponent<RopeSegment>().ResetAnchor();
      top = hj;
    }

    public void RemoveLink() {
      // Debug.Log("RemoveLink");

      if (top.gameObject.GetComponent<RopeSegment>().isPlayerAttached) {
        c.roping.Slide(-1);
      }

      HingeJoint2D newTop = top.gameObject.GetComponent<RopeSegment>().connectedBelow.GetComponent<HingeJoint2D>();
      newTop.connectedBody = hook;
      newTop.gameObject.transform.position = hook.gameObject.transform.position;
      newTop.GetComponent<RopeSegment>().ResetAnchor();
      Destroy(top.gameObject);
      top = newTop;
    }

  }
}
