using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	public static MusicManager instance;

	void Awake () {
		// First we check if there are any other instances conflicting
		if (instance != null && instance != this) {
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		} else {
			// Here we save our singleton instance
			instance = this;

			DontDestroyOnLoad(gameObject);	
		}
	}

}
