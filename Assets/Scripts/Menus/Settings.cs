using UnityEngine;

namespace Menus {
    /// <summary>
    /// Setting holder.
    /// </summary>
    public static class Settings {
        /// <summary>
        /// Dumbed down graphics with Vsync for the lowest system reuqirements.
        /// </summary>
        public static bool SketchGraphics {
            get { return Profile.GetBool("Sketch", false); }
            set { Profile.SetBool("Sketch", value); }
        }

        /// <summary>
        /// Enable the motion blur effect.
        /// </summary>
        public static bool MotionBlur {
            get { return Profile.GetBool("MotionBlur", false); }
            set { Profile.SetBool("MotionBlur", value); }
        }

        /// <summary>
        /// Lower bounds of the Leap Motion control space in all dimensions.
        /// </summary>
        public static Vector3 LeapLowerBounds {
            get { return Profile.GetVector3("LeapMin", -200, 100, -112.5f); }
            set { Profile.SetVector3("LeapMin", value); }
        }

        /// <summary>
        /// Upper bounds of the Leap Motion control space in all dimensions.
        /// </summary>
        public static Vector3 LeapUpperBounds {
            get { return Profile.GetVector3("LeapMax", 200, 300, 112.5f); }
            set { Profile.SetVector3("LeapMax", value); }
        }

        /// <summary>
        /// Set Leap Motion bounds for use on a vertical plane.
        /// </summary>
        public static void LeapSetupXY() {
            Vector3 Lower = LeapLowerBounds, Upper = LeapUpperBounds;
            LeapMotion.Instance.LeapLowerBounds = new Vector2(Lower.x, Lower.y);
            LeapMotion.Instance.LeapUpperBounds = new Vector2(Upper.x, Upper.y);
        }

        /// <summary>
        /// Set Leap Motion bounds for use on a horizontal plane.
        /// </summary>
        public static void LeapSetupXZ() {
            Vector3 Lower = LeapLowerBounds, Upper = LeapUpperBounds;
            LeapMotion.Instance.LeapLowerBounds = new Vector2(Lower.x, Lower.z);
            LeapMotion.Instance.LeapUpperBounds = new Vector2(Upper.x, Upper.z);
        }
    }
}
