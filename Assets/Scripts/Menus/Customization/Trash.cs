using UnityEngine;

namespace Menus.Customization {
    /// <summary>
    /// The trash button destroys the picked up attachment.
    /// </summary>
    [AddComponentMenu("Menus / Customization / Trash")]
    public class Trash : MonoBehaviour {
        public void Use() {
            if (Attachment.PickedUp)
                Customize.Instance.OnRemove();
            Attachment.DestroyPickedUp();
        }
    }
}