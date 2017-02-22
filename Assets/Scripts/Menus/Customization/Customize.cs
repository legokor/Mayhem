using UnityEngine;

namespace Menus.Customization {
    /// <summary>
    /// Allows attaching objects to the player's ship.
    /// </summary>
    [AddComponentMenu("Menus / Customization / Customize")]
    public class Customize : Singleton<Customize> {
        [Tooltip("The ship's main body to build on.")]
        public GameObject Body;
        [Tooltip("Possible components to attach.")]
        public GameObject[] Attachments;

        /// <summary>
        /// Saves the created ship.
        /// </summary>
        public void Serialize() {
            // TODO: save components
        }

        /// <summary>
        /// Loads the saved player ship.
        /// </summary>
        public void Deserialize() {
            // Remove anything that's currently attached to the ship.
            int Children = Body.transform.childCount;
            while (Children-- != 0)
                Destroy(Body.transform.GetChild(Children).gameObject);
            // TODO: load components
        }
    }
}