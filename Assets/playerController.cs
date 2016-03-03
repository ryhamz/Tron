using UnityEngine;
using System.Collections;


//Controls player movement and trail generation.

public class playerController : MonoBehaviour {

	public GameObject explosion;
	public Transform playerTransform;
	public Pose3D devicePose;
	public Rigidbody bikeBody;
	public Cardboard mainCamera;
	public GameObject trailObject;
	public GameObject trailTurnObject;

	private int startingDir; //this will be mod 4. 0 is forward. 1 is right. 2 is back. 3 is left.
	private bool inTurn;

	Transform cameraTransform;
	Vector3 forceDirection;

	private GameObject currTrailObj;
	private float currTrailOrigin;
	private bool gameOver;


	void Awake() {
		forceDirection = playerTransform.right;
	}

	// Use this for initialization
	void Start () {
		gameOver = false;
		devicePose = Cardboard.SDK.HeadPose;
		startingDir = 0;
		inTurn = false;



	}

	// Update is called once per frame
	void Update () {
		bikeBody.velocity = Vector3.zero;
		bikeBody.rotation = playerTransform.rotation;
		bikeBody.AddRelativeForce (Vector3.right * 500);

		if (Input.GetKeyDown ("space") && inTurn == false) {
			ChooseTurn ();
		}
			

	}

	//Call it hacky, but this prevents our turning coroutines from being slightly off.
	void CorrectRotation() {
		if (playerTransform.eulerAngles.y > 268 && playerTransform.eulerAngles.y < 272) {
			Vector3 tempRotation = playerTransform.eulerAngles;
			tempRotation.y = 270;
			playerTransform.eulerAngles = tempRotation;
		}

		if (playerTransform.eulerAngles.y > 358 || playerTransform.eulerAngles.y < 2) {
			Vector3 tempRotation = playerTransform.eulerAngles;
			tempRotation.y = 0;
			playerTransform.eulerAngles = tempRotation;
		}

		if (playerTransform.eulerAngles.y > 88 && playerTransform.eulerAngles.y < 92) {
			Vector3 tempRotation = playerTransform.eulerAngles;
			tempRotation.y = 90;
			playerTransform.eulerAngles = tempRotation;
		}

		if (playerTransform.eulerAngles.y > 178 && playerTransform.eulerAngles.y < 182) {
			Debug.Log ("Correcting forward");
			Vector3 tempRotation = playerTransform.eulerAngles;
			tempRotation.y = 180;
			playerTransform.eulerAngles = tempRotation;
		}


	}

	int interval = 35;
	float nextTime = 0;
	void LateUpdate() {

		// The condition nextTime > 25 lets the cycle start moving before we start spawning.
		//If we are in a turn, we want to spawn more frequent, smaller objects.
		if (inTurn) {
			if (nextTime % 15 == 0 && nextTime > 25 && gameOver == false) {
				//SpawnTrailObject (inTurn);
				SpawnTrailObjectAlt(inTurn);
			}
		} else { //Every <interval> frames, make a block.
			if (nextTime % interval == 0 && nextTime > 25 && gameOver == false) {
				//SpawnTrailObject (inTurn);
				SpawnTrailObjectAlt(inTurn);
			}
		}
		nextTime++;
	}

	void ChooseTurn () {
		float lookAngle = devicePose.Orientation.eulerAngles.y;

		if (lookAngle > 0 && lookAngle < 40)
			TurnRight ();
		else if (lookAngle > 40 && lookAngle < 360)
			TurnLeft ();
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

	//TODO possibly remove this. Replacement for SpawnTrailObject is below.
	void SpawnTrailObject(bool inTurn) {
		if (inTurn == false) {
			Vector3 trailObjectPos = playerTransform.position;
			Vector3 tempScale = trailObject.transform.localScale;
			if (startingDir == 0) {
				trailObjectPos.x -= 6;
				tempScale.z = .8f;
				tempScale.x = 5;
				trailObject.transform.localScale = tempScale;
			}
			if (startingDir == 1) {
				trailObjectPos.z += 6;
				tempScale.z = 5;
				tempScale.x = .8f;
				trailObject.transform.localScale = tempScale;
			}
			if (startingDir == 2) {
				trailObjectPos.x += 6;
				tempScale.z = .8f;
				tempScale.x = 5;
				trailObject.transform.localScale = tempScale;
			}
			if (startingDir == 3) {
				trailObjectPos.z -= 6;
				tempScale.z = 5;
				tempScale.x = .8f;
				trailObject.transform.localScale = tempScale;
			}
			Instantiate (trailObject, trailObjectPos, Quaternion.identity);
		} else {
			Instantiate (trailTurnObject, playerTransform.position, Quaternion.identity);

		}
	}

	// SpawnTrailObjectAlt spawns trail objects right on the player position, but delays turning on their collider.
	void SpawnTrailObjectAlt(bool inTurn) {
		Vector3 tempScale = trailTurnObject.transform.localScale;
		if (inTurn == false && nextTime % interval == 0 && nextTime > 25) {
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
		Debug.Log ("feeling triggered");
		gameOver = true;
		GetComponent<Rigidbody>().isKinematic = true;
		GetComponent<Rigidbody>().detectCollisions = false;
		Instantiate (explosion, playerTransform.position, playerTransform.rotation);
	
		GetComponent<Renderer>().enabled = false;
	
	}
		
}
