using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class Collision : MonoBehaviour {

    // Note: Exclude the layer of the character associated with this script
    // or otherwise things won't work properly
    [Header("Layers")]
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public LayerMask oneWayPlatformLayer;

    [Space]
    [Header("States")]
    public bool onGround;
    public bool onWater;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;

    public GameObject currentOneWayPlatform;

    [Space]
    [Header("Collision")]
    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private Color debugCollisionColor = Color.green;

    [Space]
    [Header("Character Collision")]
    public CapsuleCollider2D characterCollider;
    public CapsuleCollider2D blockerCollider;
    public bool canPushCharacters = true;

    private CharController2D c;

    void Start() {
      c = GetComponent<CharController2D>();
      Physics2D.IgnoreCollision(characterCollider, blockerCollider, true);
      blockerCollider.gameObject.SetActive(!canPushCharacters);
    }

    // Update is called once per frame
    void Update() {
      Collider2D oneWayCollider = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, oneWayPlatformLayer);
      currentOneWayPlatform = oneWayCollider ? oneWayCollider.gameObject : null;

      onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
      onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, wallLayer)
          || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, wallLayer);

      onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, wallLayer);
      onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, wallLayer);

      wallSide = onRightWall ? -1 : 1;

      // don't compute walls if we are inside water
      if (onWater) {
        // onGround = false; 
        onWall = false; onRightWall = false; onLeftWall = false;
      }
    }

    void OnDrawGizmos() {
      Gizmos.color = debugCollisionColor;

      var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

      Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
      Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
      Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }
  }

}
