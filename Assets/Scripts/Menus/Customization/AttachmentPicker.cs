using UnityEngine;

namespace Menus.Customization {
    /// <summary>
    /// A button to spawn an attachment.
    /// </summary>
    [AddComponentMenu("Menus / Customization / Attachment Picker")]
    public class AttachmentPicker : MonoBehaviour {
        [Tooltip("The attachment to create.")]
        public GameObject Attachment;

        [Tooltip("Local position of the spawned object.")]
        public Vector3 Offset;

        [Tooltip("Local rotation of the spawned object.")]
        public Vector3 RotationOffset = new Vector3(22.5f, 90f, 0);

        [Tooltip("Local scale of the spawned object.")]
        public float Scale = .5f;

        /// <summary>
        /// The object above the button.
        /// </summary>
        GameObject Icon;

        /// <summary>
        /// Called when the button is clicked, spawns the attachment.
        /// </summary>
        public void Pick() {
            if (!Customization.Attachment.PickedUp) {
                GameObject NewInstance = Instantiate(Attachment);
                NewInstance.GetComponent<Attachment>().Body = Customize.Instance.Body;
                NewInstance.GetComponentInChildren<Renderer>().material = Customize.Instance.GetMaterial();
            }
        }

        void Start() {
            Icon = Instantiate(Attachment, transform.position - transform.forward * transform.localScale.z + Offset * Scale, transform.rotation * Quaternion.Euler(RotationOffset));
            float ActualScale = transform.localScale.y * Scale;
            Icon.transform.localScale = new Vector3(ActualScale, ActualScale, ActualScale);
            Destroy(Icon.GetComponent<Attachment>());
        }
    }
}