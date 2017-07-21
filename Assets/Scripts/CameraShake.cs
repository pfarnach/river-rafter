using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	[Range(0f, 0.1f)]
	public float shakeAmt;
	public float shakeDuration;

	private Camera mainCam;
	private Vector3 mainCamOriginalPos;


	void Awake () {
		mainCam = GetComponent <Camera> ();
		mainCamOriginalPos = mainCam.transform.position;
	}

	void OnEnable () {
		FindObjectOfType<RaftController> ().takeDamage += Shake;
	}

	void OnDisable () {
		if (FindObjectOfType<RaftController> () != null)
			FindObjectOfType<RaftController> ().takeDamage -= Shake;
	}

	void Shake (int health, bool isDead) {
		InvokeRepeating ("BeginShake", 0, 0.01f);
		Invoke ("StopShake", shakeDuration);
	}

	void BeginShake () {
		if (shakeAmt > 0) {
			Vector3 camPos = mainCam.transform.position;
			float offsetX = Random.value * shakeAmt * 5 - shakeAmt;
			float offsetY = Random.value * shakeAmt * 5 - shakeAmt;
			camPos.x += offsetX;
			camPos.y += offsetY;

			mainCam.transform.position = camPos;
		}
	}

	void StopShake () {
		CancelInvoke ("BeginShake");
		mainCam.transform.localPosition = mainCamOriginalPos;
	}

}
