using System;
using UnityEngine;

using LeapVR;

namespace Menus.Customization {
    /// <summary>
    /// A visual component of the player's ship.
    /// </summary>
    [AddComponentMenu("Menus / Customization / Attachment")]
    public class Attachment : MonoBehaviour {
        [Tooltip("The body to attach to.")]
        public GameObject Body;

        /// <summary>
        /// Collider of the attachment.
        /// </summary>
        MeshCollider BaseCollider, CounterpartCollider;
        /// <summary>
        /// Other side component.
        /// </summary>
        GameObject Counterpart;
        /// <summary>
        /// Attached to the body.
        /// </summary>
        bool Attached;
        /// <summary>
        /// Is there an attachment picked up?
        /// </summary>
        public static Attachment PickedUp { get; private set; }

        /// <summary>
        /// If there's a picked up attachment, destroy it.
        /// </summary>
        public static void DestroyPickedUp() {
            if (PickedUp)
                Destroy(PickedUp.gameObject);
        }

        /// <summary>
        /// Create the counterpart.
        /// </summary>
        void Start() {
#if UNITY_EDITOR
            if (!Body)
                Body = Customize.Instance.Body; // If the attachment was created outside the game code, set the correct body
#endif
            Attached = transform.parent == Body.transform;
            Counterpart = Instantiate(gameObject);
            Counterpart.transform.parent = Body.transform;
            Destroy(Counterpart.GetComponent<Attachment>());
            if (Attached) {
                PlaceCounterpart();
                CreateCollider();
            } else
                PickedUp = this;
        }

        /// <summary>
        /// Attach this component to the body and lock it in place.
        /// </summary>
        public void Attach() {
            transform.parent = Body.transform;
            CreateCollider();
            Attached = true;
            PickedUp = null;
        }

        /// <summary>
        /// Remove this component from the body for replacement or removal.
        /// </summary>
        public void Detach() {
            transform.parent = null;
            Attached = false;
            PickedUp = this;
            Destroy(BaseCollider);
            Destroy(CounterpartCollider);
        }

        /// <summary>
        /// Creates and sets up the colliders for both the base attachment and the counterpart.
        /// </summary>
        void CreateCollider() {
            BaseCollider = transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
            CounterpartCollider = Counterpart.transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
            BaseCollider.sharedMesh = CounterpartCollider.sharedMesh = GetComponentInChildren<MeshFilter>().sharedMesh;
        }

        /// <summary>
        /// Updates counterpart transform.
        /// </summary>
        void PlaceCounterpart() {
            Transform BodyT = Body.transform;
            Vector3 Diff = BodyT.InverseTransformPoint(transform.position) * BodyT.lossyScale.x;
            Counterpart.transform.position = BodyT.position + BodyT.rotation * new Vector3(-Diff.x, Diff.y, Diff.z);
            Vector3 EulerAngles = transform.eulerAngles;
            Counterpart.transform.eulerAngles = new Vector3(EulerAngles.x, EulerAngles.y, 180f - EulerAngles.z);
            Counterpart.transform.localScale = new Vector3(1, -1, 1);
        }

        /// <summary>
        /// Attachment placement and replacement.
        /// </summary>
        void Update() {
            RaycastHit Hit;
            // Selection and removal handling
            if (Attached) {
                bool ResetScaling = transform.localScale.x > 1.1f;
                if (!PickedUp && // No attachment is selected
                    !PlayerEntity.Instance && // Not ingame
                    Physics.Raycast(LeapMouse.ScreenPointToRay(), out Hit) && // Mouse is hovered part 1
                    (Hit.collider == BaseCollider || Hit.collider == CounterpartCollider)) { // Mouse is hovered part 2
                    float Upscale = 1f + Time.deltaTime * .25f;
                    transform.localScale *= Upscale;
                    Counterpart.transform.localScale *= Upscale;
                    if (LeapMouse.Instance.ActionDown()) {
                        Detach();
                        ResetScaling = true;
                    }
                } else
                    ResetScaling = true;
                if (ResetScaling) {
                    transform.localScale = new Vector3(1, 1, 1);
                    Counterpart.transform.localScale = new Vector3(1, -1, 1);
                }
                return;
            }
            // Snap to the body and placement
            if (Physics.Raycast(LeapMouse.ScreenPointToRay(), out Hit)) {
                Transform BodyT = Body.transform, CollisionParent = Hit.collider.transform.parent.parent;
                if (Hit.collider.gameObject == Body || (CollisionParent && CollisionParent == Body.transform)) {
                    Vector3 Diff = BodyT.InverseTransformPoint(Hit.point) * BodyT.localScale.x;
                    transform.position = Hit.point;
                    transform.rotation = Quaternion.LookRotation(BodyT.forward, Hit.normal);
                    transform.localScale = new Vector3(BodyT.localScale.x + Convert.ToSingle(Diff.x < 0) * -2 * BodyT.localScale.x,
                        BodyT.localScale.y, BodyT.localScale.z);
                    PlaceCounterpart();
                    if (LeapMouse.Instance.ActionDown())
                        Attach();
                } else {
                    transform.position = Counterpart.transform.position = Hit.point;
                    transform.rotation = Counterpart.transform.rotation = Quaternion.identity;
                    transform.localScale = Counterpart.transform.localScale = BodyT.localScale * .5f;
                }
            }
        }

        void OnDestroy() {
            Destroy(Counterpart);
        }
    }
}