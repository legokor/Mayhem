using System;
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
        /// Copy of the components for ingame use.
        /// </summary>
        public static GameObject[] AttachmentCopies;
        /// <summary>
        /// The body's rotation at spawn.
        /// </summary>
        Quaternion StartRotation;
        /// <summary>
        /// Last screen position pointed to.
        /// </summary>
        Vector2 LastPointerPos;

        void Start() {
            AttachmentCopies = (GameObject[])Attachments.Clone();
            StartRotation = Body.transform.rotation;
        }

        /// <summary>
        /// Converts a given number to a character array, without using 0 as any of those characters.
        /// </summary>
        static char[] SerializeFloat(float x) {
            char[] Out = new char[5];
            // First 4 bytes: actual string
            byte[] Bits = BitConverter.GetBytes(x);
            Out[0] = (char)Bits[0]; Out[1] = (char)Bits[1]; Out[2] = (char)Bits[2]; Out[3] = (char)Bits[3];
            // Last byte: what to replace zero to - anything that's not used (something from 1 to 5)
            Out[4] = (char)1;
            while (Bits[0] == Out[4] || Bits[1] == Out[4] || Bits[2] == Out[4] || Bits[3] == Out[4])
                ++Out[4];
            // Replace zeros with the new value that's representing zero
            for (int i = 0; i < 4; ++i)
                if (Out[i] == (char)0)
                    Out[i] = Out[4];
            return Out;
        }

        /// <summary>
        /// Converts the results of SerialzeFloat back to floats.
        /// </summary>
        static float DeserializeFloat(string x) {
            byte[] OriginalFloat = new byte[4];
            for (int i = 0; i < 4; ++i)
                OriginalFloat[i] = x[i] == x[4] ? (byte)0 : (byte)x[i];
            return BitConverter.ToSingle(OriginalFloat, 0);
        }

        /// <summary>
        /// Saves the created ship.
        /// </summary>
        public void Serialize() {
            StringBuilder Serialization = new StringBuilder();
            int AttachmentCount = Body.transform.childCount;
            for (int Attachment = 0; Attachment < AttachmentCount; ++Attachment) {
                Transform ChildTransform = Body.transform.GetChild(Attachment);
                if (ChildTransform.GetComponent<Attachment>()) {
                    if (ChildTransform.localEulerAngles.x < 0)
                        continue;
                    GameObject Child = ChildTransform.gameObject;
                    int Obj = 0;
                    while (!Child.name.StartsWith(AttachmentCopies[Obj].name))
                        ++Obj;
                    Serialization.Append(AttachmentCopies[Obj].name).Append(";");
                    Vector3 LocalPos = ChildTransform.localPosition, Angles = ChildTransform.localEulerAngles;
                    Serialization.Append(SerializeFloat(LocalPos.x)).Append(";");
                    Serialization.Append(SerializeFloat(LocalPos.y)).Append(";");
                    Serialization.Append(SerializeFloat(LocalPos.z)).Append(";");
                    Serialization.Append(SerializeFloat(Angles.x)).Append(";");
                    Serialization.Append(SerializeFloat(Angles.y)).Append(";");
                    Serialization.Append(SerializeFloat(Angles.z)).Append(";");
                }
            }
            PlayerPrefs.SetString("Ship", Serialization.ToString());
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Remove anything that's currently attached to the ship.
        /// </summary>
        public void Cleanup() {
            int Children = Body.transform.childCount;
            while (Children-- != 0)
                Destroy(Body.transform.GetChild(Children).gameObject);
        }

        /// <summary>
        /// Loads the saved player ship on a given GameObject.
        /// </summary>
        public static void DeserializeTo(GameObject Target) {
            string[] Ship = PlayerPrefs.GetString("Ship", "").Split(';');
            int ShipPos = 0, MaxPos = Ship.Length;
            while (MaxPos - ShipPos >= 7) {
                string Name = Ship[ShipPos++];
                int Obj = 0, Objs = AttachmentCopies.Length;
                while (!AttachmentCopies[Obj].name.Equals(Name))
                    if (++Obj >= Objs)
                        break;
                Vector3 Position = new Vector3(DeserializeFloat(Ship[ShipPos++]), DeserializeFloat(Ship[ShipPos++]), DeserializeFloat(Ship[ShipPos++]));
                Vector3 EulerAngles = new Vector3(DeserializeFloat(Ship[ShipPos++]), DeserializeFloat(Ship[ShipPos++]), DeserializeFloat(Ship[ShipPos++]));
                GameObject Attached = Instantiate(AttachmentCopies[Obj]);
                Attached.GetComponent<Attachment>().Body = Target;
                Transform AttachmentTransform = Attached.transform;
                AttachmentTransform.parent = Target.transform;
                AttachmentTransform.localPosition = Position;
                AttachmentTransform.localEulerAngles = EulerAngles;
                AttachmentTransform.localScale = new Vector3(1, 1, 1);
            }
        }

        /// <summary>
        /// Loads the saved player ship in the menu.
        /// </summary>
        public void Deserialize() {
            Body.transform.rotation = StartRotation; // Reset rotation.
            Cleanup();
            DeserializeTo(Body);
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
    }
}