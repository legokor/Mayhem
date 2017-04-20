using UnityEngine;

namespace Menus {
    /// <summary>
    /// Profile data holder.
    /// </summary>
    public static class Profile {
        /// <summary>
        /// The player's best score.
        /// </summary>
        public static int TopScore {
            get { return PlayerPrefs.GetInt("TopScore", 0); }
            set { PlayerPrefs.SetInt("TopScore", value); }
        }
    }
}