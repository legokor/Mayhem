﻿using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

using LeapVR;

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
        [Tooltip("The text displaying remaining part tokens.")]
        public Text TokensText;
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
        /// <summary>
        /// Hand distance in the last frame.
        /// </summary>
        Vector2 LastScale;
        /// <summary>
        /// The body's position at spawn.
        /// </summary>
        Vector3 StartPosition;

        /// <summary>
        /// Available part tokens.
        /// </summary>
        int Tokens {
            get { return _Tokens; }
            set { TokensText.text = "Part tokens: " + (_Tokens = value); }
        }
        int _Tokens;

        void Awake() {
            AttachmentCopies = (GameObject[])Attachments.Clone();
            ColorCopies = (Material[])Colors.Clone();
            StartPosition = Body.transform.position;
            StartRotation = Body.transform.rotation;
            SelectedColor = Profile.GetInt("ShipColor", 0);
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
        /// Is it possible to pick up a new attachment from the UI?
        /// </summary>
        public bool Pick() {
            if (!Attachment.PickedUp && Tokens > 0) {
                --Tokens;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called when an attachment were removed.
        /// </summary>
        public void OnRemove() {
            ++Tokens;
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
            Profile.SetString("Ship", Serialization.ToString());
            Profile.SetInt("ShipColor", SelectedColor);
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
        public static int DeserializeTo(GameObject Target) {
            string[] Ship = Profile.GetString("Ship", "").Split(';');
            int ShipPos = 0, MaxPos = Ship.Length, Components = 0;
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
                ++Components;
            }
            ApplyColorTo(Profile.GetInt("ShipColor", 0), Target);
            return Components;
        }

        /// <summary>
        /// Loads the saved player ship in the menu.
        /// </summary>
        public void Deserialize() {
            Body.transform.position = StartPosition;
            Body.transform.rotation = StartRotation;
            Cleanup();
            Tokens = Profile.Tokens - DeserializeTo(Body);
        }

        void Update() {
            Vector2 PointerPos = LeapMouse.Instance.ScreenPositionUnclamped(), HandsDist = PointerPos - LeapMotion.Instance.PalmOnScreenXYUnclamped(1);
            if (LeapMouse.Instance.Action()) {
                if (LeapMotion.Instance.GetHandCount() > 1 && LeapMotion.Instance.ExtendedFingers(1) == 0) {
                    float Difference = (HandsDist.magnitude - LastScale.magnitude) * Sensitivity;
                    Vector3 Direction = (Camera.main.transform.position - Body.transform.position).normalized;
                    Body.transform.position += Direction * (Difference * 3 / Screen.width);
                } else { // Rotate
                    Vector2 Difference = (LastPointerPos - PointerPos) * Sensitivity;
                    Body.transform.rotation = Quaternion.Euler(Camera.main.transform.up * Difference.x) *
                                              Quaternion.Euler(Camera.main.transform.right * Difference.y) * Body.transform.rotation;
                    Vector3 EulerAngles = Body.transform.localEulerAngles;
                    Body.transform.localEulerAngles = new Vector3(EulerAngles.x, EulerAngles.y, 0);
                }
            }
            LastPointerPos = PointerPos;
            LastScale = HandsDist;
        }
    }
}