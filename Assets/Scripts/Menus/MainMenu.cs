using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Cavern;

using Destructors;
using Helpers;

namespace Menus {
    [AddComponentMenu("Menus / Main Menu")]
    public class MainMenu : MonoBehaviour {
        [Tooltip("Button click sound.")]
        public AudioClip SelectSound;
        [Tooltip("Give access to all unlockables and hide the Exit button.")]
        public bool KioskMode = false;

        public Transform MenuTarget, CustomizeTarget, SettingsTarget, CalibrationTarget, AboutTarget;
        public LerpToPlace MenuObject, LevelSelectorObject;
        public GameObject MenuPlace, MenuHide, LevelSelectorPlace, LevelSelectorHide, ExitButton;
        public Text LeapCalibrationText, CavernText;

        /// <summary>
        /// Selected level.
        /// </summary>
        int Level;
        /// <summary>
        /// Largest available level ID.
        /// </summary>
        int MaxLevel;
        /// <summary>
        /// Camera movement target.
        /// </summary>
        Transform CameraTarget;
        /// <summary>
        /// Distance between level textures in the level selector.
        /// </summary>
        Vector3 LevelSelectorStep;

        /// <summary>
        /// Cached selection sound for static use.
        /// </summary>
        static AudioClip _SelectSound;

        /// <summary>
        /// Play a sound at the location of a GameObject.
        /// </summary>
        /// <param name="Obj">Target</param>
        public static void PlaySoundOn(GameObject Obj) {
            GameObject NewObj = new GameObject();
            NewObj.transform.position = Obj.transform.position;
            AudioSource3D Source = NewObj.AddComponent<AudioSource3D>();
            Source.clip = _SelectSound;
            Source.Volume = .1f;
            Source.Play();
            NewObj.AddComponent<TimedDespawner>().Timer = 1;
        }

        /// <summary>
        /// Non-static version of <see cref="PlaySoundOn(GameObject)"/> for Unity Action use.
        /// </summary>
        /// <param name="Obj">Target</param>
        public void PlaySoundOnObj(GameObject Obj) {
            PlaySoundOn(Obj);
        }

        void ApplyCalibration(Vector3 Minimums, Vector3 Maximums) {
            Settings.LeapLowerBounds = Minimums;
            Settings.LeapUpperBounds = Maximums;
            Settings.LeapSetup();
        }

        void Start() {
            if (LevelSelectorObject) { // This is the menu
                _SelectSound = SelectSound;
                if (KioskMode)
                    Destroy(ExitButton);
                if (!LeapMotion.Instance.Connected) {
                    LeapCalibrationText.text = "Leap Motion not found";
                    LeapCalibrationText.gameObject.GetComponent<Button>().interactable = false;
                }
                LevelSelectorStep = LevelSelectorPlace.transform.position - LevelSelectorHide.transform.position;
                MaxLevel = LevelSelectorObject.transform.childCount - 1;
                Calibration.Instance.gameObject.SetActive(false);
                Calibration.Instance.CalibrationResult += ApplyCalibration;
                Settings.LeapSetup();
                CavernText.text = "Cavern output: " + AudioListener3D.GetLayoutName();
            }
        }

        public void Play() {
            CameraTarget = MenuTarget;
            MenuObject.Target = MenuHide;
            Level = 0;
            TimedDisabler Disabler = LevelSelectorObject.transform.parent.GetComponent<TimedDisabler>();
            if (Disabler)
                Destroy(Disabler);
            LevelSelectorObject.transform.parent.gameObject.SetActive(true);
            LevelSelectorObject.transform.position = LevelSelectorHide.transform.position;
            LevelSelectorPlace.transform.position = LevelSelectorHide.transform.position + LevelSelectorStep;
            PlaySoundOn(MenuPlace);
        }

        public void PreviousLevel() {
            if (Level != 0) {
                LevelSelectorPlace.transform.position -= LevelSelectorStep;
                --Level;
            }
            PlaySoundOn(LevelSelectorHide);
        }

        public void NextLevel() {
            if (Level != MaxLevel) {
                LevelSelectorPlace.transform.position += LevelSelectorStep;
                ++Level;
            }
            PlaySoundOn(LevelSelectorHide);
        }

        public void LevelSelect(string Name) {
            SceneManager.LoadScene(Name);
        }

        public void Customize() {
            CameraTarget = CustomizeTarget;
            Customization.Customize.Instance.Deserialize();
            PlaySoundOn(MenuPlace);
        }

        public void Cleanup() {
            Customization.Customize.Instance.Cleanup();
        }

        public void SettingsButton() {
            CameraTarget = SettingsTarget;
            PlaySoundOn(MenuPlace);
        }

        public void Exit() {
            Application.Quit();
        }

        public void LeapCalibration(GameObject Caller) {
            CameraTarget = CalibrationTarget;
            Calibration.Instance.gameObject.SetActive(true);
            PlaySoundOn(Caller);
        }

        public void AboutButton(GameObject Caller) {
            CameraTarget = AboutTarget;
            PlaySoundOn(Caller);
        }

        public void Back() {
            PlaySoundOn(CameraTarget.gameObject);
            if (CameraTarget == CustomizeTarget) {
                Customization.Customize.Instance.Serialize();
                Customization.Attachment.DestroyPickedUp();
            }
            if (CameraTarget == CalibrationTarget) {
                Calibration.Instance.gameObject.AddComponent<TimedDisabler>().Timer = .25f;
                CameraTarget = SettingsTarget;
            } else
                CameraTarget = CameraTarget == AboutTarget ? SettingsTarget : MenuTarget;
            MenuObject.Target = MenuPlace;
            LevelSelectorObject.transform.parent.gameObject.AddComponent<TimedDisabler>().Timer = .25f;
            LevelSelectorPlace.transform.position = LevelSelectorHide.transform.position;
        }

        void Update() {
            if (CameraTarget) {
                float MoveSpeed = 3 * Time.deltaTime;
                Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, CameraTarget.position, MoveSpeed);
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, CameraTarget.rotation, MoveSpeed);
            }
        }
    }
}