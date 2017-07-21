using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverMover : MonoBehaviour {

	public float moveSpeed;
	public float moveDistance;

	private float timeSinceLastMove = 0f;

	void OnEnable () {
		FindObjectOfType<GameManagerController> ().nextDifficultyLevel += IncreaseRiverSpeed;
	}

	void OnDisable () {
		if (FindObjectOfType<GameManagerController> () != null)
			FindObjectOfType<GameManagerController> ().nextDifficultyLevel -= IncreaseRiverSpeed;
	}

	void Update () {
		// Move sprite down
		if (timeSinceLastMove > 1 / moveSpeed) {
			timeSinceLastMove = 0f;
			transform.position += -Vector3.up * moveDistance * Time.deltaTime;
		} else {
			timeSinceLastMove += Time.deltaTime;
		}
	}

	void IncreaseRiverSpeed () {
		moveDistance += 3f;
	}

}
