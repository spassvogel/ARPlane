using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[System.Serializable]
public class ARReferencePosition {
    public string markerName;
    public Vector3 position;
}

public class ARMarkerReferenceManager : MonoBehaviour {
    public ARReferencePosition[] realWorldPositions;
    Dictionary<string, Vector3> realworldPositionsIndex = new Dictionary<string, Vector3>();

    public void Start() {
        // Index positions for quick lookup
        foreach(ARReferencePosition reference in realWorldPositions) {
            realworldPositionsIndex[reference.markerName] = reference.position;
        }
    }

    public void CalibratePosition(ARReferenceMarker marker) {
        string markerName = marker.GetName();
        if(!realworldPositionsIndex.ContainsKey(markerName)) {
            Debug.LogError($"Marker '{markerName}' not found as reference.");
            return;
        }

        // We know the position where we found the marker and we know where the marker should be located, so now we fix that difference
        Vector3 realWorldPosition = realworldPositionsIndex[markerName];
        Vector3 currentPosition = marker.transform.position;
        Vector3 difference = currentPosition - realWorldPosition;

        transform.Translate(difference);

        Debug.Log($"Found marker at {currentPosition}, should be {realWorldPosition}, changed to {marker.transform.position}. Session origin is now at {transform.position}.");
    }
}
