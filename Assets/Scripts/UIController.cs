using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public Text timeSurvivedText;
	public GameObject gameOverGO;
	public GameObject healthBarContainer;
	public RawImage healthBarImage;
	public Gradient healthColors;
	public float damageLerpDuration;


	void Awake () {
		healthBarImage.color = healthColors.Evaluate (1f);
	}

	void OnEnable () {
		FindObjectOfType<RaftController> ().takeDamage += TakeDamage;
	}

	void OnDisable () {
		if (FindObjectOfType<RaftController> () != null)
			FindObjectOfType<RaftController> ().takeDamage -= TakeDamage;
	}

	void TakeDamage (int health, bool isDead) {
		if (isDead) {
			healthBarContainer.SetActive (false);
			timeSurvivedText.text = Time.timeSinceLevelLoad.ToString ("F1");
			gameOverGO.SetActive (true);
		} else {
			StartCoroutine (LerpHealthBar ((float)health / RaftController.maxHealth));
		}
	}

	IEnumerator LerpHealthBar (float healthBarPerc) {
		float startTime = Time.time;

		Vector3 healthBarScaleSource = healthBarImage.transform.localScale;
		Vector3 healthBarScaleTarget = new Vector3(healthBarPerc, healthBarScaleSource.y, healthBarScaleSource.z);

		Color colorSource = healthBarImage.color;
		Color colorTarget = healthColors.Evaluate (healthBarPerc);

		// Lerp the color and scale size
		while (Time.time < startTime + damageLerpDuration) {
			float t = (Time.time - startTime) / damageLerpDuration;
			healthBarImage.color = Color.Lerp (colorSource, colorTarget, t);
			healthBarImage.transform.localScale = Vector3.Lerp (healthBarScaleSource, healthBarScaleTarget, t);

			yield return null;
		}
	}

}
