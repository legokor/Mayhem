using UnityEngine;

using Helpers;

namespace Enemies {
    /// <summary>
    /// Boss - stays on screen until defeated.
    /// </summary>
    [AddComponentMenu("Entities / Enemies / Boss")]
    public class Boss : EnemyBase {
        public Transform[] WeaponLocations;

        float TimeAlive = 0;

        protected override void HandleSpecialMovement() {
            transform.position = new Vector3(Mathf.Sin(TimeAlive * Mathf.PI * .15f) * PlayerEntity.AreaWMax, PlayerEntity.Instance.transform.position.y,
                MapHandler.Instance.MapPos + Mathf.Lerp(PlayerEntity.AreaHMax * 3, PlayerEntity.AreaHMax - 3 * transform.localScale.x, TimeAlive));
            TimeAlive += Time.deltaTime;
        }

        protected override Projectile[] Shoot() {
            Projectile[] Shots = new Projectile[WeaponLocations.Length];
            for (int i = 0, c = WeaponLocations.Length; i < c; ++i)
                Shots[i] = Instantiate(PlayerEntity.Instance.ProjectileEntity, WeaponLocations[i].position, WeaponLocations[i].rotation).GetComponent<Projectile>();
            return Shots;
        }
    }
}