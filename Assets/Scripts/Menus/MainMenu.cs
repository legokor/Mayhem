using System;
using System.Text;
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

        public Transform MenuTarget, CustomizeTarget, SettingsTarget, CalibrationTarget;
        public LerpToPlace MenuObject, LevelSelectorObject;
        public GameObject MenuPlace, MenuHide, LevelSelectorPlace, LevelSelectorHide, ExitButton;
        public Text SketchModeStatus, MotionBlurStatus, LeapCalibrationText, CavernText;

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
        /// Play a sound at the location of a GameObject.
        /// </summary>
        /// <param name="Obj">Target</param>
        void PlaySoundOn(GameObject Obj) {
            GameObject NewObj = new GameObject();
            NewObj.transform.position = Obj.transform.position;
            AudioSource3D Source = NewObj.AddComponent<AudioSource3D>();
            Source.clip = SelectSound;
            Source.Volume = .33f;
            Source.Play();
            NewObj.AddComponent<TimedDespawner>().Timer = 1;
        }

        void ResetGraphics() {
            QualitySettings.SetQualityLevel(Convert.ToInt32(Settings.SketchGraphics));
            SketchModeStatus.text = "Sketch graphics (" + (Settings.SketchGraphics ? "on" : "off") + ")";
            MotionBlurStatus.text = "Motion blur (" + (Settings.MotionBlur ? "on" : "off") + ")";
        }

        void ApplyCalibration(Vector3 Minimums, Vector3 Maximums) {
            Settings.LeapLowerBounds = Minimums;
            Settings.LeapUpperBounds = Maximums;
            Settings.LeapSetupXY();
        }

        void Start() {
            if (LevelSelectorObject) { // This is the menu
                if (KioskMode)
                    Destroy(ExitButton);
                ResetGraphics();
                if (!LeapMotion.Instance.Connected) {
                    LeapCalibrationText.text = "Leap Motion not found";
                    LeapCalibrationText.gameObject.GetComponent<Button>().interactable = false;
                }
                LevelSelectorStep = LevelSelectorPlace.transform.position - LevelSelectorHide.transform.position;
                MaxLevel = LevelSelectorObject.transform.childCount - 1;
                Calibration.Instance.gameObject.SetActive(false);
                Calibration.Instance.CalibrationResult += ApplyCalibration;
                Settings.LeapSetupXY();
                StringBuilder CavernOut = new StringBuilder("Cavern output: ");
                if (AudioListener3D.Current.HeadphoneVirtualizer)
                    CavernOut.Append(" headphone");
                else {
                    int Regular = 0, LFE = 0, Ceiling = 0, Floor = 0, Channels = AudioListener3D.Channels.Length;
                    for (int i = 0; i < Channels; ++i)
                        if (AudioListener3D.Channels[i].LFE) ++LFE;
                        else if (AudioListener3D.Channels[i].x == 0) ++Regular;
                        else if (AudioListener3D.Channels[i].x < 0) ++Ceiling;
                        else if (AudioListener3D.Channels[i].x > 0) ++Floor;
                    CavernOut.Append(Regular).Append('.').Append(LFE);
                    if (Ceiling > 0 || Floor > 0) CavernOut.Append('.').Append(Ceiling);
                    if (Floor > 0) CavernOut.Append('.').Append(Floor);
                }
                CavernText.text = CavernOut.ToString();
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

        public void SketchMode(GameObject Caller) {
            Settings.SketchGraphics = !Settings.SketchGraphics;
            ResetGraphics();
            PlaySoundOn(Caller);
        }

        public void MotionBlur(GameObject Caller) {
            Settings.MotionBlur = !Settings.MotionBlur;
            ResetGraphics();
            PlaySoundOn(Caller);
        }

        public void LeapCalibration(GameObject Caller) {
            CameraTarget = CalibrationTarget;
            Calibration.Instance.gameObject.SetActive(true);
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
                CameraTarget = MenuTarget;
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