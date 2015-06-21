using UnityEngine;
using System.Collections;


[System.Serializable]
public class BodyParts {
	public Head head;
	public Torso torso;
	public Arms arms;
	public Legs legs;
}

[System.Serializable]
public class Head {
	public Transform go;
	public Transform hair;
	public Transform beard;
	public Transform hat;
}

[System.Serializable]
public class Torso {
	public Transform go;
	public Transform jacket;
}

[System.Serializable]
public class Arms {
	public Transform go;

	public Transform empty;
	public Transform onehand;
	public Transform twohand;
	public Transform overhead;
	public Transform drag;

	public Transform weapon;
	public Transform shield;
}

[System.Serializable]
public class Legs {
	public Transform go;
	public Transform pants;
	public Transform shoes;
}


public class Anim : MonoBehaviour {

	//Ent ent;
	Animator animator;

	public BodyParts body;

	public string stanceType;


	void Start () {
		//ent = GetComponent<Ent>();
		animator = transform.Find("Sprite").GetComponent<Animator>();
		GetBodyParts();
		InitBodyParts();
		ChangeArmStance(body.arms.twohand);
	}


	private void GetBodyParts () {
		body = new BodyParts();
		
		body.head = new Head();
		body.head.go = transform.Find("Sprite/Head");
		body.head.hair = transform.Find("Sprite/Head/TagHair");
		body.head.beard = transform.Find("Sprite/Head/TagBeard");
		body.head.hat = transform.Find("Sprite/Head/TagHat");
		
		body.torso = new Torso();
		//body.torso.jacket = transform.Find("Sprite/Torso/TagJacket");
		
		body.arms = new Arms();
		body.arms.go = transform.Find("Sprite/Arms");
		body.arms.empty = transform.Find("Sprite/Arms/Empty");
		body.arms.onehand = transform.Find("Sprite/Arms/OneHand");
		body.arms.twohand = transform.Find("Sprite/Arms/TwoHand");
		body.arms.overhead = transform.Find("Sprite/Arms/OverHead");
		body.arms.drag = transform.Find("Sprite/Arms/Drag");
		
		body.arms.weapon = null;
		body.arms.shield = transform.Find("Sprite/Arms/TagShield");
		
		body.legs = new Legs();
		body.legs.go = transform.Find("Sprite/Legs");
	}


	private void InitBodyParts () {
		body.head.hair.gameObject.SetActive(false);
		body.head.beard.gameObject.SetActive(false);
		body.head.hat.gameObject.SetActive(true);
		body.arms.empty.gameObject.SetActive(true);
		body.arms.onehand.gameObject.SetActive(false);
		body.arms.twohand.gameObject.SetActive(false);
		body.arms.overhead.gameObject.SetActive(false);
		body.arms.drag.gameObject.SetActive(false);
		body.arms.shield.gameObject.SetActive(false);
	}
	

	


	void Update () {
		if (Input.GetKeyDown(KeyCode.Q)) { ChangeArmStance(body.arms.empty); }
		if (Input.GetKeyDown(KeyCode.W)) { ChangeArmStance(body.arms.onehand); }
		if (Input.GetKeyDown(KeyCode.E)) { ChangeArmStance(body.arms.twohand); }
		if (Input.GetKeyDown(KeyCode.R)) { ChangeArmStance(body.arms.overhead); }
		if (Input.GetKeyDown(KeyCode.T)) { ChangeArmStance(body.arms.drag); }
	}

	public AnimationClip GetCurrentClip () {
		return animator.GetCurrentAnimatorClipInfo(0)[0].clip;
	}

	public void Play(string clipName, float speed = 1) {
		animator.speed = speed;
		animator.Play(clipName);

		PlayTag(body.head.hat, clipName, speed);
	}


	private void PlayTag (Transform tag, string clipName, float speed = 1) {
		if (tag.childCount == 0) { 
			return; 
		}
		
		Transform tr = tag.GetChild(0);
		if (!tr) { return; }

		Animator a = tr.GetComponent<Animator>();
		if (!a) { return; }

		a.speed = speed;
		a.Play(clipName);
	}


	public void ChangeArmStance (Transform armStance) {
		foreach (Transform child in body.arms.go) {
			child.gameObject.SetActive(false);
		}

		armStance.gameObject.SetActive(true);

		body.arms.weapon = armStance.Find("Tag" + armStance.name);

		stanceType = armStance.name;
	}


	public string GetAttackAnimation () {
		// TODO: We have to figure out if we are carrying a projectile or melee weapon
		switch (stanceType) {
		case "Empty":
		case "OneHand":
			return "attack1h90";
		case "TwoHand":
			return "attack2h90";
		case "OverHead":
			return "throw90";
		case "Drag":
			return "attack1h90";
		}

		return "attack1h90";
	}

}
