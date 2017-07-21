using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Background music: https://freesound.org/people/dAmbient/sounds/235349/

public class GameManagerController : MonoBehaviour {

	public event System.Action nextDifficultyLevel;
	public float timeBetweenLevels;

	private float timeSinceNextLevel = 0f;
	private bool isGameOver = false;

	void OnEnable () {
		FindObjectOfType<RaftController> ().takeDamage += UpdateHealth;
	}

	void OnDisable () {
		if (FindObjectOfType<RaftController> () != null)
			FindObjectOfType<RaftController> ().takeDamage -= UpdateHealth;
	}

	void Update () {
		if (isGameOver && Input.GetMouseButtonDown (0)) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}

		if (!isGameOver) {
			// Increase difficulty if enough time has passed
			if (timeSinceNextLevel > timeBetweenLevels) {
				timeSinceNextLevel = 0f;

				if (nextDifficultyLevel != null)
					nextDifficultyLevel ();
			} else {
				timeSinceNextLevel += Time.deltaTime;
			}	
		}
	}

	void UpdateHealth (int nextHealth, bool isDead) {
		if (isDead) {
			// Stop river from speeding up after death			
			isGameOver = true;
		}
	}
}
