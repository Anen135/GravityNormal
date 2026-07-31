using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
[AddComponentMenu("Events/Character Controller Events")]
public sealed class CharacterControllerEvents : MonoBehaviour
{
    public Action<ControllerColliderHit> ControllerColliderHitAction;

    void OnControllerColliderHit(ControllerColliderHit hit) =>
        ControllerColliderHitAction?.Invoke(hit);
}
