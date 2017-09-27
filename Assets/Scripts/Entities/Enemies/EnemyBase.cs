using UnityEngine;

using Cavern;

using Helpers;
using Menus;
using Weapons;

namespace Enemies {
    /// <summary>
    /// Enemy base class. Handles common enemy features, such as shooting and drops.
    /// </summary>
    [RequireComponent(typeof(AudioSource3D))]
    public abstract class EnemyBase : MonoBehaviour {
        [Tooltip("Each shot that hits the player removes this much health.")]
        public int Damage = 3;
        [Tooltip("The damage this enemy can take before dying.")]
        public int Health = 10;
        [Tooltip("Movement direction.")]
        public Vector2 Movement = new Vector2(0, 5);
        [Tooltip("The weapon kind this enemy is using.")]
        public WeaponKinds WeaponKind = WeaponKinds.Unassigned;
        [Tooltip("How fast this enemy can shoot. The time between shots in seconds."), Range(.1f, 5f)]
        public float ShootingSpeed = .5f;
        [Tooltip("The rare loot this unit can drop.")]
        public GameObject[] RareDrops;

        /// <summary>Is u ded?</summary>
        bool Dead = false;
        /// <summary>Time until the next shot.</summary>
        float Cooldown;
        /// <summary>Audio source component.</summary>
        AudioSource3D Source;

        /// <summary>
        /// Called when the enemy is spawned.
        /// </summary>
        protected virtual void Creation() { }

        /// <summary>
        /// Called each frame for moving the enemy.
        /// </summary>
        protected virtual void HandleSpecialMovement() { }

        /// <summary>
        /// Returns a projectile which this kind of enemy shoots.
        /// </summary>
        protected abstract Projectile Shoot();

        /// <summary>
        /// Returns where the loot should spawn if the enemy is killed.
        /// </summary>
        protected virtual Vector3 LootSpawnPosition() {
            return transform.position;
        }

        /// <summary>
        /// When the enemy is spawned, set it up.
        /// </summary>
        void Start() {
            Damage = MapHandler.Instance.EnemyDamage;
            Health = MapHandler.Instance.EnemyHealth;
            Source = GetComponent<AudioSource3D>();
            if (!Settings.HQAudio)
                Source.Mute = true;
            Creation(); // Enemy specific setup
            Cooldown = ShootingSpeed; // Initial cooldown
        }

        /// <summary>
        /// Called each frame.
        /// </summary>
        void Update() {
            transform.position -= new Vector3(Movement.x * Time.deltaTime, 0, Movement.y * Time.deltaTime); // Generic movement
            HandleSpecialMovement(); // Enemy specific movement
            // Handle shooting
            if (WeaponKind != WeaponKinds.Unassigned) { // Only when the weapon is assigned
                Cooldown -= Time.deltaTime;
                if (Cooldown <= 0 && Camera.main.WorldToViewportPoint(transform.position).y > 0) {
                    Projectile projectile = Shoot();
                    projectile.Damage = Damage;
                    projectile.Speed = 75;
                    projectile.WeaponKind = WeaponKind;
                    projectile.Repaint(WeaponBase.WeaponKindColor(WeaponKind));
                    if (!Source.Mute)
                        Source.Play();
                    Cooldown += ShootingSpeed;
                }
            }
        }

        /// <summary>
        /// Called when the enemy has hit something.
        /// </summary>
        /// <param name="col">Collider of the hit GameObject.</param>
        void OnTriggerEnter(Collider col) {
            if (Dead) // Don't calculate this again for already dead enemies
                return;
            Vector3 PositionOnScreen = Camera.main.WorldToViewportPoint(transform.position);
            if (PositionOnScreen.x < 0 || PositionOnScreen.x > 1 || PositionOnScreen.y < 0 || PositionOnScreen.y > 1) // Don't hit enemies out of the screen
                return;
            Projectile proj;
            if (proj = col.gameObject.GetComponentInParent<Projectile>()) { // If the hit object was a projectile
                if (!proj.Player) // Only process projectiles shot by the player
                    return;
                Health -= proj.Damage - (proj.WeaponKind == WeaponKind ? 1 : 0); // Take damage
                Destroy(col.gameObject);
                if (Health <= 0) { // Die if health reached zero
                    Dead = true; // Set dead flag, because the loot spawning might trigger a hit for this object
                    // Spawn loot
                    if (RareDrops.Length != 0 && Random.value < .5f)
                        Instantiate(Random.value < .25f ? RareDrops[Random.Range(0, RareDrops.Length)] : PlayerEntity.Instance.XPPickupObject,
                            LootSpawnPosition(), Quaternion.identity);
                    // Actually die
                    Instantiate(PlayerEntity.Instance.DeathEffect, transform.position, transform.rotation);
                    MapHandler.Instance.AwardKillScore();
                    Destroy(gameObject);
                }
            }
        }
    }
}