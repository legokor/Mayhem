using UnityEngine;
using System;

namespace Menus {
    /// <summary>
    /// Setting holder.
    /// </summary>
    public static class Settings {
        /// <summary>
        /// Enable background music.
        /// </summary>
        public static bool Music {
            get { return _Music ?? (_Music = Profile.GetBool("Music", true)).Value; }
            set { Profile.SetBool("Music", (_Music = value).Value); }
        }
        static bool? _Music = null;

        /// <summary>
        /// Enable high quality audio.
        /// </summary>
        public static bool HQAudio {
            get { return _HQAudio ?? (_HQAudio = Profile.GetBool("HQAudio", false)).Value; }
            set { Profile.SetBool("HQAudio", (_HQAudio = value).Value); }
        }
        static bool? _HQAudio = null;

        /// <summary>
        /// Dumbed down graphics with Vsync for the lowest system reuqirements.
        /// </summary>
        public static bool SketchGraphics {
            get { bool Level = Profile.GetBool("Sketch", false); SetGraphics(Level); return Level; }
            set { Profile.SetBool("Sketch", value); SetGraphics(value); }
        }
        static void SetGraphics(bool Sketch) {
            int TargetLevel = Convert.ToInt32(Sketch);
            if (QualitySettings.GetQualityLevel() != TargetLevel)
                QualitySettings.SetQualityLevel(TargetLevel);
        }

        /// <summary>
        /// Enable the motion blur effect.
        /// </summary>
        public static bool MotionBlur {
            get { return _MotionBlur ?? (_MotionBlur = Profile.GetBool("MotionBlur", false)).Value; }
            set { Profile.SetBool("MotionBlur", (_MotionBlur = value).Value); }
        }
        static bool? _MotionBlur = null;

        /// <summary>
        /// Enable Side-by-Side 3D.
        /// </summary>
        public static bool ThreeD {
            get { return SBS.Enabled = Profile.GetBool("ThreeD", false); }
            set { Profile.SetBool("ThreeD", value); SBS.Enabled = value; }
        }

        /// <summary>
        /// Third person mode instead of a top-down camera.
        /// </summary>
        public static bool FollowerCamera {
            get { return _FollowerCamera ?? (_FollowerCamera = Profile.GetBool("FollowerCamera", true)).Value; }
            set { Profile.SetBool("FollowerCamera", (_FollowerCamera = value).Value); }
        }
        static bool? _FollowerCamera = null;

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
