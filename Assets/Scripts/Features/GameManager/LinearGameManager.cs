using UnityEngine;
using UnityEngine.SceneManagement;

public class LinearGameManager : BaseGameManager {

    [Header("Scenes")]
    public string nextScene;
    public string failedScene;

    protected override void OnAllMissionsCompleted() {
        Debug.Log("=== All missions completed ===");
        if (!string.IsNullOrEmpty(nextScene)) SceneManager.LoadScene(nextScene);
    }

    protected override void OnMissionFailed() {
        Debug.Log("=== Mission failed ===");
        if (!string.IsNullOrEmpty(failedScene)) SceneManager.LoadScene(failedScene);
    }
}
