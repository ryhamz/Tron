using UnityEngine;
using System.Collections;

public class colliderDelay : MonoBehaviour {


	private BoxCollider bCollider;
	// Use this for initialization
	void Start () {
		bCollider = GetComponent<BoxCollider> ();
		bCollider.enabled = false;
	}
	
	// Update is called once per frame
	int counter = 0;
	void Update () {
		if (counter > 80) {
			bCollider.enabled = true;
			bCollider.isTrigger = true;
		}
		counter++;
	}
}
