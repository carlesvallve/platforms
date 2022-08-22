using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

namespace Carles.Engine2D {

  public class Camera2D : MonoBehaviour {

    public Transform target;
    public float speedX = 100f;
    public float speedY = 5f;
    private float displaceX = 0; //0.3f;

    // public Transform tileMap;
    // public Rect limits = new Rect(-3, -2, 1.6f, 1.3f);

    // ===========================================================
    // Camera Init
    // ===========================================================

    void Start() {
      // auto-get player as camera target
      PlayerInput pi = FindObjectOfType<PlayerInput>();
      if (pi) target = pi.transform;

      if (!target) {
        Debug.LogWarning("Camera - has no target!");
        return;
      }

      transform.position = target.transform.position;
    }


    // ===========================================================
    // Camera Update
    // ===========================================================

    void Update() {
      if (!target) { return; }

      float x = transform.position.x;
      float y = transform.position.y;

      // follow horizontally
      if (CanFollowHorizontal()) {
        float targetX = target.transform.position.x + (displaceX * target.transform.localScale.x);
        x = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * speedX); //  / 5
      }


      // follow vertically
      if (CanFollowVertical()) {
        float speed = speedY;
        //if (!below && target.transform.position.y < transform.position.y) { speed = speedY * 5; }
        float minY = -1000; // bounds.min.y + diff.y;
        y = Mathf.Lerp(transform.position.y, Mathf.Max(target.transform.position.y, minY), Time.deltaTime * speed);
      }

      // locate camera
      transform.position = new Vector3(x, y, -1);

      // apply camera limits
      // ApplyLimits();
    }


    private bool CanFollowHorizontal() {
      //if (target.state == States.ATTACK || target.state == States.HURT) { return false; }
      return true;
    }


    private bool CanFollowVertical() {
      return true;

      // bool below = target.controller.collisions.below;
      // if (below) { return true; }
      // if (!below && target.transform.position.y < transform.position.y) { return true; }
      // if (target.IsOnLadder()) { return true; }
      // if (target.IsOnWater()) { return true; }

      // return false;
    }


    // ===========================================================
    // Camera Bounds
    // ===========================================================

    // private void ApplyLimits() {
    //   if (transform.position.y < limits.y) {
    //     transform.position = new Vector3(transform.position.x, limits.y, transform.position.z);
    //   }

    //   if (transform.position.y > limits.height) {
    //     transform.position = new Vector3(transform.position.x, limits.height, transform.position.z);
    //   }

    //   if (transform.position.x < limits.x) {
    //     transform.position = new Vector3(limits.x, transform.position.y, transform.position.z);
    //   }

    //   if (transform.position.x > limits.width) {
    //     transform.position = new Vector3(limits.width, transform.position.y, transform.position.z);
    //   }
    // }
  }

}
