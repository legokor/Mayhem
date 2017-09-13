using UnityEngine;

namespace Helpers {
    /// <summary>
    /// Handles the motion of a dropped pickup.
    /// </summary>
    [AddComponentMenu("Helpers / Drop Movement")]
    public class DropMovement : MonoBehaviour {
        /// <summary>
        /// Position offset direction.
        /// </summary>
        float Angle = 0;

        /// <summary>
        /// Get a position offset in the direction of the rotation vector.
        /// </summary>
        Vector3 GetPosForAngle() {
            return new Vector3(3 * Mathf.Sin(Angle), 0, 3 * Mathf.Cos(Angle));
        }

        /// <summary>
        /// Apply a random rotation on start.
        /// </summary>
        void Start() {
            transform.rotation = Quaternion.Euler(Random.value * 360, Random.value * 360, Random.value * 360);
        }

        /// <summary>
        /// Positioning and rotating.
        /// </summary>
        void Update() {
            // This part spins the object around an outside point for additional, better looking movement.
            Vector3 Last = GetPosForAngle();
            Angle += Mathf.PI * Time.deltaTime;
            transform.position += GetPosForAngle() - Last;
            // Rotate the object around.
            float RoundDelta = 360 * Time.deltaTime;
            transform.Rotate(new Vector3(RoundDelta, RoundDelta, RoundDelta));
        }
    }
}