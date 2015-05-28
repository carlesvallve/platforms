using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {

	private Ent ent;
	public bool landed = false;

	public LayerMask collisionMask;
	public LayerMask attackCollisionMask;

	const float skinWidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	float maxClimbAngle = 80;
	float maxDescendAngle = 80;

	float horizontalRaySpacing;
	float verticalRaySpacing;

	private BoxCollider2D boxCollider;
	RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;

	public bool triggerEvents = true;


	void Awake() {
		ent = GetComponent<Ent>();
		boxCollider = GetComponent<BoxCollider2D> ();
		CalculateRaySpacing ();
	}


	public void ToggleCollider (bool value) {
		boxCollider.enabled = value;
	}


	public void Move(Vector3 velocity, bool jumpingDown = false) {
		UpdateRaycastOrigins ();
		collisions.Reset ();
		collisions.velocityOld = velocity;

		if (velocity.y < 0) {
			DescendSlope(ref velocity);
		}

		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}

		if (velocity.y != 0) {
			VerticalCollisions (ref velocity, jumpingDown);
		}

		if (velocity.x != 0 || velocity.y != 0) { 
			AttackCollisions (ref velocity);
		}

		transform.Translate (velocity);
	}


	void HorizontalCollisions(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;
		
		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			if (hit) {

				// handle one way collision layer
				if (hit.transform.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform")) {
					continue;
				}

				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxClimbAngle) {
					if (collisions.descendingSlope) {
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance-skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) {
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}
	}

	
	void VerticalCollisions(ref Vector3 velocity, bool jumpingDown = false) {
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;


		for (int i = 0; i < verticalRayCount; i ++) {
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			if (hit) {

				// handle one way collision layer
				if (hit.transform.gameObject.layer == LayerMask.NameToLayer("OneWayPlatform")) {
					if (directionY == 1 || (directionY == - 1 && jumpingDown)) {
						continue;
					}
				}

				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (collisions.climbingSlope) {
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			} 
		}

		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right * directionX,rayLength, collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle(hit.normal,Vector2.up);
				if (slopeAngle != collisions.slopeAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}


		// trigger landing on ent
		if (triggerEvents && !landed && collisions.below && directionY == -1) { ent.TriggerLanding(); } 
		landed = collisions.below;

	}


	void AttackCollisions(ref Vector3 velocity, bool jumpingDown = false) {
		float rayLength;

		// vertical
		float directionY = Mathf.Sign (velocity.y);
		if (directionY == -1 && !collisions.below) { 

			rayLength = Mathf.Abs (velocity.y) + skinWidth;
			for (int i = 0; i < verticalRayCount; i ++) {
				Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
				rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, attackCollisionMask);
				Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

				if (hit) {
					if (triggerEvents) { 
						ent.TriggerCollisionAttack(hit.transform.gameObject); 
					}
					return;
				}
			}
		}

		// horizontal
		float directionX = Mathf.Sign (velocity.x);
		if (directionX != 0 && !collisions.below) {
		
			rayLength = Mathf.Abs (velocity.x) + skinWidth;
			for (int i = 0; i < horizontalRayCount; i ++) {
				Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
				rayOrigin += Vector2.up * (horizontalRaySpacing * i);
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, attackCollisionMask);
				Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

				if (hit) {
					if (triggerEvents) { 
						ent.TriggerCollisionAttack(hit.transform.gameObject); 
					}
					return;
				}
			}
		}
		
	}


	void ClimbSlope(ref Vector3 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= climbVelocityY) {
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		}
	}


	void DescendSlope(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				if (Mathf.Sign(hit.normal.x) == directionX) {
					if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)) {
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
						velocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}


	void UpdateRaycastOrigins() {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}


	void CalculateRaySpacing() {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}


	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
	

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;
		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector3 velocityOld;

		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;
			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}

}
