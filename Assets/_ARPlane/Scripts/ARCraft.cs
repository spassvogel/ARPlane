//#define DEBUG

namespace UniversoAumentado.ARCraft {

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ARCraft : MonoBehaviour {

	public Transform target;
	public Transform cam;
	private float maxForce = 200f;
	private float maxForceDistance = 0.2f;
	private float forwardForceBias = 5f;
	private float rotateSpeed = .5f;
	private float stability = 30f;
	private float banking = 30f;
	private float lift = 500f;
	private float noseDownForce = 30f;
	private float artificialGravity = 250f;
	private float expectedMaxSpeed = 30f;

	private Vector3 previousVelocity;
	private float currentLift = 0f;

	public bool reverse = false;

	private float debugValue = 0f;

	Rigidbody body;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody>();
		previousVelocity = body.velocity;
    }
	
	// Update is called once per frame
	void Update () {
		currentLift = CalculateLift(body.velocity);

		LookAtTarget();
		FollowTarget();
		Banking();
		Stabilize();
		Gravity();

		previousVelocity = body.velocity;
	}

	float  CalculateLift(Vector3 velocity) {
		Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
		float magnitude = horizontalVelocity.magnitude;

		// Should move significantly (10%+) to get the plane started again
		if(magnitude < expectedMaxSpeed / 10f) magnitude = 0;

		return Mathf.Max(0, Mathf.Min(1, magnitude / expectedMaxSpeed));
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
		body.AddTorque(cross * angleDiff * rotateSpeed * currentLift);

		Debug.DrawLine(transform.position, lookAtTarget, Color.green);
	}

	void FollowTarget() {
		if(target == null) return;

		float distance = Vector3.Distance(target.position, transform.position);
		Vector3 direction = (target.position - transform.position).normalized;

        // Attract
		float distanceMultiplier = Mathf.Min(1, distance / maxForceDistance);
		Vector3 engineForce = direction * distanceMultiplier * maxForce;
        body.AddForce(engineForce);

		// Attract more in Z-direction of target
		Vector3 fromTarget = target.InverseTransformDirection(target.position - transform.position);
		Vector3 force = target.TransformDirection(new Vector3(0, 0, fromTarget.z));
		force.y = 0; // no force bias in y-direction
		body.AddForce(force * forwardForceBias);
	}

	void Banking() {
		Vector3 velocity = transform.InverseTransformDirection(body.velocity);
		Vector3 acceleration = velocity - transform.InverseTransformDirection(previousVelocity);

		debugValue = acceleration.x;
		body.AddRelativeTorque(Vector3.forward * (-acceleration.x) * banking);
	}

	void Stabilize() {
		Vector3 torqueVector = Vector3.Cross(transform.up, Vector3.up);
		body.AddTorque(torqueVector * stability);
	}

	void Gravity() {
		// Continuous gravity
		body.AddForce(Vector3.down * artificialGravity);

		// Lift to cancel out gravity
		Vector3 velocity = transform.InverseTransformDirection(body.velocity);
		velocity.z = 0;
		body.AddForce(Vector3.up * Mathf.Min(artificialGravity, currentLift * lift));
		
		// Continuous nose down
		Vector3 noseDown = Vector3.Cross(transform.forward, Vector3.down);
		body.AddTorque(noseDown * noseDownForce);
	}

#if DEBUG
    private void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle() { fontSize = 140 };
        guiStyle.normal.textColor = Color.red;
        GUI.Label(new Rect(100, 100, 100, 20), "Debug: " + Mathf.Round(debugValue), guiStyle);
    }
#endif
}

}