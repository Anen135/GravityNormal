using System;
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("Events/Update Events")]
public sealed class UpdateEvents : MonoBehaviour
{
    public Action UpdateAction;
    public Action LateUpdateAction;
    public Action FixedUpdateAction;

    void Update() => UpdateAction?.Invoke();
    void LateUpdate() => LateUpdateAction?.Invoke();
    void FixedUpdate() => FixedUpdateAction?.Invoke();
}
