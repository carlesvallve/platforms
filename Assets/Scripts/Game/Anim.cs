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
	public GameObject go;
	public GameObject hair;
	public GameObject beard;
	public GameObject hat;
}

[System.Serializable]
public class Torso {
	public GameObject go;
	public GameObject jacket;
}

[System.Serializable]
public class Arms {
	public GameObject go;
	public GameObject empty;
	public GameObject onehand;
	public GameObject twohand;
	public GameObject overhead;
	public GameObject drag;
	public GameObject shield;
}

[System.Serializable]
public class Legs {
	public GameObject go;
	public GameObject pants;
	public GameObject shoes;
}



public class Anim : MonoBehaviour {

	Ent ent;
	Animator animator;

	public BodyParts body;


	void Start () {
		ent = GetComponent<Ent>();
		animator = transform.Find("Sprite").GetComponent<Animator>();
		GetBodyParts();
	}


	private void GetBodyParts () {
		body = new BodyParts();
		
		body.head = new Head();
		body.head.go = transform.Find("Sprite/Head").gameObject;
		body.head.hair = transform.Find("Sprite/Head/TagHair").gameObject;
		body.head.beard = transform.Find("Sprite/Head/TagBeard").gameObject;
		body.head.hat = transform.Find("Sprite/Head/TagHat").gameObject;
		
		body.torso = new Torso();
		//body.torso.jacket = transform.Find("Sprite/Torso/TagJacket").gameObject;
		
		body.arms = new Arms();
		body.arms.go = transform.Find("Sprite/Arms").gameObject;
		body.arms.empty = transform.Find("Sprite/Arms/Empty").gameObject;
		body.arms.onehand = transform.Find("Sprite/Arms/OneHand").gameObject;
		body.arms.twohand = transform.Find("Sprite/Arms/TwoHand").gameObject;
		body.arms.overhead = transform.Find("Sprite/Arms/OverHead").gameObject;
		body.arms.drag = transform.Find("Sprite/Arms/Drag").gameObject;
		body.arms.shield = transform.Find("Sprite/Arms/TagShield").gameObject;
		
		body.legs = new Legs();
		body.legs.go = transform.Find("Sprite/Legs").gameObject;
		//body.legs.pants = transform.Find("Sprite/Legs/Pants").gameObject;
		//body.legs.shoes = transform.Find("Sprite/Legs/Shoes").gameObject;

		body.head.hair.SetActive(false);
		body.head.beard.SetActive(false);
		body.head.hat.SetActive(false);
		//body.torso.jacket.SetActive(false);
		body.arms.empty.SetActive(true);
		body.arms.onehand.SetActive(false);
		body.arms.twohand.SetActive(false);
		body.arms.overhead.SetActive(false);
		body.arms.drag.SetActive(false);
		body.arms.shield.SetActive(false);

		//body.legs.pants.SetActive(false);
		//body.legs.shoes.SetActive(false);
	}
	

	public void ChangeStance (GameObject armStance) {
		foreach (Transform child in body.arms.go.transform) {
			child.gameObject.SetActive(false);
		}

		armStance.SetActive(true);
	}


	void Update () {
		if (Input.GetKeyDown(KeyCode.Q)) { ChangeStance(body.arms.empty); }
		if (Input.GetKeyDown(KeyCode.W)) { ChangeStance(body.arms.onehand); }
		if (Input.GetKeyDown(KeyCode.E)) { ChangeStance(body.arms.twohand); }
		if (Input.GetKeyDown(KeyCode.R)) { ChangeStance(body.arms.overhead); }
		if (Input.GetKeyDown(KeyCode.T)) { ChangeStance(body.arms.shield); }
	}


	public void Play(string clipName) {
		animator.Play(clipName);
	}
}
