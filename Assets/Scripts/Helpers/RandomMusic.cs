using UnityEngine;
using System;

using Cavern;
using LeapVR;

using Menus;

namespace Helpers {
    /// <summary>
    /// Picks and plays a random music from the given array, and does this again after it's finished.
    /// Has a now playing popup.
    /// </summary>
    [AddComponentMenu("Helpers / Random Music")]
    [RequireComponent(typeof(AudioSource3D))]
    public class RandomMusic : MonoBehaviour {
        /// <summary>
        /// Music clip with replay gain.
        /// </summary>
        [Serializable] public struct Music {
            public AudioClip Clip;
            public float Gain;
        }

        [Tooltip("Music to pick a random from.")]
        public Music[] Files;
        [Tooltip("The key for changing music.")]
        public KeyCode Skip = KeyCode.N;

        /// <summary>
        /// The audio source to play the music on.
        /// </summary>
        AudioSource3D Source;
        /// <summary>
        /// Time to wait before starting the first music. Leave some time to the scene to initialize.
        /// </summary>
        float InitialTimeout = .25f;
        /// <summary>
        /// Artist of the current song (top line).
        /// </summary>
        string Artist = "";
        /// <summary>
        /// Song title (bottom line).
        /// </summary>
        string Song;
        /// <summary>
        /// Time remaining from displaying the now playing song.
        /// </summary>
        float DisplayTime = 0;
        /// <summary>
        /// Now playing popup frame texture.
        /// </summary>
        Texture2D InnerTexture;
        /// <summary>
        /// Now playing popup content texture.
        /// </summary>
        Texture2D OuterTexture;

        /// <summary>
        /// Create the required textures.
        /// </summary>
        void Start() {
            Source = GetComponent<AudioSource3D>();
            Source.SpatialBlend = 0;
            InnerTexture = new Texture2D(1, 1);
            InnerTexture.SetPixel(0, 0, new Color(0, 0, 0, .25f));
            InnerTexture.Apply();
            OuterTexture = new Texture2D(1, 1);
            OuterTexture.SetPixel(0, 0, new Color(1, 1, 1, .25f));
            OuterTexture.Apply();
        }

        /// <summary>
        /// Free up the textures.
        /// </summary>
        void OnDestroy() {
            Destroy(InnerTexture);
            Destroy(OuterTexture);
        }

        /// <summary>
        /// Now playing popup.
        /// </summary>
        void OnGUI() {
            if (DisplayTime > 0) {
                float Fade = Mathf.Min(1 - Mathf.Abs(DisplayTime - 2), 0) * 400;
                DisplayTime -= Time.deltaTime * .5f;
                SBS.StereoTexture(new Rect(Fade, 100, 200, 50), OuterTexture);
                SBS.StereoTexture(new Rect(Fade, 102, 198, 46), InnerTexture);
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;
                GUI.skin.label.fontSize = 14;
                GUI.skin.label.fontStyle = FontStyle.Italic;
                SBS.StereoLabel(new Rect(Fade, 105, 200, 20), Artist);
                GUI.skin.label.fontStyle = FontStyle.BoldAndItalic;
                SBS.StereoLabel(new Rect(Fade, 125, 200, 20), Song);
            }
        }

        void Update() {
            if (InitialTimeout > 0) {
                InitialTimeout -= Time.deltaTime;
                return;
            }
            if (Input.GetKeyDown(Skip) || !Settings.Music) // Trigger a skip by stopping the playback
                Source.IsPlaying = false;
            if (Settings.Music && !Source.IsPlaying) { // Play the next song if nothing is playing (either finished or interrupted)
                // Don't pick the same song again = exclude the last, and if the now playing is picked, use the last
                int Pick = UnityEngine.Random.Range(0, Files.Length - Convert.ToInt32(Source.clip != null));
                if (Files[Pick].Clip == Source.clip)
                    Pick = Files.Length - 1;
                // Disable music picker when files are missing
                if (!Files[Pick].Clip) {
                    Debug.LogError("Music files are missing.");
                    enabled = false;
                    return;
                }
                // Get title
                Song = (Source.clip = Files[Pick].Clip).name;
                if (Song.Contains(" - ")) {
                    Artist = Song.Substring(0, Song.IndexOf(" - "));
                    Song = Song.Substring(Artist.Length + 3);
                }
                // Finalize
                Source.Volume = Files[Pick].Gain;
                DisplayTime = 4;
                Source.Play();
            }
        }
    }
}