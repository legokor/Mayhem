using UnityEngine;

using Helpers;

namespace Enemies {
    public class Fighter : EnemyBase {
        public float NoseOffset = 10f;

        protected override void HandleSpecialMovement() {
            transform.rotation = Quaternion.Euler(0, 180, -Movement.x * .5f);
        }

        protected override Projectile Shoot() {
            return Instantiate(PlayerEntity.Instance.ProjectileEntity, transform.position + transform.forward * NoseOffset, Quaternion.Euler(0, 180, 0)).GetComponent<Projectile>();
        }
    }
}