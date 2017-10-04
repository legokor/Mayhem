using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Menus {
    /// <summary>
    /// A button which can change a setting.
    /// </summary>
    [AddComponentMenu("Menus / Setting Button")]
    [RequireComponent(typeof(Text), typeof(Button))]
    public class SettingButton : MonoBehaviour {
        [Tooltip("The Setting's field in the Settings class.")]
        public string FieldName;
        [Tooltip("The displayed name on the button.")]
        public string FullName;

        PropertyInfo Property;
        Text Display;

        bool Get() {
            return (bool)Property.GetValue(null, null);
        }

        void SetText(bool Value) {
            Display.text = FullName + (Value ? " (on)" : " (off)");
        }

        void Start() {
            Property = typeof(Settings).GetProperty(FieldName);
            Display = GetComponent<Text>();
            SetText(Get());
            GetComponent<Button>().onClick.AddListener(Flip);
        }

        public void Flip() {
            bool NewValue = !Get();
            Property.SetValue(null, NewValue, null);
            SetText(NewValue);
            MainMenu.PlaySoundOn(gameObject);
        }
    }
}