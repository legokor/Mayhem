using System.Text;
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
        /// The body's rotation at spawn.
        /// </summary>
        Quaternion StartRotation;
        /// <summary>
        /// Last screen position pointed to.
        /// </summary>
        Vector2 LastPointerPos;

        void Start() {
            StartRotation = Body.transform.rotation;
        }

        /// <summary>
        /// Saves the created ship.
        /// </summary>
        public void Serialize() {
            StringBuilder Serialization = new StringBuilder();
            int AttachmentCount = Body.transform.childCount;
            for (int Attachment = 0; Attachment < AttachmentCount; ++Attachment) {
                Transform ChildTransform = Body.transform.GetChild(Attachment);
                GameObject Child = Body.transform.GetChild(Attachment).gameObject;
                int Obj = 0;
                while (Child.name.StartsWith(Attachments[Obj].name))
                    ++Obj;
                Serialization.Append(Attachments[Obj].name).Append(";");
                Serialization.Append(ChildTransform.localPosition.x).Append(";");
                Serialization.Append(ChildTransform.localPosition.y).Append(";");
                Serialization.Append(ChildTransform.localPosition.z).Append(";");
                Serialization.Append(ChildTransform.localRotation.x).Append(";");
                Serialization.Append(ChildTransform.localRotation.y).Append(";");
                Serialization.Append(ChildTransform.localRotation.z).Append(";");
                Serialization.Append(ChildTransform.localScale.x).Append(";");
                Serialization.Append(ChildTransform.localScale.y).Append(";");
                Serialization.Append(ChildTransform.localScale.z).Append(";");
            }
            // TODO: save it
        }

        /// <summary>
        /// Loads the saved player ship.
        /// </summary>
        public void Deserialize() {
            // Remove anything that's currently attached to the ship.
            int Children = Body.transform.childCount;
            while (Children-- != 0)
                Destroy(Body.transform.GetChild(Children).gameObject);
            Body.transform.rotation = StartRotation; // Reset rotation.
            // TODO: load components
        }

        void Update() {
            Vector2 PointerPos = LeapMouse.Instance.ScreenPosition();
            if (LeapMouse.Instance.Action()) {
                Vector2 Difference = LastPointerPos - PointerPos;
                Body.transform.rotation = Quaternion.Euler(Camera.main.transform.up * Difference.x) *
                                          Quaternion.Euler(Camera.main.transform.right * Difference.y) * Body.transform.rotation;
                Vector3 EulerAngles = Body.transform.localEulerAngles;
                Body.transform.localEulerAngles = new Vector3(EulerAngles.x, EulerAngles.y, 0);
            }
            LastPointerPos = PointerPos;
        }

        private float RotateAroundAxis(GameObject objectToRotate, string joyAxisName, Vector3 vectorToRotateAround, float speed) {
            if (string.IsNullOrEmpty(joyAxisName)) {
                return 0;
            }

            var result = Input.GetAxis(joyAxisName) * speed * 60 * Time.deltaTime;
            objectToRotate.transform.rotation = Quaternion.Euler(vectorToRotateAround * result) * objectToRotate.transform.rotation;
            return result;
        }
    }
}