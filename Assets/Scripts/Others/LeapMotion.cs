using Leap;
using UnityEngine;

/// <summary>
/// Leap Motion handling simplified.
/// </summary>
[AddComponentMenu("Others / Leap Motion")]
public class LeapMotion : Singleton<LeapMotion> {
    [Tooltip("Bottom of hand detection bounds when used in 2D.")]
    public Vector2 LeapLowerBounds = new Vector2(-200, -112.5f);
    [Tooltip("Top of hand detection bounds when used in 2D.")]
    public Vector2 LeapUpperBounds = new Vector2(200, 112.5f);

    /// <summary>
    /// Connected Leap Motion device.
    /// </summary>
    Controller Device;

    /// <summary>
    /// A position indicating unavailable Leap Motion data (e.g. no hands are detected).
    /// </summary>
    public static readonly Vector2 NotAvailable = new Vector2(-1, -1);

    /// <summary>
    /// Connect to the device automatically on creation.
    /// </summary>
    void Awake() {
        Device = new Controller();
    }

    /// <summary>
    /// Safely disconnect the device after use.
    /// </summary>
    void OnDestroy() {
        if (Device.IsConnected)
            Device.ClearPolicy(Controller.PolicyFlag.POLICY_OPTIMIZE_HMD);
        Device.StopConnection();
    }

    /// <summary>
    /// Returns if a connected Leap Motion has been found.
    /// </summary>
    public bool Connected {
        get {
            return Device.IsConnected;
        }
    }

    /// <summary>
    /// Calculates where the given world position should be shown on screen.
    /// </summary>
    /// <param name="FromLeap">Hand position from the device</param>
    /// <returns>Screen position</returns>
    Vector2 ScreenFromLeap(Vector2 FromLeap) {
        return new Vector2(
            (Mathf.Clamp(FromLeap.x, LeapLowerBounds.x, LeapUpperBounds.x) - LeapLowerBounds.x) / (LeapUpperBounds.x - LeapLowerBounds.x) * Screen.width,
            (Mathf.Clamp(FromLeap.y, LeapLowerBounds.y, LeapUpperBounds.y) - LeapLowerBounds.y) / (LeapUpperBounds.y - LeapLowerBounds.y) * Screen.height);
    }

    /// <summary>
    /// Calculates where the given world position should be shown on screen.
    /// </summary>
    /// <param name="FromLeap">Hand position from the device</param>
    /// <returns>Viewport position</returns>
    Vector2 ViewportFromLeap(Vector2 FromLeap) {
        return new Vector2(
            (Mathf.Clamp(FromLeap.x, LeapLowerBounds.x, LeapUpperBounds.x) - LeapLowerBounds.x) / (LeapUpperBounds.x - LeapLowerBounds.x),
            (Mathf.Clamp(FromLeap.y, LeapLowerBounds.y, LeapUpperBounds.y) - LeapLowerBounds.y) / (LeapUpperBounds.y - LeapLowerBounds.y));
    }

    /// <summary>
    /// Check if the user is using the controller.
    /// </summary>
    /// <returns>True if there are any hands detected</returns>
    public bool IsUsed() {
        return Device.Frame().Hands.Count != 0;
    }

    /// <summary>
    /// Get count of hands.
    /// </summary>
    /// <returns>The number of hands the device detects</returns>
    public int GetHandCount() {
        return Device.Frame().Hands.Count;
    }

    /// <summary>
    /// Raw palm position data.
    /// </summary>
    /// <param name="HandID">Hand ID</param>
    /// <returns>Palm position, or (-1, -1) if there's no hand</returns>
    public Vector3 PalmPosition(int HandID = 0) {
        if (Device.IsConnected && Device.Frame().Hands.Count > HandID) {
            Vector Pos = Device.Frame().Hands[HandID].PalmPosition;
            return new Vector3(Pos.x, Pos.y, Pos.z);
        } else
            return Vector3.zero;
    }

