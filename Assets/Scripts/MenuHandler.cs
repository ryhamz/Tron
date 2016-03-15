using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour {
	private NetworkManager netManager;

	float timer = 5.0f;
	private bool showGUI = false;
	// Use this for initialization
	void Start () {
		netManager =GameObject.FindObjectOfType <NetworkManager> ();
		
	}
	
	// Update is called once per frame
	void Update () {
		if (showGUI == true) {
			timer -= Time.deltaTime;
		}
	}
		

	public void startSingleplayer() {
		StartCoroutine (singleLoader());
	}

	public void start2player() {
		StartCoroutine (multiplayerLoader());
	}

	public void resetScene() {
		//SceneManager.UnloadScene(SceneManager.GetActiveScene().buildIndex);
		StartCoroutine(reset ());

	}

	IEnumerator singleLoader () {
		showGUI = true;
		yield return new WaitForSeconds (timer);
		Application.LoadLevel ("scene1");
	}

	IEnumerator multiplayerLoader () {
		showGUI = true;
		yield return new WaitForSeconds (timer);
		Application.LoadLevel ("2player");
	}

	IEnumerator reset () {
		netManager.StopHost ();
		Application.LoadLevel ("2player");
		netManager.StartHost ();
		yield return new WaitForSeconds (1);

	}



	void OnGUI() {
		if (showGUI == true) {
			GUI.Box (new Rect (100, 175, 150, 100), "" + timer.ToString ("0"));
		}
	}
}
