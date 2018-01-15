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

        bool LastValue;
        PropertyInfo Property;
        Text Display;

        bool Get() {
            return (bool)Property.GetValue(null, null);
        }

        void SetText(bool Value) {
            Display.text = FullName + (Value ? " (on)" : " (off)");
        }

        void Reload() {
            if (Get() != LastValue)
                Flip();
        }

        void OnEnable() {
            Property = typeof(Settings).GetProperty(FieldName);
            Display = GetComponent<Text>();
            SetText(LastValue = Get());
            GetComponent<Button>().onClick.AddListener(Flip);
            Profile.OnProfileChanged += Reload;
        }

        public void Flip() {
            bool NewValue = LastValue = !LastValue;
            Property.SetValue(null, NewValue, null);
            SetText(NewValue);
            MainMenu.PlaySoundOn(gameObject);
        }

        void OnDisable() {
            Profile.OnProfileChanged -= Reload;
        }
    }
}