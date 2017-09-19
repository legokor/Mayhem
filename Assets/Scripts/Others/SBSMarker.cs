using UnityEngine;

/// <summary>
/// Marks a camera (other than the main camera) for <see cref="SBS"/> handling.
/// </summary>
[RequireComponent(typeof(Camera))]
public class SBSMarker : MonoBehaviour {
    Camera Cam;

    void Awake() {
        SBS.AddCamera(Cam = GetComponent<Camera>());
    }

    void OnDestroy() {
        SBS.RemoveCamera(Cam);
    }
}