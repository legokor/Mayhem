using UnityEngine;

namespace Menus.Customization {
    /// <summary>
    /// A visual component of the player's ship.
    /// </summary>
    [AddComponentMenu("Menus / Customization / Attachment")]
    public class Attachment : MonoBehaviour {
        public void Attach() {
            transform.parent = Customize.Instance.Body.transform;
        }
    }
}