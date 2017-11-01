using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Menus {
    /// <summary>
    /// Profile data holder.
    /// </summary>
    public static class Profile {
        public delegate void ProfileChange();

        public static event ProfileChange OnProfileChanged;

        /// <summary>
        /// Get or set the profile by name.
        /// </summary>
        public static string Username {
            get {
                if (!UserLoaded) {
                    _Username = PlayerPrefs.GetString("Profile", "Default");
                    UserLoaded = true;
                }
                return _Username;
            }
            set {
                PlayerPrefs.SetString("Profile", _Username = value);
                PlayerPrefs.Save();
                OnProfileChanged?.Invoke();
            }
        }
        static bool UserLoaded = false;
        static string _Username = "Default";

        /// <summary>
        /// The player's best score.
        /// </summary>
        public static int TopScore {
            get { return GetInt("TopScore", 0); }
            set { SetInt("TopScore", value); }
        }

        /// <summary>
        /// Unlocked levels' count.
        /// </summary>
        public static int Unlocks {
            get { return GetInt("Unlocks", 1); }
            set { SetInt("Unlocks", value); }
        }

        /// <summary>
        /// Tokens available to buy parts.
        /// </summary>
        public static int Tokens {
            get { return GetInt("Tokens", 3); }
            set { SetInt("Tokens", value); }
        }

        /// <summary>
        /// Key name for the currently loaded profile.
        /// </summary>
        /// <param name="Key">Original key name</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string GetKeyName(string Key) {
            return new StringBuilder(Username).Append(Key).ToString();
        }

        /// <summary>
        /// Sets the value of the preference identified by key.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="Value">New value</param>
        public static void SetBool(string Key, bool Value) {
            PlayerPrefs.SetInt(GetKeyName(Key), Value ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Sets the value of the preference identified by key.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="Value">New value</param>
        public static void SetFloat(string Key, float Value) {
            PlayerPrefs.SetFloat(GetKeyName(Key), Value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Sets the value of the preference identified by key.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="Value">New value</param>
        public static void SetInt(string Key, int Value) {
            PlayerPrefs.SetInt(GetKeyName(Key), Value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Sets the value of the preference identified by key.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="Value">New value</param>
        public static void SetString(string Key, string Value) {
            PlayerPrefs.SetString(GetKeyName(Key), Value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Sets the value of the preference identified by key.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="Value">New value</param>
        public static void SetVector3(string Key, Vector3 Value) {
            Key = GetKeyName(Key);
            PlayerPrefs.SetFloat(Key + "X", Value.x);
            PlayerPrefs.SetFloat(Key + "Y", Value.y);
            PlayerPrefs.SetFloat(Key + "Z", Value.z);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Returns the value corresponding to key in the preferences if it exists.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="DefaultValue">Value to return if the key does not exist</param>
        /// <returns>Value by key or the default value if the key does not exist</returns>
        public static bool GetBool(string Key, bool DefaultValue = false) {
            return PlayerPrefs.GetInt(GetKeyName(Key), DefaultValue ? 1 : 0) != 0;
        }

        /// <summary>
        /// Returns the value corresponding to key in the preferences if it exists.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="DefaultValue">Value to return if the key does not exist</param>
        /// <returns>Value by key or the default value if the key does not exist</returns>
        public static float GetFloat(string Key, float DefaultValue = 0) {
            return PlayerPrefs.GetFloat(GetKeyName(Key), DefaultValue);
        }

        /// <summary>
        /// Returns the value corresponding to key in the preferences if it exists.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="DefaultValue">Value to return if the key does not exist</param>
        /// <returns>Value by key or the default value if the key does not exist</returns>
        public static int GetInt(string Key, int DefaultValue = 0) {
            return PlayerPrefs.GetInt(GetKeyName(Key), DefaultValue);
        }

        /// <summary>
        /// Returns the value corresponding to key in the preferences if it exists.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="DefaultValue">Value to return if the key does not exist</param>
        /// <returns>Value by key or the default value if the key does not exist</returns>
        public static string GetString(string Key, string DefaultValue = "") {
            return PlayerPrefs.GetString(GetKeyName(Key), DefaultValue);
        }

        /// <summary>
        /// Returns the value corresponding to key in the preferences if it exists.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="DefaultX">Value to return on the X axis if the key does not exist</param>
        /// <param name="DefaultY">Value to return on the Y axis if the key does not exist</param>
        /// <param name="DefaultZ">Value to return on the Z axis if the key does not exist</param>
        /// <returns>Value by key or the default value if the key does not exist</returns>
        public static Vector3 GetVector3(string Key, float DefaultX = 0, float DefaultY = 0, float DefaultZ = 0) {
            Key = GetKeyName(Key);
            return new Vector3(PlayerPrefs.GetFloat(Key + "X", DefaultX), PlayerPrefs.GetFloat(Key + "Y", DefaultY), PlayerPrefs.GetFloat(Key + "Z", DefaultZ));
        }

        /// <summary>
        /// List of possible key names for a profile.
        /// </summary>
        readonly static string[] AllKeyNames = {
            "TopScore", "Unlocks", "Tokens", // Progression
            "Ship", "ShipColor", // Customization
            "Music", "HQAudio", "Sketch", "MotionBlur", "ThreeD", "FollowerCamera", "LeapMin", "LeapMax" // Settings
        };

        /// <summary>
        /// Delete all keys for a profile.
        /// </summary>
        /// <param name="Name">Profile name</param>
        public static void DeleteProfile(string Name) {
            int KeyCount = AllKeyNames.Length;
            for (int Key = 0; Key < KeyCount; ++Key) {
                string KeyName = new StringBuilder(Name).Append(AllKeyNames[Key]).ToString();
                if (PlayerPrefs.HasKey(KeyName))
                    PlayerPrefs.DeleteKey(KeyName);
            }
            PlayerPrefs.Save();
        }
    }
}