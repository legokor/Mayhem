using UnityEngine;

using Cavern;

namespace Helpers {
    /// <summary>
    /// Picks and plays a random sound from the given array.
    /// It should only be used on objects without an AudioSource3D component.
    /// </summary>
    [AddComponentMenu("Helpers / Random Sound")]
    [RequireComponent(typeof(AudioSource3D))]
    public class RandomSound : MonoBehaviour {
        [Tooltip("Clips to pick a random from.")]
        public AudioClip[] Clips;

        /// <summary>
        /// Get the required source and pick a random clip to play it on.
        /// </summary>
        void Start() {
            AudioSource3D Source = GetComponent<AudioSource3D>();
            Source.clip = Clips[Random.Range(0, Clips.Length)];
            Source.Play();
        }
    }
}