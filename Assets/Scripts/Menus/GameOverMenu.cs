using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menus {
    public class GameOverMenu : MonoBehaviour {
        public void Retry() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Menu() {
            SceneManager.LoadScene(0);
        }
    }
}