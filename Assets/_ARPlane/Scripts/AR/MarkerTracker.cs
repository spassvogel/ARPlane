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
		public Transform offsetObject;
		public Transform playerObject;

		/// <summary>
		/// The marker detector
		/// </summary>
		private MarkerDetector markerDetector;

		void Start () {
			Debug.Assert(offsetObject != null, "ARCamera: No offsetObject set.");
			Debug.Assert(playerObject != null, "ARCamera: No playerObject set.");
			markerDetector = new MarkerDetector ();
		}

		protected override void ReadTextureConversionParameters() {
			// We DONT want it to flip!
			base.ReadTextureConversionParameters();
#if UNITY_EDITOR
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

			Matrix4x4 transformMatrix = markerDetector.TransfromMatrixForIndex(0);
			PositionPlayer(transformMatrix);
		}

		private void PositionPlayer(Matrix4x4 transformMatrix) {
			Matrix4x4 matrixY = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, -1, 1));
			Matrix4x4 matrixZ = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, 1, -1));
			Matrix4x4 matrix = matrixY * transformMatrix * matrixZ;

			Vector3 markerPosition = MatrixHelper.GetPosition(matrix);
			Quaternion markerRotation = MatrixHelper.GetQuaternion(matrix);

			// First fix rotation error because it can mess up the position

			Quaternion playerRotation = playerObject.transform.rotation;
			// The player rotation should be the inverse of the marker rotation.
			Quaternion targetRotation = Quaternion.Inverse(markerRotation);
			Quaternion errorRotation = targetRotation * Quaternion.Inverse(playerRotation);
			
			Vector3 mRotation = markerRotation.eulerAngles;
			Vector3 tRotation = targetRotation.eulerAngles;
			Vector3 eRotation = errorRotation.eulerAngles;
			Vector3 fRotation = (errorRotation * playerRotation).eulerAngles;

			// Rotate offset to counter error
			offsetObject.transform.rotation = errorRotation * offsetObject.transform.rotation;

			// Now fix position error

			Vector3 playerPosition = playerObject.position;
			// The marker is placed at some point from the origin. We want the marker to be the origin, so player should be (0,0,0) - markerPosition.
			Vector3 targetPosition = Vector3.zero - markerPosition;
			Vector3 errorPosition = targetPosition - playerPosition;

			// Move offset to counter error
			offsetObject.transform.position += errorPosition;

		}
	}
}
