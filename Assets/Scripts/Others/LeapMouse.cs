using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("Others / Leap Mouse")]
public class LeapMouse : Singleton<LeapMouse> {
    [Tooltip("Cursor texture.")]
    public Texture MouseIcon;
    [Tooltip("On-screen off-hand marker.")]
    public Texture OffHandIcon;
    [Tooltip("Size of the cursor.")]
    public Vector2 MouseSize = new Vector2(64, 64);
    [Tooltip("The center of the cursor is the selection.")]
    public bool CenterPointer = true;

    /// <summary>
    /// A tap happened in the last frame.
    /// </summary>
    bool Tapped = false;
    /// <summary>
    /// The UI element the cursor was over last frame.
    /// </summary>
    Selectable LastHovered;
    /// <summary>
    /// Dummy data required for some UI calls.
    /// </summary>
    PointerEventData RandomPointerEventData;
    /// <summary>
    /// Extended finger count at the last frame.
    /// </summary>
    int LastFingerCount = 0;
    /// <summary>
    /// Cached hand position.
    /// </summary>
    Vector2 HandPosition = new Vector2(-1, -1);

    /// <summary>
    /// Create a ray from the camera at the given screen point.
    /// </summary>
    public static Ray ScreenPointToRay() {
        Vector2 LeapPosition = LeapMotion.Instance.PalmOnScreenXY();
        return SBS.StereoRay(LeapMotion.Instance.IsUsed() ? new Vector3(LeapPosition.x, Screen.height - LeapPosition.y) : Input.mousePosition);
    }

    /// <summary>
    /// Gets if the user tapped or clicked.
    /// </summary>
    public bool ActionDown() {
        return LeapMotion.Instance.IsUsed() ? Tapped : Input.GetMouseButtonDown(0);
    }

    /// <summary>
    /// Gets if the user grabs.
    /// </summary>
    public bool Action() {
        return LeapMotion.Instance.IsUsed() ? LastFingerCount == 0 : Input.GetMouseButton(0);
    }

    /// <summary>
    /// Gets the pointer (mouse or main hand) position on screen.
    /// </summary>
    public Vector2 ScreenPosition() {
        return LeapMotion.Instance.IsUsed() ? LeapMotion.Instance.PalmOnScreenXY() : new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
    }

    /// <summary>
    /// Gets the pointer (mouse or main hand) position on or off screen.
    /// </summary>
    public Vector2 ScreenPositionUnclamped() {
        return LeapMotion.Instance.IsUsed() ? LeapMotion.Instance.PalmOnScreenXYUnclamped() : new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
    }

    void Start() {
        RandomPointerEventData = new PointerEventData(GetComponent<EventSystem>());
    }

    void DrawPointer(Vector2 Position, bool FullSize, Texture Cursor) {
        Vector2 DrawStartPos = Position;
        if (CenterPointer)
            DrawStartPos -= MouseSize * (FullSize ? .6f : .5f);
        GUI.DrawTexture(new Rect(DrawStartPos, MouseSize * (FullSize ? 1 : .8f)), Cursor);
    }

    void OnGUI() {
        if (LeapMotion.Instance.IsUsed()) {
            DrawPointer(HandPosition, LastFingerCount != 0, MouseIcon);
            if (OffHandIcon)
                for (int OffHand = 1; OffHand < LeapMotion.Instance.GetHandCount(); ++OffHand)
                    DrawPointer(LeapMotion.Instance.PalmOnScreenXYUnclamped(OffHand), LeapMotion.Instance.ExtendedFingers(OffHand) != 0, OffHandIcon);
        }
    }

    void Update() {
        HandPosition = ScreenPosition();
        int FingerCount = LeapMotion.Instance.ExtendedFingers();
        Tapped = FingerCount == 0 && LastFingerCount != 0;
            RaycastHit hit;
        if (Physics.Raycast(SBS.StereoRay(new Vector2(HandPosition.x, Screen.height - HandPosition.y)), out hit)) {
            Selectable Hovered = hit.collider.gameObject.GetComponentInChildren<Selectable>();
            if (Hovered) {
                if (LastHovered && Hovered != LastHovered)
                    LastHovered.OnPointerExit(RandomPointerEventData);
                Hovered.OnPointerEnter(RandomPointerEventData);
                LastHovered = Hovered;
                if (ActionDown()) {
                    if (Hovered.GetType() == typeof(Button))
                        ((Button)Hovered).OnPointerClick(RandomPointerEventData);
                    else
                        Hovered.Select();
                }
            } else if (LastHovered)
                LastHovered.OnPointerExit(RandomPointerEventData);
        } else if (LastHovered)
            LastHovered.OnPointerExit(RandomPointerEventData);
        LastFingerCount = FingerCount;
    }
}