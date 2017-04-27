using UnityEngine;

using Helpers;

namespace Weapons {
    /// <summary>
    /// Weapon that fires a beam forward.
    /// </summary>
    [AddComponentMenu("Weapons / Laser")]
    public class Laser : WeaponBase {
        protected override void Setup() {
            _Kind = WeaponKinds.Laser;
            _DisplayName = "LASER";
            Cooldown = .0625f;
        }

        protected override void Shoot() {
            Projectile projectile = Instantiate(PlayerEntity.Instance.BeamEntity, transform.position + new Vector3(0, 0, 12), transform.rotation).GetComponent<Projectile>();
            projectile.Player = true;
            projectile.Damage = Level + 1;
            projectile.WeaponKind = WeaponKinds.Laser;
            projectile.transform.localScale = new Vector3(Level, 1, 1);
            PlayerEntity.Instance.PlaySound(PlayerEntity.Instance.AudioBeam, .5f);
        }
    }
}