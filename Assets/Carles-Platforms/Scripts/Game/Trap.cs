using UnityEngine;
using System.Collections;


public enum TrapTypes {
	BLOCK = 0,
	SPIKE = 1
}


public class Trap : MonoBehaviour {

	public TrapTypes type = TrapTypes.BLOCK;
	public float delayActivate = 0.3f;
	public float delayReset = 2f;
	public bool active = false;

	private Ent trap;
	private TextMesh info;
	private Vector3 originalPos;


	void Awake () {
		trap = transform.Find("Trap").GetComponent<Ent>();
		originalPos = trap.transform.position;

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
		
		if (type == TrapTypes.BLOCK) {
			StartCoroutine(PlayTrapBlock());
		} else if (type == TrapTypes.SPIKE) {
			StartCoroutine(PlayTrapSpike());
		}

		yield return new WaitForSeconds(delayReset);
		StartCoroutine(RewindTrap());
	}


	public IEnumerator PlayTrapBlock () {
		trap.affectedByGravity = true;
		yield break;
	}


	public IEnumerator PlayTrapSpike () {
		trap.affectedByGravity = false;

		Vector3 startPos = trap.transform.position;
		Vector3 endPos = startPos + Vector3.up * 1f;

		float i = 0;
		float rate = 1f / 0.1f;
		while (i < 1f) {
			i += Time.deltaTime * rate;
			trap.transform.position = Vector3.Lerp(startPos, endPos, i);
			yield return null;
		}

		trap.transform.position = endPos;
	}


	public IEnumerator RewindTrap () {
		trap.affectedByGravity = false;

		Vector3 startPos = trap.transform.position;
		Vector3 endPos = originalPos;

		float i = 0;
		float rate = 1f / (type == TrapTypes.BLOCK ? 2f : 0.2f);
		while (i < 1f) {
			i += Time.deltaTime * rate;
			trap.transform.position = Vector3.Lerp(startPos, endPos, i);
			yield return null;
		}

		trap.transform.position = endPos;

		active = false;
	}
}
