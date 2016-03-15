using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AIPlayerController : MonoBehaviour {
	private float turnProbability;
	private long turnQuanta;
	private long quantSeen;

	public GameObject explosion;
	public Transform playerTransform;
	public Pose3D devicePose;
	public Rigidbody bikeBody;
	public GameObject trailObject;
	public GameObject trailTurnObject;
	public AudioClip explodeClip;
	public GameObject gameOverMenu;

	private int startingDir; //this will be mod 4. 0 is forward. 1 is right. 2 is back. 3 is left.
	private bool inTurn;


	Transform cameraTransform;
	Vector3 forceDirection;

	private GameObject currTrailObj;
	private float currTrailOrigin;
	private bool gameOver;

	public AIPlayerController (float probability, long quanta)
	{
		turnProbability = probability;
		turnQuanta = quanta;
	}

	void Awake() {
		forceDirection = playerTransform.right;


	}

	void Start ()
	{
		quantSeen = 0;

		gameOver = false;
		startingDir = 0;
		inTurn = false;
	}


	void Update ()
	{
		


		bikeBody.velocity = Vector3.zero;
		bikeBody.rotation = playerTransform.rotation;
		bikeBody.AddRelativeForce (Vector3.right * 500);



	}

	//  TODO Interval should honestly vary automatically based on our choice
	//  of player speed, but it is hardcoded for now.
	int interval = 15;  
	float nextTime = 0.0f;
	float dropCount = 0;
	void LateUpdate() {

		quantSeen++;
		if (nextTime % 350 == 0 && nextTime > 10 && gameOver == false && inTurn == false) { 
			if (quantSeen >= turnQuanta) {
				// determine whether or not to make a turn
				float turnVal = Random.Range (0.0f, 1.0f);
				if (turnVal >= turnProbability) {
					// we're going to execute a turn
					ExecuteRandomTurn ();
				}
				quantSeen = 0;
			}
		}



		// The condition dropCount > 25 lets the cycle start moving before we start spawning.
		//If we are in a turn, we want to spawn more frequent, smaller objects.
		if (inTurn) {
			if (dropCount % 10 == 0 && dropCount > 25 && gameOver == false) {
				//SpawnTrailObject (inTurn);
				SpawnTrailObjectAlt(inTurn);
			}
		} else { //Every <interval> frames, make a block.
			if (dropCount % 15 == 0 && dropCount > 25 && gameOver == false) {
				//SpawnTrailObject (inTurn);
				SpawnTrailObjectAlt(inTurn);
			}
		}
		dropCount++;

		nextTime++;
		if (Random.Range (1, 350) == 1) {
			nextTime = 350;
		}
	}

	void ExecuteRandomTurn () {
		float turnChoice = Random.Range (0.0f, 1.0f);
		if (turnChoice > 0.5) {
			TurnLeft ();
		} else {
			TurnRight ();
		}
	}

	//Call it hacky, but this prevents our turning coroutines from being slightly off.
	void CorrectRotation() {
		if (playerTransform.eulerAngles.y > 267 && playerTransform.eulerAngles.y < 273) {
			Vector3 tempRotation = playerTransform.eulerAngles;
			tempRotation.y = 270;
			playerTransform.eulerAngles = tempRotation;
		}

		if (playerTransform.eulerAngles.y > 357 || playerTransform.eulerAngles.y < 3) {
			Vector3 tempRotation = playerTransform.eulerAngles;
			tempRotation.y = 0;
			playerTransform.eulerAngles = tempRotation;
		}

		if (playerTransform.eulerAngles.y > 87 && playerTransform.eulerAngles.y < 93) {
			Vector3 tempRotation = playerTransform.eulerAngles;
			tempRotation.y = 90;
			playerTransform.eulerAngles = tempRotation;
		}

		if (playerTransform.eulerAngles.y > 177 && playerTransform.eulerAngles.y < 183) {
			Debug.Log ("Correcting forward");
			Vector3 tempRotation = playerTransform.eulerAngles;
			tempRotation.y = 180;
			playerTransform.eulerAngles = tempRotation;
		}


	}

		

	float nfmod(float a, float b) {
		return a - b * Mathf.Floor(a / b);
	}

	/* perform a left turn */
	void TurnLeft () {
		StartCoroutine (PerformTurn(Vector3.down * 90, 0.5f));
		Cardboard.SDK.Recenter ();

		startingDir = (int)nfmod(startingDir - 1, 4);
		Debug.Log ("starting dir is now: " + startingDir);
	}

	/* perform a right turn */
	void TurnRight () {
		StartCoroutine (PerformTurn (Vector3.up * 90, 0.5f));
		Cardboard.SDK.Recenter ();
		startingDir = (int)nfmod(startingDir + 1, 4);
	}

	IEnumerator PerformTurn (Vector3 byAngles, float inTime) {
		inTurn = true;
		Quaternion fromAngle = playerTransform.rotation;
		Quaternion toAngle = Quaternion.Euler (playerTransform.eulerAngles + byAngles);

		for (float t = 0f; t < 1; t += Time.deltaTime / inTime) {
			playerTransform.rotation = Quaternion.Lerp (fromAngle, toAngle, t);
			yield return null;
		}
		CorrectRotation ();
		inTurn = false;

	}



	// SpawnTrailObjectAlt spawns trail objects right on the player position, but delays turning on their collider.
	void SpawnTrailObjectAlt(bool inTurn) {
		Vector3 tempScale = trailTurnObject.transform.localScale;
		Debug.Log ("my inturn is: " + inTurn);
		if (inTurn == false && dropCount % 15 == 0 && dropCount > 25) {
			if (startingDir == 0) {
				//trailTurnObject.x -= 6;
				tempScale.z = .8f;
				tempScale.x = 5;
				trailTurnObject.transform.localScale = tempScale;
			}
			if (startingDir == 1) {
				//trailTurnObject.z += 6;
				tempScale.z = 5;
				tempScale.x = .8f;
				trailTurnObject.transform.localScale = tempScale;
			}
			if (startingDir == 2) {
				//trailObjectPos.x += 6;
				tempScale.z = .8f;
				tempScale.x = 5;
				trailTurnObject.transform.localScale = tempScale;
			}
			if (startingDir == 3) {
				//trailObjectPos.z -= 6;
				tempScale.z = 5;
				tempScale.x = .8f;
				trailTurnObject.transform.localScale = tempScale;
			}
			Instantiate (trailTurnObject, playerTransform.position, Quaternion.identity);
		} else {
			tempScale.z = 1.5f;
			tempScale.x = 1.5f;
			trailTurnObject.transform.localScale = tempScale;
			Instantiate (trailTurnObject, playerTransform.position, Quaternion.identity);	
		}
	}

	public void Turn() {

		float turnAngle = devicePose.Orientation.eulerAngles.y;
		//Debug.Log(devicePose.Orientation.eulerAngles.y);
		int turnDirection = -1;
		if (turnAngle > 0 && turnAngle < 180) {
			turnDirection = 1;
		} 
		if (turnAngle >= 180 && turnAngle <= 360) { 
			turnAngle = 360 - turnAngle;
		}


		Quaternion turnQuaternion = playerTransform.localRotation;
		Debug.Log ("Player rotation is" + turnQuaternion);
		turnQuaternion.y += turnDirection * turnAngle;
		Debug.Log (turnQuaternion.y);
		playerTransform.localRotation = turnQuaternion ;
		Debug.Log ("Player rotation is" + playerTransform.localRotation + " after turn");

	}

	void OnTriggerEnter(Collider other) {
		AudioSource explosionSound = GetComponent <AudioSource>();
		Debug.Log ("feeling triggered");
		gameOver = true;
		GetComponent<Rigidbody>().isKinematic = true;
		GetComponent<Rigidbody>().detectCollisions = false;
		explosionSound.PlayOneShot (explodeClip);
		Instantiate (explosion, playerTransform.position, playerTransform.rotation);

		GetComponent<Renderer>().enabled = false;
		gameOverMenu.GetComponent<Renderer> ().enabled = true;
	}

}
