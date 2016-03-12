using UnityEngine;
using System.Collections;

public class MenuHandler : MonoBehaviour {
	float timer = 5.0f;
	private bool showGUI = false;
	// Use this for initialization
	void Start () {
		
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

	IEnumerator singleLoader () {
		showGUI = true;
		yield return new WaitForSeconds (timer);
		Application.LoadLevel ("scene1");
	}

	void OnGUI() {
		if (showGUI == true) {
			GUI.Box (new Rect (100, 175, 150, 100), "" + timer.ToString ("0"));
		}
	}
}
