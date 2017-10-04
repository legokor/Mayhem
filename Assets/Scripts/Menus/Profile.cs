using System.Text;
using UnityEngine;

namespace Menus {
    /// <summary>
    /// Profile data holder.
    /// </summary>
    public static class Profile {
        /// <summary>
        /// Current player username.
        /// </summary>
        public static string Username = "Default";

        /// <summary>
        /// Sets the value of the preference identified by key.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="Value">New value</param>
        public static void SetBool(string Key, bool Value) {
            PlayerPrefs.SetInt(new StringBuilder(Username).Append(Key).ToString(), Value ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Sets the value of the preference identified by key.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="Value">New value</param>
        public static void SetFloat(string Key, float Value) {
            PlayerPrefs.SetFloat(new StringBuilder(Username).Append(Key).ToString(), Value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Sets the value of the preference identified by key.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="Value">New value</param>
        public static void SetInt(string Key, int Value) {
            PlayerPrefs.SetInt(new StringBuilder(Username).Append(Key).ToString(), Value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Sets the value of the preference identified by key.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="Value">New value</param>
        public static void SetString(string Key, string Value) {
            PlayerPrefs.SetString(new StringBuilder(Username).Append(Key).ToString(), Value);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Sets the value of the preference identified by key.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="Value">New value</param>
        public static void SetVector3(string Key, Vector3 Value) {
            string KeyName = new StringBuilder(Username).Append(Key).ToString();
            PlayerPrefs.SetFloat(KeyName + "X", Value.x);
            PlayerPrefs.SetFloat(KeyName + "Y", Value.y);
            PlayerPrefs.SetFloat(KeyName + "Z", Value.z);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Returns the value corresponding to key in the preferences if it exists.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="DefaultValue">Value to return if the key does not exist</param>
        /// <returns>Value by key or the default value if the key does not exist</returns>
        public static bool GetBool(string Key, bool DefaultValue = false) {
            return PlayerPrefs.GetInt(new StringBuilder(Username).Append(Key).ToString(), DefaultValue ? 1 : 0) != 0;
        }

        /// <summary>
        /// Returns the value corresponding to key in the preferences if it exists.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="DefaultValue">Value to return if the key does not exist</param>
        /// <returns>Value by key or the default value if the key does not exist</returns>
        public static float GetFloat(string Key, float DefaultValue = 0) {
            return PlayerPrefs.GetFloat(new StringBuilder(Username).Append(Key).ToString(), DefaultValue);
        }

        /// <summary>
        /// Returns the value corresponding to key in the preferences if it exists.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="DefaultValue">Value to return if the key does not exist</param>
        /// <returns>Value by key or the default value if the key does not exist</returns>
        public static int GetInt(string Key, int DefaultValue = 0) {
            return PlayerPrefs.GetInt(new StringBuilder(Username).Append(Key).ToString(), DefaultValue);
        }

        /// <summary>
        /// Returns the value corresponding to key in the preferences if it exists.
        /// </summary>
        /// <param name="Key">Key name</param>
        /// <param name="DefaultValue">Value to return if the key does not exist</param>
        /// <returns>Value by key or the default value if the key does not exist</returns>
        public static string GetString(string Key, string DefaultValue = "") {
            return PlayerPrefs.GetString(new StringBuilder(Username).Append(Key).ToString(), DefaultValue);
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
            string KeyName = new StringBuilder(Username).Append(Key).ToString();
            return new Vector3(PlayerPrefs.GetFloat(KeyName + "X", DefaultX), PlayerPrefs.GetFloat(KeyName + "Y", DefaultY), PlayerPrefs.GetFloat(KeyName + "Z", DefaultZ));
        }

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
    }
}