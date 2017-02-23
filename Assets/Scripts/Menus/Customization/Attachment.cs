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
        bool Attached = false;

        /// <summary>
        /// Create the counterpart.
        /// </summary>
        void Start() {
            Counterpart = Instantiate(gameObject);
            Destroy(Counterpart.GetComponent<Attachment>());
        }

        public void Attach() {
            transform.parent = Customize.Instance.Body.transform;
            Attached = true;
        }

        public void Detach() {
            transform.parent = null;
            Attached = false;
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
                    bool Inversion = Diff.x > 0;
                    transform.position = Hit.point;
                    transform.rotation = Quaternion.LookRotation(Body.forward, Hit.normal);
                    transform.localScale = new Vector3(Inversion ? 1 : -1, 1, 1);
                    Counterpart.transform.position = Body.position + Body.rotation * new Vector3(-Diff.x, Diff.y, Diff.z);
                    Counterpart.transform.rotation = Quaternion.LookRotation(Body.forward, new Vector3(Hit.normal.x, -Hit.normal.y, -Hit.normal.z));
                    Counterpart.transform.localScale = new Vector3(Inversion ? 1 : -1, -1, 1);
                }
            }
        }
    }
}