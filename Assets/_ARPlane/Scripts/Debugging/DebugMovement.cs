using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversoAumentado.ARCraft.Debugging
{

	public class DebugMovement : MonoBehaviour {

		public float moveSpeed = 3;
		public float lookSpeed = 3;
		public bool logPosition = false;

		Vector3 rotation = Vector3.zero;

		// Use this for initialization
		void Start () {
			
		}

		// Update is called once per frame
		void Update () {
			#if UNITY_EDITOR
				Move();
				Look();
			#endif
			if (logPosition) {
				Debug.Log($"Camera: {transform.position}");
			}
		}


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
	}