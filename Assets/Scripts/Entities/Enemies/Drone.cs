using UnityEngine;

using Helpers;
using Weapons;

namespace Enemies {
    /// <summary>
    /// Drone - rotates, moves in a wave, and shoots at the player.
    /// </summary>
    [AddComponentMenu("Entities / Enemies / Drone")]
    public class Drone : EnemyBase {
        public float RotationSpeed = 180f;
        public float ShakeIntensity = 5f;
        public float ShakeAmplitude = 5f;

        float LastX = 0;

        protected override void Creation() {
            WeaponKind = WeaponBase.RandomWeaponKind();
        }

        protected override void HandleSpecialMovement() {
            transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);
            float NewX = Mathf.Sin(ShakeIntensity * Time.time) * ShakeAmplitude;
            transform.position = transform.position + new Vector3(NewX - LastX, 0);
            LastX = NewX;
        }

        protected override Projectile Shoot() {
            Vector3 Direction = PlayerEntity.Instance.transform.position + new Vector3(0, 0,
                Mathf.Pow(Vector3.Distance(PlayerEntity.Instance.transform.position, transform.position), .775f)) - transform.position;
            return Instantiate(PlayerEntity.Instance.ProjectileEntity, transform.position, Quaternion.LookRotation(Direction)).GetComponent<Projectile>();
        }
    }
}