using UnityEngine;
using UnityEngine.UI;

namespace Menus {
    /// <summary>
    /// Profile switcher behaviour.
    /// </summary>
    [AddComponentMenu("Menus / Profile Switcher")]
    public class ProfileSwitcher : MonoBehaviour {
        [Tooltip("Sample button.")]
        public Button Sample;

        /// <summary>
        /// List of available player profiles.
        /// </summary>
        string[] Profiles = { "Default" };

        /// <summary>
        /// Load profile list when the profile switcher is created.
        /// </summary>
        void Awake() {
            // Load list
            if (!PlayerPrefs.HasKey("ProfileCount"))
                Save();
            int ProfileCount = PlayerPrefs.GetInt("ProfileCount");
            Profiles = new string[ProfileCount];
            for (int Profile = 0; Profile < ProfileCount; ++Profile)
                Profiles[Profile] = PlayerPrefs.GetString("Profile" + Profile);
            // Create buttons
            float ButtonDistance = Sample.GetComponent<RectTransform>().sizeDelta.y;
            for (int Profile = 0; Profile < ProfileCount; ++Profile) {
                GameObject NewButton = Instantiate(Sample.gameObject, Sample.transform.parent);
                NewButton.transform.position += new Vector3(0, Profile * ButtonDistance);
                NewButton.GetComponent<Text>().text = Profiles[Profile];
                int CurrentProfile = Profile;
                NewButton.GetComponent<Button>().onClick.AddListener(delegate { LoadProfile(Profiles[CurrentProfile]); });
            }
            Destroy(Sample.gameObject);
        }

        /// <summary>
        /// Loads a profile by name. Delegates will do the rest.
        /// </summary>
        void LoadProfile(string Name) {
            Profile.Username = Name;
        }

        /// <summary>
        /// Save profile list.
        /// </summary>
        void Save() {
            int ProfileCount = Profiles.Length;
            PlayerPrefs.SetInt("ProfileCount", ProfileCount);
            for (int Profile = 0; Profile < ProfileCount; ++Profile)
                PlayerPrefs.SetString("Profile" + Profile, Profiles[Profile]);
        }
    }
}