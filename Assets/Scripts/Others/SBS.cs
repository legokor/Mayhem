using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic Side-by-Side 3D.
/// </summary>
public class SBS : MonoBehaviour {
    public static float EyeDistance = .075f;
    public static float EyeRotation = 5;

    /// <summary>
    /// True if 3D cameras are active.
    /// </summary>
    static bool State = false;
    /// <summary>
    /// Pair of the main camera.
    /// </summary>
    static Camera OtherMain;
    /// <summary>
    /// Created secondary cameras.
    /// </summary>
    static List<Camera> OtherEyes = new List<Camera>();
    /// <summary>
    /// Cameras to copy.
    /// </summary>
    static List<Camera> Targets = new List<Camera>();
    /// <summary>
    /// Active SBS handler instance.
    /// </summary>
    static GameObject Holder;

    public static void AddCamera(Camera Target) {
        Targets.Add(Target);
        if (State)
            CreateCopy(Target);
    }

    public static void RemoveCamera(Camera Target) {
        Targets.Remove(Target);
        IEnumerator<Camera> OtherEnum = OtherEyes.GetEnumerator(), TargetEnum = Targets.GetEnumerator();
        while (OtherEnum.MoveNext() && TargetEnum.MoveNext()) {
            Camera TargetCam = TargetEnum.Current;
            if (TargetCam == Target) {
                Camera OtherEye = OtherEnum.Current;
                OtherEyes.Remove(OtherEye);
                Destroy(OtherEye.gameObject);
                Targets.Remove(TargetCam);
                return;
            }
        }
    }

    static void CreateCopy(Camera TargetCam) {
        GameObject NewObj = new GameObject();
        Camera OtherEye = NewObj.AddComponent<Camera>();
        OtherEye.gameObject.transform.parent = TargetCam.gameObject.transform;
        OtherEye.transform.localPosition = new Vector3(-EyeDistance, 0, 0);
        OtherEye.transform.localEulerAngles = new Vector3(0, EyeRotation, 0);
        OtherEye.clearFlags = TargetCam.clearFlags;
        OtherEye.backgroundColor = TargetCam.backgroundColor;
        OtherEye.cullingMask = TargetCam.cullingMask;
        OtherEye.rect = new Rect(0, 0, .5f, 1);
        float FovExtension = Screen.width / (float)Screen.height;
        OtherEye.fieldOfView = TargetCam.fieldOfView *= FovExtension;
        OtherEye.orthographic = TargetCam.orthographic;
        OtherEye.orthographicSize = TargetCam.orthographicSize *= FovExtension;
        TargetCam.rect = new Rect(.5f, 0, .5f, 1);
        OtherEye.depth = TargetCam.depth;
        Skybox Sky = TargetCam.gameObject.GetComponent<Skybox>();
        if (Sky)
            NewObj.AddComponent<Skybox>().material = Sky.material;
        OtherEyes.Add(OtherEye);
        if (TargetCam == Camera.main)
            OtherMain = OtherEye;
    }

    public static bool Enabled {
        get { return State; }
        set {
            if (value == State)
                return;
            if (Holder)
                Destroy(Holder);
            if (!value) {
                IEnumerator<Camera> OtherEnum = OtherEyes.GetEnumerator(), TargetEnum = Targets.GetEnumerator();
                while (OtherEnum.MoveNext() && TargetEnum.MoveNext()) {
                    Camera OtherEye = OtherEnum.Current, TargetCam = TargetEnum.Current;
                    Destroy(OtherEye.gameObject);
                    TargetCam.rect = new Rect(0, 0, 1, 1);
                    float FovExtension = Screen.height / (float)Screen.width;
                    TargetCam.fieldOfView *= FovExtension;
                    TargetCam.orthographicSize *= FovExtension;
                }
                OtherEyes.Clear();
            } else {
                (Holder = new GameObject()).AddComponent<SBS>();
                IEnumerator<Camera> TargetEnum = Targets.GetEnumerator();
                while (TargetEnum.MoveNext())
                    CreateCopy(TargetEnum.Current);
                DontDestroyOnLoad(Holder.gameObject);
            }
            State = value;
        }
    }

    void Awake() {
        Targets.Clear();
        AddCamera(Camera.main);
    }

    void Update() {
        // Re-enable through scenes
        if (OtherEyes.Count != 0 && !OtherEyes[0]) {
            OtherEyes.Clear();
            State = false;
            Enabled = true;
        }
        // S key: flip left/right eye
        if (Input.GetKeyDown(KeyCode.S)) {
            IEnumerator<Camera> OtherEnum = OtherEyes.GetEnumerator(), TargetEnum = Targets.GetEnumerator();
            while (OtherEnum.MoveNext() && TargetEnum.MoveNext()) {
                Camera OtherEye = OtherEnum.Current, TargetCam = TargetEnum.Current;
                Rect Temp = TargetCam.rect;
                TargetCam.rect = OtherEye.rect;
                OtherEye.rect = Temp;
            }
        }
    }

    public static Ray StereoRay(Vector3 Position) {
        Camera c = Camera.main;
        if (OtherMain) {
            int HalfWidth = Screen.width / 2;
            if ((c.rect.x == 0 && Position.x >= HalfWidth) || (c.rect.x != 0 && Position.x < HalfWidth))
                c = OtherMain;
        }
        return c.ScreenPointToRay(Position);
    }
}