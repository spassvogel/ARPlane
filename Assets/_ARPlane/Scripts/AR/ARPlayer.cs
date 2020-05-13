using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UniversoAumentado.ARCraft.AR {

public class ARPlayer : MonoBehaviour {

	public GameObject cursor;
	public ARCraft arCraft;
	
	Vector3 originalCursorPosition;
	Vector3 previousPosition = Vector3.zero;

	// Use this for initialization
	void Start () {
		if(cursor == null) {
			Debug.LogError("No Player cursor set.");
		}
		if(arCraft == null) {
			Debug.LogError("No ARCraft set.");
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
		arCraft.reverse = velocity.z < 0;
	}
}

}