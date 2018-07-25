using Helpers;
using UnityEngine;

/// <summary>
/// Destroy the object if no music is available.
/// </summary>
public class DestroyIfNoMusic : MonoBehaviour {
    void Awake() {
        if (RandomMusic.Instance && RandomMusic.Instance.Files.Length != 0)
            Destroy(this);
        else
            Destroy(gameObject);
    }
}