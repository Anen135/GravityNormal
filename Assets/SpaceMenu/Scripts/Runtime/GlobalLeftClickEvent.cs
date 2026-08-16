using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GlobalLeftClickEvent : MonoBehaviour
{
    #region Inspector

    [Header("Events Global Left Click Event")]
    public UnityEvent onLeftClick;

    [Header("Settings Global Left Click Event")]
    public bool ignoreWhenPointerOverUI = false;

    [Header("Debug")]
    public bool leftClickPressed;

    #endregion

    void Update()
    {
        leftClickPressed = Mouse.current != null &&
                           Mouse.current.leftButton.wasPressedThisFrame;

        if (!leftClickPressed)
            return;

        if (ignoreWhenPointerOverUI &&
            UnityEngine.EventSystems.EventSystem.current != null &&
            UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        onLeftClick?.Invoke();
    }
}