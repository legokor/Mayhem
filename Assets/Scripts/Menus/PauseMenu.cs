using UnityEngine;

namespace Menus {
    /// <summary>
    /// Pause menu behaviour.
    /// </summary>
    [AddComponentMenu("Menus / Pause")]
    public class PauseMenu : MonoBehaviour {
        /// <summary>
        /// Cached time scale in case it's modified.
        /// </summary>
        float OldTimeScale = 1;

        /// <summary>
        /// Toggle pause menu.
        /// </summary>
        public void Toggle() {
            gameObject.SetActive(!gameObject.activeSelf);
        }

        /// <summary>
        /// Continue playing.
        /// </summary>
        public void Resume() {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Freeze time when this menu is shown.
        /// </summary>
        void OnEnable() {
            OldTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

        /// <summary>
        /// Unfreeze time when this menu is hidden.
        /// </summary>
        void OnDisable() {
            Time.timeScale = OldTimeScale;
        }
    }
}