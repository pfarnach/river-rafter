using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverSegment : MonoBehaviour {

	private float spriteHeight;

	void Start () {
		spriteHeight = GetComponent <SpriteRenderer> ().size.y;
	}

	void Update () {
		// Reset sprite up to top once it goes off screen
		if (transform.position.y < - 10) {
			transform.position += Vector3.up * spriteHeight * 2f;
		}
	}

}
