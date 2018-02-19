using UnityEngine;

using LeapVR;

namespace Menus {
    [AddComponentMenu("Menus / Calibration")]
    class Calibration : Singleton<Calibration> {
        [Tooltip("Position in the world representing the controller.")]
        public Transform VisualizationFloor;
        [Tooltip("Raw controller values' multiplication.")]
        public float DisplayScale = .01f;

        public delegate void Result(Vector3 Minimums, Vector3 Maximums);
        /// <summary>
        /// Called when the Leap Motion bounds have changed.
        /// </summary>
        public Result CalibrationResult;

        GameObject Cube, ResultCube;

        /// <summary>
        /// Creates a cube GameObject in a given color.
        /// </summary>
        GameObject CreateCube(Color color) {
            GameObject Creation = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Creation.transform.parent = transform;
            Creation.transform.rotation = transform.rotation;
            Material CubeMat = Creation.GetComponent<Renderer>().material;
            CubeMat.shader = Shader.Find("Transparent/Diffuse");
            CubeMat.color = color;
            return Creation;
        }

        void Awake() {
            CalibrationResult = OnResult;
        }

        void Start() {
            Cube = CreateCube(new Color(1, 1, 1, .5f));
            ResultCube = CreateCube(new Color(0, 1, 0, .5f));
            ResultCube.transform.localPosition = PositionForResult(Settings.LeapLowerBounds, Settings.LeapUpperBounds);
            ResultCube.transform.localScale = ScaleForResult(Settings.LeapLowerBounds, Settings.LeapUpperBounds);
        }

        /// <summary>
        /// Calculates the center of the Leap Motion bounds.
        /// </summary>
        Vector3 PositionForResult(Vector3 Minimums, Vector3 Maximums) {
            return new Vector3((Minimums.x + Maximums.x) * .5f / transform.lossyScale.x, (Minimums.y + Maximums.y) * .5f / transform.lossyScale.y,
                (Minimums.z + Maximums.z) * .5f / transform.lossyScale.z) * DisplayScale;
        }

        /// <summary>
        /// Calculates the dimensions of the Leap Motion bounds.
        /// </summary>
        Vector3 ScaleForResult(Vector3 Minimums, Vector3 Maximums) {
            return new Vector3((Maximums.x - Minimums.x) / transform.lossyScale.x, (Maximums.y - Minimums.y) / transform.lossyScale.y,
                (Maximums.z - Minimums.z) / transform.lossyScale.z) * DisplayScale;
        }

        void OnResult(Vector3 Minimums, Vector3 Maximums) {
            ResultCube.transform.localPosition = PositionForResult(Minimums, Maximums);
            ResultCube.transform.localScale = ScaleForResult(Minimums, Maximums);
        }

        void Update() {
            int Hands = LeapMotion.Instance.GetHandCount();
            if (Hands >= 2) {
                // Get data from the controller
                Vector3 HandAPos = LeapMotion.Instance.PalmPosition(0);
                int HandAFingers = LeapMotion.Instance.ExtendedFingers(0);
                Vector3 HandBPos = LeapMotion.Instance.PalmPosition(1);
                int HandBFingers = LeapMotion.Instance.ExtendedFingers(1);
                // Find boundaries
                Vector3 Minimums = new Vector3(Mathf.Min(HandAPos.x, HandBPos.x), Mathf.Min(HandAPos.y, HandBPos.y), Mathf.Min(HandAPos.z, HandBPos.z)),
                    Maximums = new Vector3(Mathf.Max(HandAPos.x, HandBPos.x), Mathf.Max(HandAPos.y, HandBPos.y), Mathf.Max(HandAPos.z, HandBPos.z));
                // Visualize boundaries
                Cube.transform.localPosition = PositionForResult(Minimums, Maximums);
                Cube.transform.localScale = ScaleForResult(Minimums, Maximums);
                // Return results if marked complete
                if (HandAFingers == 0 && HandBFingers == 0)
                    CalibrationResult(Minimums, Maximums);
            }
        }
    }
}