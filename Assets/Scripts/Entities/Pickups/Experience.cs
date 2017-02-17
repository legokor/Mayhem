using UnityEngine;

namespace Pickups {
    [AddComponentMenu("Entities / Pickups / Experience")]
    public class Experience : PickupBase {
        protected override void OnPickup() {
            PlayerEntity.Instance.AwardExperience();
        }
    }
}