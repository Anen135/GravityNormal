using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Renderer))]
[AddComponentMenu("Events/Rendering/Renderer Visibility Events")]
public sealed class RendererVisibilityEvents : MonoBehaviour
{
    public Action BecameVisibleAction;
    public Action BecameInvisibleAction;

    // Intentionally retained aliases for the same Unity callbacks.
    public Action BecameVisibleFromCameraAction;
    public Action BecameInvisibleFromCameraAction;

    void OnBecameVisible()
    {
        BecameVisibleAction?.Invoke();
        BecameVisibleFromCameraAction?.Invoke();
    }

    void OnBecameInvisible()
    {
        BecameInvisibleAction?.Invoke();
        BecameInvisibleFromCameraAction?.Invoke();
    }
}
