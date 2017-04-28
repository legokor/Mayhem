using UnityEngine;

using Helpers;

namespace Weapons {
    /// <summary>
    /// Weapon that fires a beam forward.
    /// </summary>
    [AddComponentMenu("Weapons / Laser")]
    public class Laser : WeaponBase {
        /// <summary>
        /// As a beam must be displayed each frame, a different cooldown must be used here.
        /// </summary>
        float ActualCooldown = 0;

        protected override void Setup() {
            _Kind = WeaponKinds.Laser;
            _DisplayName = "LASER";
            Cooldown = 0;
        }

        protected override void Shoot() {
            Projectile projectile = Instantiate(PlayerEntity.Instance.BeamEntity, transform.position + new Vector3(0, 0, 12), transform.rotation).GetComponent<Projectile>();
            projectile.Player = true;
            ActualCooldown -= Time.deltaTime;
            if (ActualCooldown <= 0) {
                ActualCooldown += .0625f; // The beam actually hits 16 times a second
                projectile.Damage = Level + 1;
            } else
                projectile.Damage = 0;
            projectile.WeaponKind = WeaponKinds.Laser;
            projectile.transform.localScale = new Vector3(Level, 1, 1);
            PlayerEntity.Instance.PlaySound(PlayerEntity.Instance.AudioBeam, .5f);
        }
    }
}