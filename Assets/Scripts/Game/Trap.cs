using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour {

	public float delayActivate = 0.25f;
	public float delayReset = 2f;

	public bool active = false;

	private Ent block;
	private TextMesh info;

	private Vector3 originalPos;

	void Awake () {
		block = transform.Find("Block").GetComponent<Ent>();
		originalPos = block.transform.position;

		info = transform.Find("Info").GetComponent<TextMesh>();
		StartCoroutine(UpdateInfo(null));
	}


	public virtual IEnumerator UpdateInfo (string str) {
		if (!info) { yield break; }

		info.gameObject.SetActive(str != null);
		if (str == null) { yield break; }
		info.text = str;
	}


	public void Activate () {
		StartCoroutine(PlayTrap());
	}
	
	
	public IEnumerator PlayTrap () {
		if (active) { yield break; }

		Audio.play("Audio/sfx/Tick", 0.4f, Random.Range(1.5f, 1.5f));
		StartCoroutine(UpdateInfo("CLICK!"));
		active = true;

		yield return new WaitForSeconds(delayActivate);

		StartCoroutine(UpdateInfo(null));
		block.affectedByGravity = true;


		yield return new WaitForSeconds(delayReset);
		StartCoroutine(RewindTrap());
	}


	public IEnumerator RewindTrap () {
		block.affectedByGravity = false;

		Vector3 startPos = block.transform.position;

		float i = 0;
		float rate = 1f / 2f;
		while (i < 1f) {
			i += Time.deltaTime * rate;
			block.transform.position = Vector3.Lerp(startPos, originalPos, i);
			yield return null;
		}

		block.transform.position = originalPos;

		active = false;
	}
}
