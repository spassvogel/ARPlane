using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ARCraft : MonoBehaviour {

	public Transform target;
	public Transform cam;
	public float speed = 5f;
	private float forwardSpeedBias = 10f;
	private float rotateSpeed = .5f;

	public bool reverse = false;

	Rigidbody body;
	Vector3 previousPosition;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody>();
		previousPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		LookAtTarget();
		FollowTarget();
		Banking();

		previousPosition = transform.position;
	}

	void LookAtTarget() {
		Vector3 aim = (target.position - cam.position).normalized * 10;
		if(reverse) aim *= -1;
		Vector3 lookAtTarget = target.position + aim;

		lookAtTarget.y = transform.position.y;

        // The step size is equal to speed times frame time.
        float step = rotateSpeed * 10 * Time.deltaTime;
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookAtTarget - transform.position), step);
		
		Debug.DrawLine(transform.position, lookAtTarget, Color.green);
	}

	void FollowTarget() {
		if(target == null) return;

		float distance = Vector3.Distance(target.position, transform.position);
		Vector3 direction = (target.position - transform.position).normalized;
	
		// Attract
		body.AddForce(direction * distance * speed);

		// Attract more in Z-direction of target
		Vector3 fromTarget = target.InverseTransformDirection(target.position - transform.position);
		Vector3 force = target.TransformDirection(new Vector3(0, 0, fromTarget.z));
		body.AddForce(force * forwardSpeedBias);
	}

	void Banking() {
		// TODO: use rigidbody velocity
		Vector3 velocity = transform.InverseTransformDirection(transform.position - previousPosition) * transform.localScale.x / Time.deltaTime;

		// (weird) assumption/measurement: each dimension of velocity is max around 2, regardless of max speed
		float roll = -velocity.x * 45;
		float pitch = -velocity.y * 45;
		Vector3 currentRotation = transform.eulerAngles;

		transform.eulerAngles = new Vector3(pitch, currentRotation.y, roll);
	}
}
