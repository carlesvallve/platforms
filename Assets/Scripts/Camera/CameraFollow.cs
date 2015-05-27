using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	public Ent target;

	public float speedX = 100f;
	public float speedY = 5f;
	private float displaceX = 0; //0.3f;

	public Transform tileMap;
	private Bounds bounds;
	public Rect diff = new Rect(2.6f, 1.66f, 2.2f, 0.96f);

	//private Flashback94_PostProcess flash94;


	// ===========================================================
	// Camera Init
	// ===========================================================

	void Awake () {
		//flash94 = transform.Find("TileLayer").GetComponent<Flashback94_PostProcess>();

		if (!target) {
			Debug.LogWarning("Camera in this scene has no target!");
			return;
		}

		transform.position = target.transform.position;

		bounds = CalculateBounds(tileMap);
	}

	
	// ===========================================================
	// Camera Update
	// ===========================================================

	void Update () {
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
			float minY = bounds.min.y + diff.y;
			y = Mathf.Lerp(transform.position.y, Mathf.Max(target.transform.position.y, minY), Time.deltaTime * speed);
		}

		// locate camera
		transform.position = new Vector3(x, y, -10);

		// limit camera position to scene bounds
		//ApplyBoundLimits();
	}


	private bool CanFollowHorizontal () {
		//if (target.state == States.ATTACK || target.state == States.HURT) { return false; }
		return true;
	}

	
	private bool CanFollowVertical () {
		bool below = target.controller.collisions.below;
		if (below) { return true; }
		if (!below && target.transform.position.y < transform.position.y) { return true; }
		if (target.IsOnLadder()) { return true; }
		//if (target.IsInWater()) { return true; }

		return false;
	}


	// ===========================================================
	// Camera Bounds
	// ===========================================================

	private Bounds CalculateBounds (Transform tr) {
		if (tr == null) {
			Debug.LogWarning("Camera requires a transform to calculate scene bounds!");
			return new Bounds();
		}

		Bounds bounds = new Bounds (tr.position, Vector3.one);
		Renderer[] renderers = tr.GetComponentsInChildren<Renderer> ();
		foreach (Renderer renderer in renderers) {
			bounds.Encapsulate (renderer.bounds);
		}

		//print ("Scene bounds: " + bounds);
		return bounds;
	}


	private void ApplyBoundLimits () {
		// apply bound limits
		if (!tileMap) { return; }

		if (transform.position.x < bounds.min.x + diff.x) {
			transform.position = new Vector2(bounds.min.x + diff.x, transform.position.y);
		}

		if (transform.position.x > bounds.max.x - diff.width) {
			transform.position = new Vector2(bounds.max.x - diff.width, transform.position.y);
		}

		if (transform.position.y < bounds.min.y + diff.y) {
			transform.position = new Vector2(transform.position.x, bounds.min.y + diff.y);
		}

		if (transform.position.y > bounds.max.y - diff.height) {
			transform.position = new Vector2(transform.position.x, bounds.max.y - diff.height);
		}
	}


	/*public IEnumerator Pixelate (float time) {
		flash94.downsampling = Flashback94_PostProcess.DownsampleType.RELATIVE;
		flash94.downsampleRelativeAmount = 2;

		float startTime = Time.time;
		while(Time.time < startTime + time) {
			flash94.downsampleRelativeAmount += 1;
			yield return null;
		}

		flash94.downsampling = Flashback94_PostProcess.DownsampleType.NONE;
	}*/
}
