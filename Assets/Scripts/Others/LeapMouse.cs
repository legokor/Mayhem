using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeapMouse : Singleton<LeapMouse> {
    [Tooltip("Cursor texture.")]
    public Texture MouseIcon;
    [Tooltip("Size of the cursor.")]
    public Vector2 MouseSize = new Vector2(64, 64);
    [Tooltip("The center of the cursor is the selection.")]
    public bool CenterPointer = true;

    /// <summary>
    /// The button the cursor was over last frame.
    /// </summary>
    Button LastHovered;
    /// <summary>
    /// Dummy data required for some UI calls.
    /// </summary>
    PointerEventData RandomPointerEventData;
    /// <summary>
    /// Extended finger count at the last frame.
    /// </summary>
    int LastFingerCount = 0;

	void Start() {
        RandomPointerEventData = new PointerEventData(GetComponent<EventSystem>());
    }

    void OnEnable() {
        Menus.Settings.LeapSetupXY();
    }

    void OnGUI() {
        Vector2 HandPosition = LeapMotion.Instance.PalmOnScreenXY();
        if (HandPosition.x != -1) {
            int FingerCount = LeapMotion.Instance.ExtendedFingers();
            Vector2 DrawStartPos = HandPosition;
            if (CenterPointer) {
                DrawStartPos -= MouseSize * .5f;
                if (FingerCount != 0)
                    DrawStartPos -= MouseSize * .1f;
            }
            GUI.DrawTexture(new Rect(DrawStartPos, MouseSize * (FingerCount != 0 ? 1 : .8f)), MouseIcon);
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector2(HandPosition.x, Screen.height - HandPosition.y)), out hit)) {
                Button Hovered = hit.collider.gameObject.GetComponentInChildren<Button>();
                if (Hovered) {
                    if (LastHovered && Hovered != LastHovered)
                        LastHovered.OnPointerExit(RandomPointerEventData);
                    Hovered.OnPointerEnter(RandomPointerEventData);
                    LastHovered = Hovered;
                    if (FingerCount == 0 && LastFingerCount != 0)
                        Hovered.OnPointerClick(RandomPointerEventData);
                } else if (LastHovered)
                    LastHovered.OnPointerExit(RandomPointerEventData);
            } else if (LastHovered)
                LastHovered.OnPointerExit(RandomPointerEventData);
            LastFingerCount = FingerCount;
        }
	}
}