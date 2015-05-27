using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {

	public int height = 3;
	public Sprite spriteTop;
	public Sprite spriteCenter;
	public Sprite spriteBottom;


	private Transform top;
	private Transform bottom;


	void Awake () {
		top = transform.Find("Top");
		bottom = transform.Find("Bottom");

		bottom.transform.localPosition = Vector3.up * 0;
		top.transform.localPosition = Vector3.up * (height - 1);

		for (int i = 1; i < height - 1; i++) {
			Transform tile = ((Transform)Instantiate(bottom, transform.position + Vector3.up * i, Quaternion.identity));
			tile.GetComponent<SpriteRenderer>().sprite = spriteCenter;
			tile.SetParent(transform);
			tile.name = "Center" + i;
		}

	}
	
}
