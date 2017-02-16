using UnityEngine;

namespace Destructors {
    /// <summary>
    /// Disables the object after a set time.
    /// </summary>
    public class TimedDisabler : MonoBehaviour {
        public float Timer = 3;

        void Awake() {
            TimedDisabler[] Disablers = GetComponents<TimedDisabler>();
            foreach (TimedDisabler Disabler in Disablers)
                if (Disabler != this)
                    Destroy(Disabler);
        }

        void Update() {
            if ((Timer -= Time.deltaTime) <= 0) {
                gameObject.SetActive(false);
                Destroy(this);
            }
        }
    }
}