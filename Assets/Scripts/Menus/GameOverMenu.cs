using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;

namespace Menus {
    [AddComponentMenu("Menus / Game Over")]
    public class GameOverMenu : MonoBehaviour {
        public Text Score, ScoreShadow;

        public void DisplayScore(int Score) {
            int TopScore = Profile.TopScore;
            if (Score > TopScore) {
                this.Score.text = ScoreShadow.text = new StringBuilder("New top score: ").Append(Score).ToString();
                Profile.TopScore = Score;
            } else {
                this.Score.text = ScoreShadow.text = new StringBuilder("Score: ").Append(Score).Append(", top: ").Append(TopScore).ToString();
            }
        }

        public void Retry() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Menu() {
            SceneManager.LoadScene(0);
        }
    }
}