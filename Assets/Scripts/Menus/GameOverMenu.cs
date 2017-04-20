using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

namespace Menus {
    [AddComponentMenu("Menus / Game Over")]
    public class GameOverMenu : MonoBehaviour {
        public Text Score, ScoreShadow;

        /// <summary>
        /// Show the player's score on the Game Over UI.
        /// </summary>
        /// <param name="Score">The player's score</param>
        public void DisplayScore(int Score) {
            int TopScore = Profile.TopScore;
            if (Score > TopScore) {
                this.Score.text = ScoreShadow.text = new StringBuilder("New top score: ").Append(Score).ToString();
                Profile.TopScore = Score;
            } else {
                this.Score.text = ScoreShadow.text = new StringBuilder("Score: ").Append(Score).Append(", top: ").Append(TopScore).ToString();
            }
        }

        /// <summary>
        /// Play the same level again.
        /// </summary>
        public void Retry() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Exit to the main menu.
        /// </summary>
        public void Menu() {
            SceneManager.LoadScene(0);
        }
    }
}