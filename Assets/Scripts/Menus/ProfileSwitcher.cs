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
        [Tooltip("Profile list viewport content.")]
        public GameObject ProfileList;

        /// <summary>
        /// Vertical distance beetween buttons.
        /// </summary>
        float ButtonDistance;

        /// <summary>
        /// List of available player profiles.
        /// </summary>
        string[] Profiles = { "Default" };

        /// <summary>
        /// Create a loader button for a profile by cloning the <see cref="Sample"/>.
        /// </summary>
        /// <param name="Profile">Profile ID in the <see cref="Profiles"/> array</param>
        /// <returns>Loader button for the chosen profile</returns>
        Button CreateProfileButton(int Profile) {
            Transform ContentHolder = Sample.transform.parent;
            GameObject NewButton = Instantiate(Sample.gameObject, ContentHolder);
            RectTransform ContentRect = ContentHolder.GetComponent<RectTransform>();
            float ViewportHeight = Profile * ButtonDistance;
            ContentRect.sizeDelta = new Vector2(ContentRect.sizeDelta.x, ViewportHeight + ButtonDistance);
            // Temporarily moved, see the Update function!
            //NewButton.transform.localPosition = new Vector3(NewButton.transform.localPosition.x, -ViewportHeight);
            NewButton.GetComponent<Text>().text = Profiles[Profile];
            int CurrentProfile = Profile;
            Button ButtonComponent = NewButton.GetComponent<Button>();
            ButtonComponent.onClick.AddListener(delegate { LoadProfile(Profiles[CurrentProfile]); });
            return ButtonComponent;
        }

        /// <summary>
        /// Create loader buttons for all stored profiles.
        /// </summary>
        void CreateProfileButtons() {
            ButtonDistance = Sample.GetComponent<RectTransform>().sizeDelta.y;
            Button LastButton = null;
            int ProfileCount = Profiles.Length;
            for (int Profile = 0; Profile < ProfileCount; ++Profile)
                LastButton = CreateProfileButton(Profile);
            Destroy(Sample.gameObject);
            Sample = LastButton;
        }

        void Update() {
            // Temporary fix for Unity 2017.3.0, as repositioning after instantiating is bugged.
            Transform ButtonHolder = Sample.transform.parent;
            float InitialX = Sample.transform.localPosition.x;
            for (int ProfileID = 0; ProfileID < ButtonHolder.childCount; ++ProfileID)
                ButtonHolder.GetChild(ProfileID).localPosition = new Vector3(InitialX, -ProfileID * ButtonDistance);
            // Scrolling
            ProfileList.transform.position += new Vector3(0, (-Input.GetAxis("Vertical") * Time.deltaTime -
                Input.GetAxis("Mouse ScrollWheel") * ProfileList.transform.lossyScale.y * ButtonDistance * 30));
        }

        /// <summary>
        /// Load profile list when the profile switcher is created.
        /// </summary>
        void Awake() {
            if (!PlayerPrefs.HasKey("ProfileCount"))
                Save();
            int ProfileCount = PlayerPrefs.GetInt("ProfileCount");
            Profiles = new string[ProfileCount];
            for (int Profile = 0; Profile < ProfileCount; ++Profile)
                Profiles[Profile] = PlayerPrefs.GetString("Profile" + Profile);
            CreateProfileButtons();
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

        public void DeleteProfile(Text Name) {
            int ProfileCount = Profiles.Length;
            if (ProfileCount == 1)
                return;
            // Destroy all buttons except one
            Transform Holder = Sample.transform.parent;
            Sample = Holder.GetChild(0).gameObject.GetComponent<Button>();
            int Children = Holder.childCount;
            for (int Child = Children - 1; Child >= 1; --Child)
                Destroy(Holder.GetChild(Child).gameObject);
            // Delete profile entry
            for (int ProfileID = 0; ProfileID < ProfileCount; ++ProfileID) {
                if (Profiles[ProfileID].Equals(Name.text)) {
                    Profile.DeleteProfile(Name.text);
                    if (Profiles[ProfileID].Equals(Profile.Username))
                        LoadProfile(Profiles[ProfileID != 0 ? 0 : 1]); // Change profile to first available if the current is being deleted
                    --ProfileCount;
                    while (ProfileID < ProfileCount)
                        Profiles[ProfileID] = Profiles[++ProfileID];
                    Array.Resize(ref Profiles, ProfileCount);
                    CreateProfileButtons(); // Recreate UI
                    Save();
                    return;
                }
            }
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