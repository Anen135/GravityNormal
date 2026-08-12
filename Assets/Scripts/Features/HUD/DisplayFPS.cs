using TMPro;
using UnityEngine;

public class DisplayFPS : MonoBehaviour
{
    [Header("References Display FPS")]
    public TMP_Text fpsText;

    [Header("Settings Display FPS")]
    [Min(0.1f)] public float updateInterval = 0.5f;

    [Header("Debug")]
    public float fps;

    float elapsedTime;
    int frameCount;

    void Update()
    {
        if (fpsText == null) return;
        elapsedTime += Time.unscaledDeltaTime;
        frameCount++;

        if (elapsedTime < updateInterval)
            return;

        fps = frameCount / elapsedTime;

        fpsText.SetText("FPS: {0:0}", fps);

        elapsedTime = 0f;
        frameCount = 0;
    }
}