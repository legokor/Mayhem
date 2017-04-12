using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Menus {
    [AddComponentMenu("Menus / Game Over")]
    public class GameOverMenu : MonoBehaviour {
        public Text Score, ScoreShadow;

        public void DisplayScore(int Score) {
            this.Score.text = ScoreShadow.text = "Score: " + Score;
        }

        public void Retry() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Menu() {
            SceneManager.LoadScene(0);
        }
    }
}