using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPlayer : MonoBehaviour {

	public GameObject cursor;
	
	Vector3 originalCursorPosition;
	Vector3 previousPosition = Vector3.zero;
	bool goingBackwards = false;

	// Use this for initialization
	void Start () {
		if(cursor == null) {
			Debug.LogError("No Player cursor set.");
		} else {
			originalCursorPosition = cursor.transform.position;
		}
	}
	
	// Update is called once per frame
	void Update () {
		UpdateCursor();

		previousPosition = transform.position;
	}

	void UpdateCursor() {
		if(cursor == null) return;

		Vector3 velocity = transform.InverseTransformDirection(transform.position - previousPosition) * transform.localScale.x / Time.deltaTime;
		
		// Reverse
		if(!goingBackwards && velocity.z < -10) {
			goingBackwards = true;
			// TODO
		}
		// Going forward again
		if(goingBackwards && velocity.z >= 10) {
			goingBackwards = false;
			// TODO
		}
	}
}
