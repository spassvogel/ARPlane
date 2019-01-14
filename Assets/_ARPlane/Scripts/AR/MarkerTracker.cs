namespace UniversoAumentado.ARCraft.AR {
	using OpenCvSharp;

	using UnityEngine;
	using System.Collections;
	using System.Runtime.InteropServices;
	using System;
	using System.Collections.Generic;
	using UnityEngine.UI;
    using PaperPlaneTools.AR;

    public class MarkerTracker : WebCamera {
		[Tooltip("World offset")]
		public Transform offsetObject;
		[Tooltip("Player camera")]
		public Transform playerObject;
		[Tooltip("(0-1) Slows down AR tracking movement to stabilize position and rotation.")]
		public float stabilization = 0.8f;
		public float stabilizationThreshold = 1f;

		/// <summary>
		/// The marker detector
		/// </summary>
		private MarkerDetector markerDetector;

		private List<Listener> listeners = new List<Listener>();

		void Start () {
			Debug.Assert(offsetObject != null, "ARCamera: No offsetObject set.");
			Debug.Assert(playerObject != null, "ARCamera: No playerObject set.");
			markerDetector = new MarkerDetector ();
		}

		public void AddListener(Listener listener) {
			this.listeners.Add(listener);
		}

		protected override void ReadTextureConversionParameters() {
			base.ReadTextureConversionParameters();
#if UNITY_EDITOR
			// We DONT want it to flip!
			TextureParameters.FlipHorizontally = false;
#endif
		}

		protected override void Awake() {
			int cameraIndex = -1;
			for (int i = 0; i < WebCamTexture.devices.Length; i++) {
				WebCamDevice webCamDevice = WebCamTexture.devices [i];
				if (webCamDevice.isFrontFacing == false) {
					cameraIndex = i;
					break;
				}
				if (cameraIndex < 0) {
					cameraIndex = i;
				}
			}

			if (cameraIndex >= 0) {
				DeviceName = WebCamTexture.devices [cameraIndex].name;
			}
		}

		protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output) {
			Mat img = Unity.TextureToMat (input, TextureParameters);
			ProcessFrame(img, img.Cols, img.Rows);
			output = Unity.MatToTexture(img, output);
			return true;
		}

		private void ProcessFrame (Mat mat, int width, int height) {
			List<int> arucoIds = markerDetector.Detect (mat, width, height);
			if(arucoIds.Count == 0) {
				return;
			}
			FireMarkerDetected();

			Matrix4x4 transformMatrix = markerDetector.TransfromMatrixForIndex(0);
			PositionPlayer(transformMatrix);
		}


		private void PositionPlayer(Matrix4x4 transformMatrix) {
			Matrix4x4 matrixY = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, -1, 1));
			Matrix4x4 matrixZ = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, 1, -1));
			Matrix4x4 matrix = matrixZ * transformMatrix.inverse * matrixY;

			Vector3 targetPosition = MatrixHelper.GetPosition(matrix);
			Quaternion targetRotation = MatrixHelper.GetQuaternion(matrix);

			Quaternion playerRotation = playerObject.rotation;
			Quaternion errorRotation = targetRotation * Quaternion.Inverse(playerRotation);

			Vector3 playerPosition = playerObject.position;
			Vector3 errorPosition = targetPosition - playerPosition;

			// Move offset to counter error
			Vector3 move = (1-stabilization) * errorPosition;
			float distance = move.magnitude;
			offsetObject.position += move;

			// Rotate offset to counter error
			Quaternion offsetRotation = Quaternion.Lerp(Quaternion.identity, errorRotation, 1-stabilization) * offsetObject.rotation;
			float rotationAngle = Quaternion.Angle(offsetObject.rotation, offsetRotation);
			offsetObject.rotation = offsetRotation;

			if(distance < 0.1f * stabilizationThreshold && rotationAngle < 0.5f * stabilizationThreshold) {
				FireMarkerStable();
			}
		}

		private void FireMarkerDetected() {
			foreach(Listener listener in listeners) {
				listener.OnMarkerDetected();
			}
		}

		private void FireMarkerStable() {
			foreach(Listener listener in listeners) {
				listener.OnMarkerStabilized();
			}
		}

		public interface Listener {
			void OnMarkerDetected();
			void OnMarkerStabilized();
		}
	}
}
