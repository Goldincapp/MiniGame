using UnityEngine;
using UnityEngine.SceneManagement;

namespace TowerDefense
{
    public class Preloader : MonoBehaviour
    {
        public void LoadNextScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}