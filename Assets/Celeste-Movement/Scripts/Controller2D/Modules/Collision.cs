using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Carles.Engine2D {

  public class Collision : MonoBehaviour {

    // Note: Exclude the layer of the character associated with this script
    // or otherwise things won't work properly
    [Header("Layers")]
    public LayerMask groundLayer;

    [Space]
    [Header("States")]
    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;

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

    void Start() {
      Physics2D.IgnoreCollision(characterCollider, blockerCollider, true);
      blockerCollider.gameObject.SetActive(!canPushCharacters);
    }

    // Update is called once per frame
    void Update() {
      onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
      onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer)
          || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

      onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
      onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

      wallSide = onRightWall ? -1 : 1;
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
