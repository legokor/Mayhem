using System;
using UnityEngine;

namespace Menus.Customization {
    /// <summary>
    /// A visual component of the player's ship.
    /// </summary>
    [AddComponentMenu("Menus / Customization / Attachment")]
    public class Attachment : MonoBehaviour {
        [Tooltip("The body to attach to.")]
        public GameObject Body;

        /// <summary>
        /// Other side component.
        /// </summary>
        GameObject Counterpart;
        /// <summary>
        /// Attached to the body.
        /// </summary>
        bool Attached;

        /// <summary>
        /// Create the counterpart.
        /// </summary>
        void Start() {
            Attached = transform.parent == Body.transform;
            Counterpart = Instantiate(gameObject);
            Counterpart.transform.parent = transform;
            Destroy(Counterpart.GetComponent<Attachment>());
            if (Attached) {
                PlaceCounterpart();
                CreateCollider();
            }
        }

        public void Attach() {
            transform.parent = Body.transform;
            CreateCollider();
            Attached = true;
        }

        public void Detach() {
            transform.parent = null;
            Attached = false;
            Destroy(transform.GetChild(0).GetComponent<MeshCollider>());
            Destroy(Counterpart.transform.GetChild(0).GetComponent<MeshCollider>());
        }

        void CreateCollider() {
            MeshCollider NewCollider = transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
            MeshCollider CounterCollider = Counterpart.transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
            NewCollider.sharedMesh = CounterCollider.sharedMesh = GetComponentInChildren<MeshFilter>().sharedMesh;
        }

        void PlaceCounterpart() {
            Transform BodyT = Body.transform;
            Vector3 Diff = BodyT.InverseTransformPoint(transform.position) * BodyT.lossyScale.x;
            Counterpart.transform.position = BodyT.position + BodyT.rotation * new Vector3(-Diff.x, Diff.y, Diff.z);
            Vector3 EulerAngles = transform.eulerAngles;
            Counterpart.transform.eulerAngles = new Vector3(EulerAngles.x, EulerAngles.y, 180f - EulerAngles.z);
            Counterpart.transform.localScale = new Vector3(1, -1, 1);
        }

        /// <summary>
        /// Snap to the body.
        /// </summary>
        void Update() {
            if (Attached)
                return;
            RaycastHit Hit;
            if (Physics.Raycast(LeapMouse.ScreenPointToRay(), out Hit)) {
                Transform BodyT = Body.transform;
                if (Hit.collider.gameObject == Body || Hit.collider.transform.GetComponentInParent<Attachment>()) {
                    Vector3 Diff = BodyT.InverseTransformPoint(Hit.point) * BodyT.localScale.x;
                    transform.position = Hit.point;
                    transform.rotation = Quaternion.LookRotation(BodyT.forward, Hit.normal);
                    transform.localScale = new Vector3(BodyT.localScale.x + Convert.ToSingle(Diff.x < 0) * -2 * BodyT.localScale.x, BodyT.localScale.y, BodyT.localScale.z);
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
    }
}