using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Events/Rendering/BRP Camera Render Events")]
public sealed class BuiltInCameraRenderEvents : MonoBehaviour
{
    public Action PreCullAction;
    public Action PreRenderAction;
    public Action PostRenderAction;

    void OnPreCull() => PreCullAction?.Invoke();
    void OnPreRender() => PreRenderAction?.Invoke();
    void OnPostRender() => PostRenderAction?.Invoke();
}
