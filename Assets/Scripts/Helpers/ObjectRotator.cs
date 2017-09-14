using UnityEngine;

namespace Helpers {
    /// <summary>
    /// Rotates an object.
    /// </summary>
    [AddComponentMenu("Helpers / Object Rotator")]
    public class ObjectRotator : MonoBehaviour {
        /// <summary>
        /// Rotation speed multiplier.
        /// </summary>
        public float Speed = 1;

        [Header("Axes")]
        public bool X, Y = true, Z;

        /// <summary>
        /// Rotating.
        /// </summary>
        void Update() {
            Vector3 Angles = transform.eulerAngles;
            float Addition = Time.deltaTime * Speed;
            if (X)
                Angles.x += Addition;
            if (Y)
                Angles.y += Addition;
            if (Z)
                Angles.z += Addition;
            transform.eulerAngles = Angles;
        }
    }
}