//#define DEBUG

namespace UniversoAumentado.ARCraft {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Net;
	using UnityEngine;

	[RequireComponent(typeof(Rigidbody))]
	public class ARCraft : MonoBehaviour {

		public Transform target;
		public Transform cam;
		public GameObject bombPrefab;
		public DebugTexts debugTexts;

		private float maxForce = 500;
		private float maxForceDistance = 25f;
		private float forwardForceBias = 5f;
		private float rotateSpeed = .5f;
		private float stability = 30f;
		private float banking = 30f;
		private float lift = 500f;
		private float noseDownForce = 30f;
		private float artificialGravity = 250f;
		private float expectedMaxSpeed = 200f;

		public bool reverse = false;

		private Vector3 previousVelocity;
		private float currentLift = 0f;
		private float horizontalDistanceToTarget = 0;
		private float altitude = Mathf.Infinity;

		Rigidbody body;

		// Use this for initialization
		void Start () {
			body = GetComponent<Rigidbody>();
			previousVelocity = body.velocity;
		}
	
		// Update is called once per frame
		void Update () {
			CalculateValues();
			LookAtTarget();
			FollowTarget();
			Banking();
			Stabilize();
			Gravity();
			HandleInput();
			CalculateAltitude();

			previousVelocity = body.velocity;
		}

		void CalculateValues() {
			currentLift = CalculateLift(body.velocity);
			horizontalDistanceToTarget = Vector2.Distance(
				new Vector2(target.position.x, target.position.z),
				new Vector2(transform.position.x, transform.position.z)
			);
			DebugText("ARCraft:", $"{transform.position}");
			DebugText("Target:", $"{target.position}");
		}

		void CalculateAltitude() {
			RaycastHit hit;
			altitude = Mathf.Infinity;
			if(Physics.Raycast(transform.position, Vector3.down, out hit)) {
				altitude = hit.distance;
			}
			DebugText("Altitude:", $"{altitude}");
		}

		float CalculateLift(Vector3 velocity) {
			float horizontalSpeed = new Vector2(velocity.x, velocity.z).magnitude;

			// Should move significantly (10%+) to get the plane started again
			if (horizontalSpeed < expectedMaxSpeed / 10f) horizontalSpeed = 0;

			return Mathf.Max(0, Mathf.Min(1, horizontalSpeed / expectedMaxSpeed));
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
			// get from one vector to the other
			Vector3 cross = Vector3.Cross(transform.forward, targetDelta);
 
			// Apply torque along that axis according to the magnitude of the angle.
			body.AddTorque(cross * angleDiff * rotateSpeed * currentLift);

			Debug.DrawLine(transform.position, lookAtTarget, Color.green);
		}

		void FollowTarget() {
			if(target == null) return;

			Vector3 direction = (target.position - transform.position).normalized;

			// Attract
			float throttle = Mathf.Min(1, horizontalDistanceToTarget / maxForceDistance);
			Vector3 engineForce = direction * throttle * maxForce;
			body.AddForce(engineForce);

			DebugText("Horiz. Distance To Target:", $"{horizontalDistanceToTarget}");
			DebugText("Throttle:", $"{throttle}");

			// Attract more in Z-direction of target
			Vector3 fromTarget = target.InverseTransformDirection(target.position - transform.position);
			Vector3 force = target.TransformDirection(new Vector3(0, 0, fromTarget.z));
			force.y = 0; // no force bias in y-direction
			body.AddForce(force * forwardForceBias);
		}

		void Banking() {
			Vector3 velocity = transform.InverseTransformDirection(body.velocity);
			Vector3 acceleration = velocity - transform.InverseTransformDirection(previousVelocity);

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
			body.AddForce(Vector3.up * Mathf.Min(artificialGravity, currentLift * lift));

			// Continuous nose down (except when landing)
			if (altitude > 5) {
				Vector3 noseDown = Vector3.Cross(transform.forward, Vector3.down);
				body.AddTorque(noseDown * noseDownForce);
			}
		}

		void HandleInput() {
			if (Input.GetMouseButtonDown(0)) {
				DropBomb();
			}
		}

		void DropBomb() {
			Debug.Log("Dropping bomb!");
			GameObject bomb = Instantiate(bombPrefab);

			// Spawn the bomb slightly below the plane to avoid collision
			bomb.transform.position = transform.position + new Vector3(0, -3, 0);
		}

		void DebugText(string name, string value) {
			if (debugTexts == null) return;
			debugTexts.SetDebugText(name, value);
		}
	}

}