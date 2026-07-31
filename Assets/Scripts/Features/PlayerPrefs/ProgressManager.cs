using UnityEngine;

public static class ProgressManager
{
    const string ProgressKey = "progress";
    public static int DefaultProgress = 0;

    public static int GetProgress() => PlayerPrefs.GetInt(ProgressKey, DefaultProgress);

    public static void SetProgress(int progress)
    {
        if (GetProgress() >= progress) return;
        PlayerPrefs.SetInt(ProgressKey, progress);
    }
}