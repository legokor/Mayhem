using UnityEngine;

namespace Pickups {
    [AddComponentMenu("Entities / Pickups / Weapon")]
    public class Weapon : PickupBase {
        public WeaponKinds GivenWeapon;

        protected override void OnPickup() {
            PlayerEntity.Instance.WeaponPickup(GivenWeapon);
        }
    }
}