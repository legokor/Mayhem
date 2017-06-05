using UnityEngine;
using UnityEngine.UI;

namespace Menus {
    /// <summary>
    /// A button which can change a setting.
    /// </summary>
    [AddComponentMenu("Menus / SettingButton")]
    [RequireComponent(typeof(Text)), RequireComponent(typeof(Button))]
    public class SettingButton : MonoBehaviour {
        /// <summary>
        /// The Setting's field in the Settings class.
        /// </summary>
        public string FieldName;
        /// <summary>
        /// The displayed name on the button.
        /// </summary>
        public string FullName;

        Text Display;

        bool Get() {
            return (bool)typeof(Settings).GetProperty(FieldName).GetValue(null, null);
        }

        void SetText(bool Value) {
            Display.text = FullName + (Value ? " (on)" : " (off)");
        }

        void Start() {
            Display = GetComponent<Text>();
            SetText(Get());
            GetComponent<Button>().onClick.AddListener(Flip);
        }

        public void Flip() {
            bool NewValue = !Get();
            typeof(Settings).GetProperty(FieldName).SetValue(null, NewValue, null);
            SetText(NewValue);
            MainMenu.PlaySoundOn(gameObject);
        }
    }
}