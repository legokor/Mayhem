using UnityEngine;

namespace Menus {
    public static class Profile {
        public static int TopScore {
            get { return PlayerPrefs.GetInt("TopScore", 0); }
            set { PlayerPrefs.SetInt("TopScore", value); }
        }
    }
}