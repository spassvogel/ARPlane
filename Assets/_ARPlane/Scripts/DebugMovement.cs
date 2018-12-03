using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMovement : MonoBehaviour {

	public float moveSpeed = 3;
	public float lookSpeed = 3;

	Vector3 rotation = Vector3.zero;

	// Use this for initialization
	void Start () {
		
	}

#if UNITY_EDITOR
	// Update is called once per frame
	void Update () {
		Move();
		Look();
	}
#endif

	void Move() {
        var y = Input.GetKey(KeyCode.Space) ? Time.deltaTime * moveSpeed : 0;
		y -= Input.GetKey(KeyCode.C) ? Time.deltaTime * moveSpeed : 0;

		var x = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        transform.Translate(x, y, z);
	}

	void Look() {
		rotation.y += Input.GetAxis ("Mouse X");
		rotation.x += -Input.GetAxis ("Mouse Y");
		transform.eulerAngles = (Vector2)rotation * lookSpeed;
	}
}
