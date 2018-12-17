namespace UniversoAumentado.ARCraft.AR {
	using OpenCvSharp;

	using UnityEngine;
	using System.Collections;
	using System.Runtime.InteropServices;
	using System;
	using System.Collections.Generic;
	using UnityEngine.UI;
    using PaperPlaneTools.AR;

    public class ARCamera : WebCamera {
		public GameObject arGameObject;

		/// <summary>
		/// The marker detector
		/// </summary>
		private MarkerDetector markerDetector;

		void Start () {
			Debug.Assert(arGameObject != null, "ARCamera: No ARGameObject set.");
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
			PositionObject(arGameObject, transformMatrix);
		}

		private void PositionObject(GameObject gameObject, Matrix4x4 transformMatrix) {
			Matrix4x4 matrixY = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, -1, 1));
			Matrix4x4 matrixZ = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, new Vector3 (1, 1, -1));
			Matrix4x4 matrix = matrixY * transformMatrix * matrixZ;

			gameObject.transform.localPosition = MatrixHelper.GetPosition (matrix);
			gameObject.transform.localRotation = MatrixHelper.GetQuaternion (matrix);
			gameObject.transform.localScale = MatrixHelper.GetScale (matrix);
		}
	}
}
