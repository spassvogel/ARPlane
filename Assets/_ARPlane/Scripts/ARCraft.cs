using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ARCraft : MonoBehaviour {

	public Transform target;
	public Transform cam;
	public float speed = 10f;
	private float forwardSpeedBias = 10f;
	private float rotateSpeed = .5f;
	private float stability = 5f;

	public bool reverse = false;

	Rigidbody body;
	Vector3 previousPosition;
	Vector3 previousVelocity;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody>();
		previousPosition = transform.position;
		previousVelocity = body.velocity;
	}
	
	// Update is called once per frame
	void Update () {
		LookAtTarget();
		FollowTarget();
		Banking();
		Stabilize();

		previousPosition = transform.position;
		previousVelocity = body.velocity;
	}

	void LookAtTarget() {
		// Find target position to look at
		Vector3 aim = (target.position - cam.position).normalized * 10;
		if(reverse) aim *= -1;
		Vector3 lookAtTarget = target.position + aim;
		Vector3 targetDelta = lookAtTarget - transform.position;
 
		// Get the angle between transform.forward and target delta
		float angleDiff = Vector3.Angle(transform.forward, targetDelta);
 
		// Get its cross product, which is the axis of rotation to
		// Get from one vector to the other
		Vector3 cross = Vector3.Cross(transform.forward, targetDelta);
 
		// Apply torque along that axis according to the magnitude of the angle.
		body.AddTorque(cross * angleDiff * rotateSpeed);

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
		Vector3 velocity = transform.InverseTransformDirection(body.velocity);
		Vector3 acceleration = transform.InverseTransformDirection(previousVelocity);

		body.AddRelativeTorque(Vector3.forward * acceleration.x * stability * 0.1f);
	}

	void Stabilize() {
		Vector3 torqueVector = Vector3.Cross(transform.up, Vector3.up);
		body.AddTorque(torqueVector * stability);
	}
}
