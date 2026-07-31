using System;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("Events/Editor/Gizmo Events")]
public sealed class EditorEvents : MonoBehaviour
{
    public Action DrawGizmosAction;
    public Action DrawGizmosSelectedAction;
    public Action ValidateAction;
#if UNITY_EDITOR
    void OnDrawGizmos() => DrawGizmosAction?.Invoke();
    void OnDrawGizmosSelected() => DrawGizmosSelectedAction?.Invoke();
    void OnValidate() => ValidateAction?.Invoke();
#endif
}
