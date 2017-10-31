using System;
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
        /// Vertical distance beetween buttons.
        /// </summary>
        float ButtonDistance;

        /// <summary>
        /// List of available player profiles.
        /// </summary>
        string[] Profiles = { "Default" };

        Button CreateProfileButton(int Profile) {
            GameObject NewButton = Instantiate(Sample.gameObject, Sample.transform.parent);
            NewButton.transform.localPosition = new Vector3(NewButton.transform.localPosition.x, -Profile * ButtonDistance);
            NewButton.GetComponent<Text>().text = Profiles[Profile];
            int CurrentProfile = Profile;
            Button ButtonComponent = NewButton.GetComponent<Button>();
            ButtonComponent.onClick.AddListener(delegate { LoadProfile(Profiles[CurrentProfile]); });
            return ButtonComponent;
        }

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
            ButtonDistance = Sample.GetComponent<RectTransform>().sizeDelta.y;
            Button LastButton = null;
            for (int Profile = 0; Profile < ProfileCount; ++Profile)
                LastButton = CreateProfileButton(Profile);
            Destroy(Sample.gameObject);
            Sample = LastButton;
        }

        /// <summary>
        /// Loads a profile by name. Delegates will do the rest.
        /// </summary>
        public void LoadProfile(string Name) {
            Profile.Username = Name;
        }

        /// <summary>
        /// Create and load a new profile.
        /// </summary>
        public void CreateProfile(InputField Name) {
            if (Name.text.Equals(string.Empty))
                return;
            int ProfileCount = Profiles.Length;
            for (int Profile = 0; Profile < ProfileCount; ++Profile)
                if (Profiles[Profile].Equals(Name.text))
                    return;
            Array.Resize(ref Profiles, ProfileCount + 1);
            LoadProfile(Profiles[ProfileCount] = Name.text);
            CreateProfileButton(ProfileCount);
            Save();
        }

        /// <summary>
        /// Save profile list.
        /// </summary>
        void Save() {
            int ProfileCount = Profiles.Length;
            PlayerPrefs.SetInt("ProfileCount", ProfileCount);
            for (int Profile = 0; Profile < ProfileCount; ++Profile)
                PlayerPrefs.SetString("Profile" + Profile, Profiles[Profile]);
            PlayerPrefs.Save();
        }
    }
}