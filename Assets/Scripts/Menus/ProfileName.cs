using UnityEngine;
using UnityEngine.UI;

namespace Menus {
    /// <summary>
    /// Handles profile name display.
    /// </summary>
    [AddComponentMenu("Menus / Profile Name")]
    public class ProfileName : MonoBehaviour {
        [Tooltip("Player name output field.")]
        public Text NameDisplay;

        /// <summary>
        /// Changes the displayed name on profile switch.
        /// </summary>
        void OnNameChanged() {
            NameDisplay.text = Profile.Username;
        }

        void Awake() {
            OnNameChanged();
        }

        void OnEnable() {
            Profile.OnProfileChanged += OnNameChanged;
        }

        void OnDisable() {
            Profile.OnProfileChanged -= OnNameChanged;
        }
    }
}