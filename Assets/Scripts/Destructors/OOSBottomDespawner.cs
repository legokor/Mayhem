using UnityEngine;

namespace Destructors {
    /// <summary>
    /// Removes the object when it leaves the screen to the bottom.
    /// </summary>
    public class OOSBottomDespawner : MonoBehaviour {
        void Update() {
            Vector3 PositionOnScreen = Camera.main.WorldToViewportPoint(transform.position);
            if (PositionOnScreen.x < -1 || PositionOnScreen.y < -1)
                Destroy(gameObject);
        }
    }
}