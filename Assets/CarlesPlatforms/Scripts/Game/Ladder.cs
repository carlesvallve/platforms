using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {


	public Sprite spriteTop;
	public Sprite spriteCenter;
	public Sprite spriteBottom;

	private Transform top;
	private Transform bottom;
	private int height;


	void Awake () {
		top = transform.Find("Top");
		bottom = transform.Find("Bottom");
		height = (int)top.transform.localPosition.y;

		for (int i = 1; i < height; i++) {
			Transform tile = ((Transform)Instantiate(bottom, transform.position + Vector3.up * i, Quaternion.identity));
			tile.GetComponent<SpriteRenderer>().sprite = spriteCenter;
			tile.SetParent(transform);
			tile.name = "Center" + i;
		}
	}


	public float GetHeight () {
		return (float)(height);
	}
	
}
