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
    
    public void ReferenceMarkerDetected(ARReferenceMarker marker) {

    }
}
