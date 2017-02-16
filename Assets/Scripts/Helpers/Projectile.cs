using UnityEngine;

namespace Helpers {
    /// <summary>
    /// An object capable of causing damage.
    /// </summary>
    [AddComponentMenu("Helpers / Projectile")]
    public class Projectile : MonoBehaviour {
        [Tooltip("Was it created by the player?")]
        public bool Player = false;
        [Tooltip("Initial velocity in m/s.")]
        public float Speed = 200;
        [Tooltip("Disappearance timer. If given, there won't be any movement.")]
        public float DestroyIn = 0;
        [Tooltip("Damage to deal.")]
        public int Damage = 0;
        [Tooltip("Weapon kind.")]
        public WeaponKinds WeaponKind = 0;

        void Update() {
            if (DestroyIn != 0) {
                DestroyIn -= Time.deltaTime;
                if (DestroyIn <= 0)
                    Destroy(gameObject);
            } else
                transform.position += transform.forward * Time.deltaTime * Speed;
        }
    }
}