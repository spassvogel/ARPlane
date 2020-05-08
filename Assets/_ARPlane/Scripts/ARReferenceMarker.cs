using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARReferenceMarker : MonoBehaviour {
    ARTrackedImage marker;
    ARMarkerReferenceManager manager;

    void Start() {
        marker = GetComponent<ARTrackedImage>();

        if(marker == null) {
            Debug.Log("ARReferenceMarker spawned without ARTrackedImage. Cannot calibrate based on this.");
            Destroy(gameObject);
            return;
        }

        GameObject managerGO = GameObject.FindGameObjectWithTag("ARReferenceManager");
        if(managerGO == null) {
            Debug.LogError("No GameObject found with tag 'ARReferenceManager'. Cannot calibrate position.");
            Destroy(gameObject);
            return;
        }

        manager = managerGO.GetComponent<ARMarkerReferenceManager>();
        if(manager == null) {
            Debug.LogError("GameObject with tag 'ARReferenceManager' did not have ARMarkerReferenceManager component. Cannot calibrate position.");
            Destroy(gameObject);
            return;
        }
    }

    // Update is called once per frame
    void Update() {
        if(marker.trackingState != TrackingState.Tracking) {
            return;
        }

        manager.CalibratePosition(this);
    }

    public string GetName() {
        return marker.referenceImage.name;
    }
}
