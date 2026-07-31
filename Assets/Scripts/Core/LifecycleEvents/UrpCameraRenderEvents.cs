using System;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Events/Rendering/URP Camera Render Events")]
public sealed class UrpCameraRenderEvents : MonoBehaviour
{
    public Action BeginCameraRenderingAction;
    public Action EndCameraRenderingAction;

    Camera targetCamera;

    void Awake() => targetCamera = GetComponent<Camera>();

    void OnEnable()
    {
        if (targetCamera == null)
            targetCamera = GetComponent<Camera>();

        RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }

    void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == targetCamera)
            BeginCameraRenderingAction?.Invoke();
    }

    void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        if (camera == targetCamera)
            EndCameraRenderingAction?.Invoke();
    }
}
