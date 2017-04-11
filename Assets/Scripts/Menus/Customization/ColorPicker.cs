using UnityEngine;

namespace Menus.Customization {
    /// <summary>
    /// The trash button destroys the picked up attachment.
    /// </summary>
    [AddComponentMenu("Menus / Customization / Color Picker")]
    public class ColorPicker : MonoBehaviour {
        public int Color;

        public void Use() {
            Customize.Instance.SelectColor(Color);
        }
    }
}