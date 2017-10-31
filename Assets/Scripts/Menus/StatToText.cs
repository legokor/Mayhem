using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Menus {
    /// <summary>
    /// Shows a setting's value on a Text.
    /// </summary>
    [AddComponentMenu("Menus / Stat to Text")]
    [RequireComponent(typeof(Text))]
    public class StatToText : MonoBehaviour {
        [Tooltip("The player property's field in the Profile class.")]
        public string FieldName;

        PropertyInfo Property;
        Text Display;

        public void Reload() {
            Display.text = ((int)Property.GetValue(null, null)).ToString();
        }

        void OnEnable() {
            Property = typeof(Profile).GetProperty(FieldName);
            Display = GetComponent<Text>();
            Reload();
            Profile.OnProfileChanged += Reload;
        }

        void OnDisable() {
            Profile.OnProfileChanged -= Reload;
        }
    }
}