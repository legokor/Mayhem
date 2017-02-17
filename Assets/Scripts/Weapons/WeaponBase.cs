using UnityEngine;

/// <summary>
/// Possible weapons and equipment states.
/// </summary>
public enum WeaponKinds {
    Unassigned = -1,
    Photon,
    Scatter,
    Laser,
    Maximum, // Keep this entry for the random selector to work
}

namespace Weapons {
    /// <summary>
    /// Weapon base class.
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour {
        [Tooltip("Weapon level.")]
        [Range(1, 5)]
        public int Level = 1;
        [Tooltip("Experience points for the current level.")]
        [Range(0, 25)]
        public int XP = 0;
        [Tooltip("Is it firing?")]
        public bool Firing = false;

        /// <summary>
        /// Weapon kind to be set in Setup.
        /// </summary>
        protected WeaponKinds _Kind = WeaponKinds.Unassigned;
        /// <summary>
        /// Weapon kind getter.
        /// </summary>
        public WeaponKinds Kind { get { return _Kind; } }
        /// <summary>
        /// Display color getter.
        /// </summary>
        public Color DisplayColor { get { return WeaponKindColor(Kind); } }
        /// <summary>
        /// Display name to be set in Setup.
        /// </summary>
        protected string _DisplayName = "BOGIMOZI";
        /// <summary>
        /// Display name getter.
        /// </summary>
        public string DisplayName { get { return _DisplayName; } }
        /// <summary>
        /// Time between shots.
        /// </summary>
        protected float Cooldown = .1f;
        /// <summary>
        /// The time when the weapon can shoot again.
        /// </summary>
        float NextShot = 0;

        /// <summary>
        /// Add a weapon to a GameObject.
        /// </summary>
        /// <param name="Kind">Weapon kind</param>
        /// <param name="Parent">Target GameObject</param>
        /// <returns>The new weapon component on the target GameObject</returns>
        public static WeaponBase AttachWeapon(WeaponKinds Kind, GameObject Parent) {
            switch (Kind) {
                case WeaponKinds.Photon: return Parent.AddComponent<Photon>();
                case WeaponKinds.Scatter: return Parent.AddComponent<Scatter>();
                case WeaponKinds.Laser: return Parent.AddComponent<Laser>();
                default: return Parent.AddComponent<Unassigned>();
            }
        }

        /// <summary>
        /// Select a random valid weapon.
        /// </summary>
        /// <returns>A random usable weapon</returns>
        public static WeaponKinds RandomWeaponKind() {
            return (WeaponKinds)Random.Range(0, (int)WeaponKinds.Maximum);
        }

        /// <summary>
        /// Get the display color of a given weapon.
        /// </summary>
        /// <param name="Kind">Weapon kind</param>
        /// <returns>Display color of the given weapon</returns>
        public static Color WeaponKindColor(WeaponKinds Kind) {
            switch (Kind) {
                case WeaponKinds.Photon: return new Color(0, .75f, 0);
                case WeaponKinds.Scatter: return new Color(.75f, .375f, 0);
                case WeaponKinds.Laser: return new Color(.75f, 0, 0);
                default: return Color.white;
            }
        }

        /// <summary>
        /// Increase weapon level.
        /// </summary>
        public void AddLevel() {
            if (Level != 5) {
                ++Level;
                XP = 0;
            }
        }

        /// <summary>
        /// Increase weapon experience.
        /// </summary>
        public void AddExperience() {
            if (Level != 5 && ++XP >= 25) {
                ++Level;
                XP = 0;
            }
        }

        /// <summary>
        /// Setup protected variables and weapon-specific initialization.
        /// </summary>
        protected abstract void Setup();

        /// <summary>
        /// Shooting action.
        /// </summary>
        protected abstract void Shoot();

        /// <summary>
        /// Call Setup on creation.
        /// </summary>
        void Awake() {
            Setup();
        }

        /// <summary>
        /// Shoot if able to.
        /// </summary>
        void Update() {
            if (Time.time >= NextShot && Firing) {
                Shoot();
                NextShot = Time.time + Cooldown;
            }
        }
    }

    /// <summary>
    /// Empty weapon class.
    /// </summary>
    public class Unassigned : WeaponBase {
        protected override void Setup() { }
        protected override void Shoot() { }
    }
}