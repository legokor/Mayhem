using UnityEngine;

using Destructors;
using Helpers;

namespace Pickups {
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    [RequireComponent(typeof(DropMovement), typeof(OOSBottomDespawner))]
    public abstract class PickupBase : MonoBehaviour {
        public AudioClip PickupSound;

        protected abstract void OnPickup();

        public void PickedUp() {
            OnPickup();
            PlayerEntity.Instance.PlaySound(PickupSound, 1, true);
            Destroy(gameObject);
        }

        void Awake() {
            GetComponent<Collider>().isTrigger = true;
            GetComponent<Rigidbody>().useGravity = false;
        }
    }
}