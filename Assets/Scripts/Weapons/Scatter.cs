using UnityEngine;

using Helpers;

namespace Weapons {
    public class Scatter : WeaponBase {
        protected override void Setup() {
            _Kind = WeaponKinds.Scatter;
            _DisplayName = "SCATTER";
        }

        protected override void Shoot() {
            float Projectiles = Level * 3, Width = (Projectiles - 1) * -.75f, Angle = -10, AngleAddition = 20f / (Projectiles - 1);
            for (int i = 0; i < Projectiles; i++) {
                Projectile projectile = Instantiate(PlayerEntity.Instance.ProjectileEntity, transform.position + new Vector3(Width, 0, 12), Quaternion.Euler(0, Angle, 0))
                    .GetComponent<Projectile>();
                Angle += AngleAddition;
                Width += 1.5f;
                projectile.Player = true;
                projectile.Damage = 2;
                projectile.WeaponKind = WeaponKinds.Scatter;
                projectile.GetComponentInChildren<Renderer>().material.color = new Color(1, .5f, 0);
            }
            PlayerEntity.Instance.PlaySound(PlayerEntity.Instance.AudioScatter, .5f);
        }
    }
}