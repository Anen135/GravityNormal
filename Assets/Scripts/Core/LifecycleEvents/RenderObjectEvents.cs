using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Renderer))]
[AddComponentMenu("Events/Rendering/Render Object Events")]
public sealed class RenderObjectEvents : MonoBehaviour
{
    public Action RenderObjectAction;
    public Action WillRenderObjectAction;

    void OnRenderObject() => RenderObjectAction?.Invoke();
    void OnWillRenderObject() => WillRenderObjectAction?.Invoke();
}
