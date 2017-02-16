using UnityEngine;

namespace Destructors {
    /// <summary>
    /// Destroy the object when it leaves the screen.
    /// </summary>
    public class OutOfScreenDespawner : MonoBehaviour {
        void Update() {
            Vector3 PositionOnScreen = Camera.main.WorldToViewportPoint(transform.position);
            if (PositionOnScreen.x < -1 || PositionOnScreen.x > 1 || PositionOnScreen.y < -1 || PositionOnScreen.y > 1) // -1 for shadow reasons
                Destroy(gameObject);
        }
    }
}