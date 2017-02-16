using UnityEngine;

namespace Helpers {
    /// <summary>
    /// Move the given GameObject to a set place smoothly.
    /// </summary>
    [AddComponentMenu("Helpers / Lerp to Place")]
    public class LerpToPlace : MonoBehaviour {
        public GameObject Target;
        public float Speed = 5f;
        [Tooltip("Skip the initial lag, especially on scene loading, as it may finish the entire movement.")]
        public float Skip = .25f;

        void Update() {
            if (Skip > 0) {
                Skip -= Time.deltaTime;
                return;
            }
            float LerpSpeed = Speed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, Target.transform.position, LerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, Target.transform.rotation, LerpSpeed);
        }
    }
}