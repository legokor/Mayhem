using UnityEngine;

using Helpers;
using Weapons;

namespace Enemies {
    /// <summary>
    /// Turret - shoots at the player from the ground. Spawns in the scenery.
    /// </summary>
    [AddComponentMenu("Entities / Enemies / Turret")]
    public class Turret : EnemyBase {
        [Tooltip("The turret head object that looks at the player.")]
        public GameObject TurretHead;

        protected override void Creation() {
            WeaponKind = WeaponBase.RandomWeaponKind();
        }

        protected override void HandleSpecialMovement() {
            TurretHead.transform.LookAt(PlayerEntity.Instance.transform.position);
        }

        protected override Projectile[] Shoot() {
            float RotRad = TurretHead.transform.eulerAngles.y * Mathf.Deg2Rad;
            Vector3 ShotPosition = new Vector3(TurretHead.transform.position.x + Mathf.Sin(RotRad) * 10, 25, TurretHead.transform.position.z + Mathf.Cos(RotRad) * 10);
            Vector3 Direction = PlayerEntity.Instance.transform.position + new Vector3(0, 0,
                Mathf.Pow(Vector3.Distance(PlayerEntity.Instance.transform.position, ShotPosition), .775f)) - ShotPosition;
            return new Projectile[] { CreateProjectile(ShotPosition, Quaternion.LookRotation(Direction)) };
        }

        protected override Vector3 LootSpawnPosition() {
            return new Vector3(transform.position.x, PlayerEntity.Instance.transform.position.y, transform.position.z);
        }
    }
}