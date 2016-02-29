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

	private int startingDir; //this will be mod 4. 0 is forward. 1 is right. 2 is back. 3 is left.

	Transform cameraTransform;
	Vector3 forceDirection;

	private GameObject currTrailObj;
	private float currTrailOrigin;
	void Awake() {
		forceDirection = playerTransform.right;
	}

	// Use this for initialization
	void Start () {
		
		devicePose = Cardboard.SDK.HeadPose;
		startingDir = 0;

		//TODO fix this shit
		Vector3 trailObjectPos = playerTransform.position;
		trailObjectPos.x -= 3.5f;
		currTrailObj = (GameObject) Instantiate (trailObject, trailObjectPos, Quaternion.identity); // The initial light trail.
		currTrailOrigin = currTrailObj.transform.position.x + 2.9f;


	}

	// Update is called once per frame
	void Update () {
		bikeBody.velocity = Vector3.zero;
		bikeBody.rotation = playerTransform.rotation;
		bikeBody.AddRelativeForce (Vector3.right * 1000);

		if (Input.GetKeyDown ("space")) {
			ChooseTurn ();
		}
			

	}

	int interval = 5;
	float nextTime = 0;
	void LateUpdate() {

		//TODO fix this shit too (or find a new method of doing it)
		// also want to refactor it into its own function...
		Vector3 currTrailScale = currTrailObj.transform.localScale;
		Vector3 currTrailPos = currTrailObj.transform.position;
		float distance = playerTransform.position.x - 2.5f - currTrailOrigin;
		Debug.Log (distance);
		if (nextTime % interval == 0) {
			currTrailPos.x += distance / 2f;
			currTrailObj.transform.position = currTrailPos;
			currTrailScale.x += distance / 2f;
			currTrailObj.transform.localScale = currTrailScale;
			currTrailOrigin = currTrailObj.transform.position.x + 2.9f;
		}
		/*
		 //Every <interval> frames, make a block.
		if (nextTime % interval == 0) {
			SpawnTrailObject ();
		}*/
		nextTime++;
	}

	void ChooseTurn () {
		float lookAngle = devicePose.Orientation.eulerAngles.y;

		if (lookAngle > 0 && lookAngle < 40)
			TurnRight ();
		else if (lookAngle > 40 && lookAngle < 360)
			TurnLeft ();
	}
		
	/* perform a left turn */
	void TurnLeft () {
		StartCoroutine (PerformTurn(Vector3.down * 90, 0.5f));
		Cardboard.SDK.Recenter ();
	}

	/* perform a right turn */
	void TurnRight () {
		StartCoroutine (PerformTurn (Vector3.up * 90, 0.5f));
		Cardboard.SDK.Recenter ();
	}

	IEnumerator PerformTurn (Vector3 byAngles, float inTime) {
		Quaternion fromAngle = playerTransform.rotation;
		Quaternion toAngle = Quaternion.Euler (playerTransform.eulerAngles + byAngles);

		for (float t = 0f; t < 1; t += Time.deltaTime / inTime) {
			playerTransform.rotation = Quaternion.Lerp (fromAngle, toAngle, t);
			yield return null;
		}
	}


	void SpawnTrailObject() {
		Vector3 trailObjectPos = playerTransform.position;
		if (startingDir == 0) {
			trailObjectPos.x -= 5;
		}
		if (startingDir == 1) {
			trailObjectPos.z += 5;
		}
		if (startingDir == 2) {
			trailObjectPos.x += 5;
		}
		if (startingDir == 3) {
			trailObjectPos.z -= 5;
		}
		Instantiate (trailObject, trailObjectPos, Quaternion.identity);
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

		GetComponent<Rigidbody>().isKinematic = true;
		GetComponent<Rigidbody>().detectCollisions = false;
		Instantiate (explosion, playerTransform.position, playerTransform.rotation);
	
		GetComponent<Renderer>().enabled = false;
	
	}
		
}
