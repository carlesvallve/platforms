using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
  public int score;
  public int curHp;
  public int maxHp;

  public float moveSpeed;
  public float jumpForce;
  public int maxJumps;

  private int jumpsAvailable;
  private float curMoveInput;

  [Header("Controllers")]
  public Rigidbody2D rb;
  public Animator anim;
  public Transform muzzle;

  //   void Start() {

  //   }


  //   void Update() {

  //   }

  // update physics related stuff
  void FixedUpdate() {
    Move();

    if (curMoveInput != 0) {
      transform.localScale = new Vector3(curMoveInput > 0 ? 1 : -1, 1, 1);
    }
  }

  void Move() {
    rb.velocity = new Vector2(curMoveInput * moveSpeed, rb.velocity.y);
  }



  void Jump() {
    Debug.Log("Jump");
    rb.velocity = new Vector2(rb.velocity.x, 0);
    rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
  }

  //

  public void OnInputMove(InputAction.CallbackContext context) {
    curMoveInput = context.ReadValue<float>();
  }

  public void OnInputJump(InputAction.CallbackContext context) {
    Debug.Log("OnInputJump " + context.phase);
    // first frame that button is pressed down
    if (context.phase == InputActionPhase.Performed) {
      if (jumpsAvailable > 0) {
        jumpsAvailable -= 1;
        Jump();
      }
    }
  }

  public void OnInputAttack(InputAction.CallbackContext context) {

  }

  void OnCollisionEnter2D(Collision2D collision) {
    Debug.Log("OnCollisionEnter2D " + collision.contacts[0].point.y + " / " + transform.position.y);
    // player touches ground
    if (collision.contacts[0].point.y < transform.position.y + 0.1) {
      Debug.Log("Grounded");
      jumpsAvailable = maxJumps;
    }
  }


}
