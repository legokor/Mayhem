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
            if (Attached)
                PlaceCounterpart();
        }

        public void Attach() {
            transform.parent = Customize.Instance.Body.transform;
            Attached = true;
        }

        public void Detach() {
            transform.parent = null;
            Attached = false;
        }

        void PlaceCounterpart() {
            Transform Body = Customize.Instance.Body.transform;
            Vector3 Diff = Body.InverseTransformPoint(transform.position) * Body.localScale.x;
            Counterpart.transform.position = Body.position + Body.rotation * new Vector3(-Diff.x, Diff.y, Diff.z);
            Counterpart.transform.rotation = Quaternion.LookRotation(Body.forward, new Vector3(transform.up.x, -transform.up.y, -transform.up.z));
            Counterpart.transform.localScale = new Vector3(1 + Convert.ToSingle(Diff.x < 0) * -2, -1, 1);
        }

        /// <summary>
        /// Snap to the body.
        /// </summary>
        void Update() {
            if (Attached)
                return;
            RaycastHit Hit;
            if (Physics.Raycast(LeapMouse.ScreenPointToRay(), out Hit)) {
                if (Hit.collider.gameObject == Customize.Instance.Body) {
                    Transform Body = Customize.Instance.Body.transform;
                    Vector3 Diff = Body.InverseTransformPoint(Hit.point) * Body.localScale.x;
                    transform.position = Hit.point;
                    transform.rotation = Quaternion.LookRotation(Body.forward, Hit.normal);
                    transform.localScale = new Vector3(1 + Convert.ToSingle(Diff.x < 0) * -2, 1, 1);
                    PlaceCounterpart();
                    if (LeapMouse.Instance.ActionDown())
                        Attach();
                } else {
                    transform.position = Counterpart.transform.position = Hit.point;
                    transform.rotation = Counterpart.transform.rotation = Quaternion.identity;
                    transform.localScale = Counterpart.transform.localScale = new Vector3(.5f, .5f, .5f);
                }
            }
        }
    }
}