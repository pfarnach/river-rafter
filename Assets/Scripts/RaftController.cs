using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaftController : MonoBehaviour {

	public static int maxHealth = 5;
	public event System.Action<int, bool> takeDamage;
	public float horizontalMovementSpeed;
	public Transform riverGO;
	public GameObject explosionPrefab;

	private int health;
	private AudioSource audioSrc;
	private Rigidbody2D rb2d;
	private SpriteRenderer spriteRenderer;
	private Shader shaderGUItext;
	private Shader shaderSpritesDefault;
	private WaitForSeconds flashWait;
	private float horizontalMovement = 0f;
	private GameObject lastCollidedObject;
	private float lastCollisionTime = 0f;

	// Touch controls stuff
	private Vector2 touchOrigin = -Vector2.one;
	private float timeSinceLastTouch = 0f;
	private float touchInputDuration = 0.22f;

	void Awake () {
		health = maxHealth;

		audioSrc = GetComponent <AudioSource> ();
		rb2d = GetComponent <Rigidbody2D> ();
		spriteRenderer = GetComponent <SpriteRenderer> ();

		shaderGUItext = Shader.Find("GUI/Text Shader");
		shaderSpritesDefault = Shader.Find("Sprites/Default"); // or whatever sprite shader is being used
		flashWait = new WaitForSeconds (0.05f);
	}

	void Update () {
		#if UNITY_STANDALONE
		horizontalMovement = Input.GetAxisRaw ("Horizontal");
		#else
		// Disable touch controls for first second so no extraneous input is registered
		if (Time.timeSinceLevelLoad < 1f) {
			return;
		}

		if (Input.touchCount > 0) {
			timeSinceLastTouch = 0f;
			Touch myTouch = Input.touches[0];

			//Check if the phase of that touch equals Began
			if (myTouch.phase == TouchPhase.Began) {
				//If so, set touchOrigin to the position of that touch
				touchOrigin = myTouch.position;
			} else if (myTouch.phase == TouchPhase.Ended) {
				//Calculate the difference between the beginning and end of the touch on the x axis.
				float x = myTouch.position.x - touchOrigin.x;

				//Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
				touchOrigin.x = -1;

				//If x is greater than zero, set horizontal to 1, otherwise set it to -1
				horizontalMovement = x > 0 ? 1 : -1;
			}
		} else {
			timeSinceLastTouch += Time.deltaTime;
		}

		// Only want previous touch's direction to influence movement for small amount of time
		if (timeSinceLastTouch > touchInputDuration) {
			horizontalMovement = 0f;
		}
		#endif

		// Turn rotation
		if (horizontalMovement > 0) {
			transform.rotation = Quaternion.RotateTowards (Quaternion.identity, Quaternion.Euler (0f, 0f, -20f), 6f);
		} else if (horizontalMovement < 0) {
			transform.rotation = Quaternion.RotateTowards (Quaternion.identity, Quaternion.Euler (0f, 0f, 20f), 6f);
		} else {
			transform.rotation = Quaternion.RotateTowards (Quaternion.identity, Quaternion.Euler (0f, 0f, 0f), 6f);
		}

		lastCollisionTime += Time.deltaTime;
	}

	void FixedUpdate () {
		rb2d.velocity = Vector2.right * horizontalMovement * horizontalMovementSpeed * Time.fixedDeltaTime;
	}

	void LateUpdate () {
		// Clamp raft's position to stay inside river
		Vector3 raftPos = transform.position;
		raftPos.x = Mathf.Clamp (raftPos.x, -1.9f, 1.9f);
		transform.position = raftPos;
	}

	void OnTriggerEnter2D (Collider2D other) {
		// Ensure that raft can't be hit by same object within certain buffer time
		// Since gameObjects are pooled and re-used, it's not enough to only check gameObject equality
		if (lastCollisionTime > 1f || other.gameObject != lastCollidedObject) {
			// Damage invincibility check
			lastCollidedObject = other.gameObject;
			lastCollisionTime = 0f;

			// Play hit sound
			audioSrc.Play ();

			// Apply damage and damage FX
			health--;
			StartCoroutine (DamageFlash ());
			ApplyHitBumpToRaft (other.transform.position.x);

			// Trigger damage event	
			if (takeDamage != null)
				takeDamage (health, health <= 0);
			
			// Death check
			if (health <= 0) {
				// Create raft explosion and parent it to river so it floats
				GameObject explosion = Instantiate (explosionPrefab, transform.position + Vector3.up * 1.2f, Quaternion.identity);
				explosion.transform.parent = riverGO;
				Destroy (gameObject);
			}
		}
	}

	IEnumerator DamageFlash () {
		// http://answers.unity3d.com/questions/582145/is-there-a-way-to-set-a-sprites-color-solid-white.html
		spriteRenderer.material.shader = shaderGUItext;
		yield return flashWait;
		spriteRenderer.material.shader = shaderSpritesDefault;
		yield return flashWait;
		spriteRenderer.material.shader = shaderGUItext;
		yield return flashWait;
		spriteRenderer.material.shader = shaderSpritesDefault;
	}

	void ApplyHitBumpToRaft (float rockPosX) {
		// Want to apply a force to the boat towards the river when it hits a rock for added effect
		transform.Translate (Vector2.right * 0.25f * (transform.position.x < rockPosX ? -1 : 1));
	}
}
