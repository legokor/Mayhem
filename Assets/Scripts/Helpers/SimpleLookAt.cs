using UnityEngine;

namespace Helpers {
    /// <summary>
    /// Makes an object look at another.
    /// </summary>
    [AddComponentMenu("Helpers / Simple Look At")]
    public class SimpleLookAt : MonoBehaviour {
        [Tooltip("The object to look at.")]
        public Transform Target;

        void Update() {
            transform.LookAt(Target.position);
        }
    }
}