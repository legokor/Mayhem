using UnityEngine;

namespace Menus {
    public static class Settings {
        /// <summary>
        /// Dumbed down graphics with Vsync for the lowest system reuqirements.
        /// </summary>
        public static bool SketchGraphics = false;
        /// <summary>
        /// Enable the motion blur effect.
        /// </summary>
        public static bool MotionBlur = true;
        public static Vector3 LeapLowerBounds = new Vector3(-200, 100, -112.5f);
        public static Vector3 LeapUpperBounds = new Vector3(-200, 300, 112.5f);

        public static void LeapSetupXY() {
            LeapMotion.Instance.LeapLowerBounds = new Vector2(LeapLowerBounds.x, LeapLowerBounds.y);
            LeapMotion.Instance.LeapUpperBounds = new Vector2(LeapUpperBounds.x, LeapUpperBounds.y);
        }

        public static void LeapSetupXZ() {
            LeapMotion.Instance.LeapLowerBounds = new Vector2(LeapLowerBounds.x, LeapLowerBounds.z);
            LeapMotion.Instance.LeapUpperBounds = new Vector2(LeapUpperBounds.x, LeapUpperBounds.z);
        }
    }
}
