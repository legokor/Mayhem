using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("Others / Leap Mouse")]
public class LeapMouse : Singleton<LeapMouse> {
    [Tooltip("Cursor texture.")]
    public Texture MouseIcon;
    [Tooltip("Size of the cursor.")]
    public Vector2 MouseSize = new Vector2(64, 64);
    [Tooltip("The center of the cursor is the selection.")]
    public bool CenterPointer = true;

    /// <summary>
    /// A tap happened in the last frame.
    /// </summary>
    bool Tapped = false;
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
    /// <summary>
    /// Cached hand position.
    /// </summary>
    Vector2 HandPosition = new Vector2(-1, -1);

    /// <summary>
    /// Create a ray from the camera at the given screen point.
    /// </summary>
    public static Ray ScreenPointToRay() {
        Vector2 LeapPosition = LeapMotion.Instance.PalmOnScreenXY();
        return Camera.main.ScreenPointToRay(LeapPosition.x == -1 ? Input.mousePosition : new Vector3(LeapPosition.x, Screen.height - LeapPosition.y));
    }

    /// <summary>
    /// Gets if the user tapped or clicked.
    /// </summary>
    public bool ActionDown() {
        return HandPosition.x != -1 ? Tapped : Input.GetMouseButtonDown(0);
    }

    /// <summary>
    /// Gets if the user grabs.
    /// </summary>
    public bool Action() {
        return HandPosition.x != -1 ? LastFingerCount == 0 : Input.GetMouseButton(0);
    }

    /// <summary>
    /// Gets the pointer (mouse or main hand) position on screen.
    /// </summary>
    public Vector2 ScreenPosition() {
        return HandPosition.x != -1 ? LeapMotion.Instance.PalmOnScreenXY() : new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
    }

	void Start() {
        RandomPointerEventData = new PointerEventData(GetComponent<EventSystem>());
    }

    void OnEnable() {
        Menus.Settings.LeapSetupXY();
    }

    void OnGUI() {
        if (HandPosition.x != -1) {
            Vector2 DrawStartPos = HandPosition;
            if (CenterPointer) {
                DrawStartPos -= MouseSize * .5f;
                if (LastFingerCount != 0)
                    DrawStartPos -= MouseSize * .1f;
            }
            GUI.DrawTexture(new Rect(DrawStartPos, MouseSize * (LastFingerCount != 0 ? 1 : .8f)), MouseIcon);
        }
    }

    void Update() {
        HandPosition = LeapMotion.Instance.PalmOnScreenXY();
        int FingerCount = LeapMotion.Instance.ExtendedFingers();
        Tapped = FingerCount == 0 && LastFingerCount != 0;
            RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector2(HandPosition.x, Screen.height - HandPosition.y)), out hit)) {
            Button Hovered = hit.collider.gameObject.GetComponentInChildren<Button>();
            if (Hovered) {
                if (LastHovered && Hovered != LastHovered)
                    LastHovered.OnPointerExit(RandomPointerEventData);
                Hovered.OnPointerEnter(RandomPointerEventData);
                LastHovered = Hovered;
                if (Tapped)
                    Hovered.OnPointerClick(RandomPointerEventData);
            } else if (LastHovered)
                LastHovered.OnPointerExit(RandomPointerEventData);
        } else if (LastHovered)
            LastHovered.OnPointerExit(RandomPointerEventData);
        LastFingerCount = FingerCount;
    }
}