    /// <summary>
    /// Palm position on screen, on a vertical plane.
    /// </summary>
    /// <param name="HandID">Hand ID</param>
    /// <returns>Palm position on screen, or (-1, -1) if there's no hand</returns>
    public Vector2 PalmOnScreenXY(int HandID = 0) {
        if (Device.IsConnected && Device.Frame().Hands.Count > HandID) {
            Hand CheckedHand = Device.Frame().Hands[HandID];
            return ScreenFromLeap(new Vector2(CheckedHand.PalmPosition.x, -CheckedHand.PalmPosition.y + LeapLowerBounds.y + LeapUpperBounds.y));
        } else {
            return NotAvailable;
        }
    }

    /// <summary>
    /// Palm position on viewport, on a vertical plane.
    /// </summary>
    /// <param name="HandID">Hand ID</param>
    /// <returns>Palm position on viewport, or (-1, -1) if there's no hand</returns>
    public Vector2 PalmOnViewportXY(int HandID = 0) {
        if (Device.IsConnected && Device.Frame().Hands.Count > HandID) {
            Hand CheckedHand = Device.Frame().Hands[HandID];
            return ViewportFromLeap(new Vector2(CheckedHand.PalmPosition.x, -CheckedHand.PalmPosition.y + LeapLowerBounds.y + LeapUpperBounds.y));
        } else {
            return NotAvailable;
        }
    }

    /// <summary>
    /// Palm position on screen, on a horizontal plane.
    /// </summary>
    /// <param name="HandID">Hand ID</param>
    /// <returns>Palm position on screen, or (-1, -1) if there's no hand</returns>
    public Vector2 PalmOnScreenXZ(int HandID = 0) {
        if (Device.IsConnected && Device.Frame().Hands.Count > HandID) {
            Hand CheckedHand = Device.Frame().Hands[HandID];
            return ScreenFromLeap(new Vector2(CheckedHand.PalmPosition.x, CheckedHand.PalmPosition.z));
        } else {
            return NotAvailable;
        }
    }

    /// <summary>
    /// Palm position on viewport, on a horizontal plane.
    /// </summary>
    /// <param name="HandID">Hand ID</param>
    /// <returns>Palm position on viewport, or (-1, -1) if there's no hand</returns>
    public Vector2 PalmOnViewportXZ(int HandID = 0) {
        if (Device.IsConnected && Device.Frame().Hands.Count > HandID) {
            Hand CheckedHand = Device.Frame().Hands[HandID];
            return ViewportFromLeap(new Vector2(CheckedHand.PalmPosition.x, CheckedHand.PalmPosition.z));
        } else {
            return NotAvailable;
        }
    }

    /// <summary>
    /// Furthest tip on screen.
    /// </summary>
    /// <param name="HandID">Hand ID</param>
    /// <returns>Furthest tip position on screen, or (-1, -1) if there's no hand</returns>
    public Vector2 SinglePointOnScreen(int HandID = 0) {
        if (Device.IsConnected && Device.Frame().Hands.Count > HandID) {
            Hand CurrentHand = Device.Frame().Hands[HandID];
            Finger Furthest = CurrentHand.Fingers[0];
            foreach (Finger CheckedFinger in CurrentHand.Fingers)
                if (Furthest.StabilizedTipPosition.z > CheckedFinger.StabilizedTipPosition.z)
                    Furthest = CheckedFinger;
            return ScreenFromLeap(new Vector2(Furthest.StabilizedTipPosition.x, Furthest.StabilizedTipPosition.y));
        } else {
            return NotAvailable;
        }
    }

    public Vector2 ScreenDelta(Vector2 CurrentPosition, ref Vector2 LastPositionHolder) {
        if (CurrentPosition == NotAvailable || LastPositionHolder == NotAvailable) {
            LastPositionHolder = CurrentPosition;
            return NotAvailable;
        }
        Vector2 Delta = CurrentPosition - LastPositionHolder;
        LastPositionHolder = CurrentPosition;
        return Delta;
    }

    /// <summary>
    /// Extended fingers for a given hand.
    /// </summary>
    /// <param name="HandID">Hand ID</param>
    /// <returns>Extended finger count</returns>
    public int ExtendedFingers(int HandID = 0) {
        int Counter = 0;
        if (Device.Frame().Hands.Count > HandID) {
            Hand CurrentHand = Device.Frame().Hands[HandID];
            foreach (Finger CheckedFinger in CurrentHand.Fingers)
                if (CheckedFinger.IsExtended)
                    Counter++;
        }
        return Counter;
    }
}