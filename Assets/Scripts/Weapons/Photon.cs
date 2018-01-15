using UnityEngine;

using Helpers;

namespace Weapons {
    /// <summary>
    /// Weapon that shoots projectiles forward.
    /// </summary>
    [AddComponentMenu("Weapons / Photon")]
    public class Photon : WeaponBase {
        protected override void Setup() {
            _Kind = WeaponKinds.Photon;
            _DisplayName = "PHOTON";
        }

        protected override void Shoot() {
            float Width = (Level - 1) * -1.25f;
            for (int i = 0; i < Level; i++) {
                Projectile projectile = Instantiate(PlayerEntity.Instance.ProjectileEntity, transform.position + new Vector3(Width, 0, 12), transform.rotation)
                    .GetComponent<Projectile>();
                Width += 2.5f;
                projectile.Player = true;
                projectile.Damage = 4; // DPS: level * 40
                projectile.WeaponKind = WeaponKinds.Photon;
                projectile.Repaint(Color.green);
                ParticleSystem.MainModule ParticleMain = projectile.GetComponent<ParticleSystem>().main;
                ParticleMain.startSizeXMultiplier *= 2;
            }
            PlayerEntity.Instance.PlaySound(PlayerEntity.Instance.AudioPhoton);
        }
    }
}