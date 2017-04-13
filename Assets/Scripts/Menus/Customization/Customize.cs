using System;
using System.Text;
using UnityEngine;

namespace Menus.Customization {
    /// <summary>
    /// Allows attaching objects to the player's ship.
    /// </summary>
    [AddComponentMenu("Menus / Customization / Customize")]
    public class Customize : Singleton<Customize> {
        [Tooltip("Ship rotation sensitivity.")]
        public float Sensitivity = 1f;
        [Tooltip("The ship's main body to build on.")]
        public GameObject Body;
        [Tooltip("Possible components to attach.")]
        public GameObject[] Attachments;
        [Tooltip("Selectable colors for the ship.")]
        public Material[] Colors;

        /// <summary>
        /// Copy of the components for ingame use.
        /// </summary>
        public static GameObject[] AttachmentCopies;
        /// <summary>
        /// Copy of colors for ingame use.
        /// </summary>
        public static Material[] ColorCopies;
        /// <summary>
        /// The material to use from the Colors array.
        /// </summary>
        int SelectedColor = 0;
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
            ColorCopies = (Material[])Colors.Clone();
            StartRotation = Body.transform.rotation;
            SelectedColor = PlayerPrefs.GetInt("ShipColor", 0);
        }

        public Material GetMaterial() {
            return Colors[SelectedColor];
        }

        public static void ApplyColorTo(int ColorID, GameObject Target) {
            Material Appliable = ColorCopies[ColorID];
            Renderer[] Renderers = Target.GetComponentsInChildren<Renderer>();
            int RendererCount = Renderers.Length;
            for (int i = 0; i < RendererCount; ++i)
                Renderers[i].material = Appliable;
        }

        public void SelectColor(int ColorID) {
            ApplyColorTo(ColorID, Body);
            SelectedColor = ColorID;
        }

        /// <summary>
        /// Converts a given number to a character array, without using 0 as any of those characters.
        /// </summary>
        static char[] SerializeFloat(float x) {
            char[] Out = new char[6];
            // First 4 bytes: float data
            byte[] Bits = BitConverter.GetBytes(x);
            for (int i = 0; i < 4; ++i) Out[i] = (char)Bits[i];
            // Last bytes: what to replace zero and ';' to - anything that's not used (something from 0 to 4 and A to D)
            Out[4] = '0'; // Offset for zero character
            Out[5] = 'A'; // Offset for ';' character
            while (Bits[0] == Out[4] || Bits[1] == Out[4] || Bits[2] == Out[4] || Bits[3] == Out[4])
                ++Out[4];
            while (Bits[0] == Out[5] || Bits[1] == Out[5] || Bits[2] == Out[5] || Bits[3] == Out[5])
                ++Out[5];
            // Replace zeros with the new values that represent zero and ';'
            for (int i = 0; i < 4; ++i) {
                if (Out[i] == 0) Out[i] = Out[4];
                if (Out[i] == ';') Out[i] = Out[5];
            }
            return Out;
        }

        /// <summary>
        /// Converts the results of SerialzeFloat back to floats.
        /// </summary>
        static float DeserializeFloat(string x) {
            byte[] OriginalFloat = new byte[4];
            for (int i = 0; i < 4; ++i) {
                if (x[i] == x[4]) OriginalFloat[i] = 0;
                else if (x[i] == x[5]) OriginalFloat[i] = (byte)';';
                else OriginalFloat[i] = (byte)x[i];
            }
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
            PlayerPrefs.SetInt("ShipColor", SelectedColor);
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
            ApplyColorTo(PlayerPrefs.GetInt("ShipColor", 0), Target);
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
                Vector2 Difference = (LastPointerPos - PointerPos) * Sensitivity;
                Body.transform.rotation = Quaternion.Euler(Camera.main.transform.up * Difference.x) *
                                          Quaternion.Euler(Camera.main.transform.right * Difference.y) * Body.transform.rotation;
                Vector3 EulerAngles = Body.transform.localEulerAngles;
                Body.transform.localEulerAngles = new Vector3(EulerAngles.x, EulerAngles.y, 0);
            }
            LastPointerPos = PointerPos;
        }
    }
}