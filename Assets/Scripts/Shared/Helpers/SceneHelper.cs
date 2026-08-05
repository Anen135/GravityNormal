using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lucy.Core.Helpers
{
    public class SceneHelper: MonoBehaviour
    {
        public void LoadScene(string sceneName) { if (!string.IsNullOrEmpty(sceneName)) SceneManager.LoadScene(sceneName); }
        public void LoadScene(int sceneIndex) { if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(sceneIndex); }
        public void ReloadCurrentScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        public void QuitApplication()
        {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else     
        Application.Quit();
#endif
        }
    }
}