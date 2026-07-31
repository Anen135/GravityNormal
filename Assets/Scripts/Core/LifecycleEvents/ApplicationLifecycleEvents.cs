using System;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("Events/Application Lifecycle Events")]
public sealed class ApplicationLifecycleEvents : MonoBehaviour
{
    public Action<bool> ApplicationFocusAction;
    public Action<bool> ApplicationPauseAction;
    public Action ApplicationQuitAction;

    void OnApplicationFocus(bool hasFocus) => ApplicationFocusAction?.Invoke(hasFocus);
    void OnApplicationPause(bool pauseStatus) => ApplicationPauseAction?.Invoke(pauseStatus);
    void OnApplicationQuit() => ApplicationQuitAction?.Invoke();
}
