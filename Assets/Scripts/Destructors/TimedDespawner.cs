using UnityEngine;

namespace Destructors {
    /// <summary>
    /// Destroy the object after a set time.
    /// </summary>
    public class TimedDespawner : MonoBehaviour {
        public float Timer = 3;

        void Awake() {
            TimedDespawner[] Despawners = GetComponents<TimedDespawner>();
            foreach (TimedDespawner Despawner in Despawners)
                if (Despawner != this)
                    Destroy(Despawner);
        }

        void Update() {
            if ((Timer -= Time.deltaTime) <= 0)
                Destroy(gameObject);
        }
    }
}