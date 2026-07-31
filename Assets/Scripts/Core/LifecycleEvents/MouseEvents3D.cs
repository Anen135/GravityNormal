using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
[AddComponentMenu("Events/Mouse/Mouse Events 3D")]
public sealed class MouseEvents3D : MonoBehaviour
{
    public Action MouseDownAction;
    public Action MouseUpAction;
    public Action MouseDragAction;
    public Action MouseEnterAction;
    public Action MouseExitAction;
    public Action MouseOverAction;
    public Action MouseUpAsButtonAction;

    void OnMouseDown() => MouseDownAction?.Invoke();
    void OnMouseUp() => MouseUpAction?.Invoke();
    void OnMouseDrag() => MouseDragAction?.Invoke();
    void OnMouseEnter() => MouseEnterAction?.Invoke();
    void OnMouseExit() => MouseExitAction?.Invoke();
    void OnMouseOver() => MouseOverAction?.Invoke();
    void OnMouseUpAsButton() => MouseUpAsButtonAction?.Invoke();
}
