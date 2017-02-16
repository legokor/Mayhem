using UnityEngine;

namespace Menus {
    /// <summary>
    /// Setting holder.
    /// </summary>
    public static class Settings {
        /// <summary>
        /// Dumbed down graphics with Vsync for the lowest system reuqirements.
        /// </summary>
        public static bool SketchGraphics = false;
        /// <summary>
        /// Enable the motion blur effect.
        /// </summary>
        public static bool MotionBlur = true;
        /// <summary>
        /// Lower bounds of the Leap Motion control space in all dimensions.
        /// </summary>
        public static Vector3 LeapLowerBounds = new Vector3(-200, 100, -112.5f);
        /// <summary>
        /// Upper bounds of the Leap Motion control space in all dimensions.
        /// </summary>
        public static Vector3 LeapUpperBounds = new Vector3(-200, 300, 112.5f);

        /// <summary>
        /// Set Leap Motion bounds for use on a vertical plane.
        /// </summary>
        public static void LeapSetupXY() {
            LeapMotion.Instance.LeapLowerBounds = new Vector2(LeapLowerBounds.x, LeapLowerBounds.y);
            LeapMotion.Instance.LeapUpperBounds = new Vector2(LeapUpperBounds.x, LeapUpperBounds.y);
        }

        /// <summary>
        /// Set Leap Motion bounds for use on a horizontal plane.
        /// </summary>
        public static void LeapSetupXZ() {
            LeapMotion.Instance.LeapLowerBounds = new Vector2(LeapLowerBounds.x, LeapLowerBounds.z);
            LeapMotion.Instance.LeapUpperBounds = new Vector2(LeapUpperBounds.x, LeapUpperBounds.z);
        }
    }
}
