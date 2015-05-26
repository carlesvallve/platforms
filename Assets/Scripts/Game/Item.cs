using UnityEngine;
using System.Collections;

public class Item : Ent {

	public override void Awake () {
		base.Awake();
	}


	protected override void CheckCollisionTarget () {
		//if (controller.collisions.below) { return; }

		// check if we jumped over a monster, if so, rebound in him and kill it
		if (controller.collisions.target) {
			Ent target = controller.collisions.target.GetComponent<Ent>();

			//jumping = false;
			//SetJump(false, 1f);

			float knockback = 1f;
			Vector2 d = (target.transform.position - transform.position).normalized * knockback;
			StartCoroutine(target.Hurt(d));
		}
	}
}
