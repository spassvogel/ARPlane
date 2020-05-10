using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARMarkerReferenceManager : MonoBehaviour {
    public ARTrackedImageManager trackedImageManager;
    public Transform markers;

    ARSessionOrigin arSessionOrigin;
    Dictionary<string, Transform> markerIndex = new Dictionary<string, Transform>();

    public void Awake() {
        // Index positions for quick lookup
        foreach (Transform marker in markers) {
            markerIndex[marker.name] = marker;
        }
        arSessionOrigin = GetComponent<ARSessionOrigin>();
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs) {
        foreach(ARTrackedImage trackedImage in eventArgs.updated) {
            if (trackedImage.trackingState < TrackingState.Tracking) {
                continue;
            }
            string name = trackedImage.referenceImage.name;
            if(!markerIndex.ContainsKey(name)) {
                Debug.Log($"Found marker '{name}' but it was not found in the marker index.");
                continue;
            }
            Transform marker = markers.Find(name);
            if(marker == null) {
                Debug.Log($"Found marker '{name}' but it was not found in the Markers Transform.");
                continue;
            }
            Debug.Log($"SessionOrigin: {arSessionOrigin.transform.position}, found marker '{name}' at {trackedImage.transform.position} (real world position {marker.position}).");
            arSessionOrigin.MakeContentAppearAt(marker, trackedImage.transform.position, trackedImage.transform.localRotation);
        }
    }
}
