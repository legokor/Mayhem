using System;
using UnityEngine;

namespace Menus.Customization {
    /// <summary>
    /// A visual component of the player's ship.
    /// </summary>
    [AddComponentMenu("Menus / Customization / Attachment")]
    public class Attachment : MonoBehaviour {
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
            Attached = transform.parent == Customize.Instance.Body.transform;
            Counterpart = Instantiate(gameObject);
            Counterpart.transform.parent = transform;
            Destroy(Counterpart.GetComponent<Attachment>());
            if (Attached) {
                PlaceCounterpart();
                CreateCollider();
            }
        }

        public void Attach() {
            transform.parent = Customize.Instance.Body.transform;
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
            Transform Body = Customize.Instance.Body.transform;
            Vector3 Diff = Body.InverseTransformPoint(transform.position) * Body.localScale.x;
            Counterpart.transform.position = Body.position + Body.rotation * new Vector3(-Diff.x, Diff.y, Diff.z);
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
                Transform Body = Customize.Instance.Body.transform;
                if (Hit.collider.gameObject == Customize.Instance.Body || Hit.collider.transform.GetComponentInParent<Attachment>()) {
                    Vector3 Diff = Body.InverseTransformPoint(Hit.point) * Body.localScale.x;
                    transform.position = Hit.point;
                    transform.rotation = Quaternion.LookRotation(Body.forward, Hit.normal);
                    transform.localScale = new Vector3(Body.localScale.x + Convert.ToSingle(Diff.x < 0) * -2 * Body.localScale.x, Body.localScale.y, Body.localScale.z);
                    PlaceCounterpart();
                    if (LeapMouse.Instance.ActionDown())
                        Attach();
                } else {
                    transform.position = Counterpart.transform.position = Hit.point;
                    transform.rotation = Counterpart.transform.rotation = Quaternion.identity;
                    transform.localScale = Counterpart.transform.localScale = Body.localScale * .5f;
                }
            }
        }
    }
}