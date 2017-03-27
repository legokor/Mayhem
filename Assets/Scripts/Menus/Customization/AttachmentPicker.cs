using UnityEngine;

namespace Menus.Customization {
    /// <summary>
    /// A button to spawn an attachment.
    /// </summary>
    [AddComponentMenu("Menus / Customization / Attachment Picker")]
    public class AttachmentPicker : MonoBehaviour {
        [Tooltip("The attachment to create.")]
        public GameObject Attachment;
    }
}