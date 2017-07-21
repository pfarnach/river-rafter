using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawnerController : MonoBehaviour {

	public GameObject rockPrefab1;
	public GameObject rockPrefab2;
	public Transform rocksParent;
	public int poolSize;
	public float spawnRate;

	private GameObject[] rocks;
	private int currentSpawnIndex = 0;
	private float timeSinceLastSpawn = 0f;

	void OnEnable () {
		FindObjectOfType<GameManagerController> ().nextDifficultyLevel += IncreaseSpawnRate;
	}

	void OnDisable () {
		if (FindObjectOfType<GameManagerController> () != null)
			FindObjectOfType<GameManagerController> ().nextDifficultyLevel -= IncreaseSpawnRate;
	}

	void Start () {
		rocks = new GameObject[poolSize];

		for (int i = 0; i < poolSize; i++) {
			rocks [i] = Instantiate (i % 2 == 0 ? rockPrefab1 : rockPrefab2) as GameObject;
			rocks [i].transform.parent = rocksParent;
			rocks [i].SetActive (false);
		}
	}
	
	void Update () {
		if (timeSinceLastSpawn > 1 / spawnRate) {
			Vector3 spawnPos = new Vector3 (Random.Range (-1.9f, 1.9f), transform.position.y, transform.position.z);
			rocks [currentSpawnIndex].transform.position = spawnPos;
			rocks [currentSpawnIndex].SetActive (true);

			currentSpawnIndex = currentSpawnIndex >= poolSize - 1 ? 0 : currentSpawnIndex + 1;
			timeSinceLastSpawn = 0;
		} else {
			timeSinceLastSpawn += Time.deltaTime;
		}
	}

	void IncreaseSpawnRate () {
		spawnRate += 0.25f;
	}
}
