using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
[AddComponentMenu("Events/Animation/Animator Events")]
public sealed class AnimatorEvents : MonoBehaviour
{
    public Action<int> AnimatorIKAction;
    public Action AnimatorMoveAction;

    void OnAnimatorIK(int layerIndex) => AnimatorIKAction?.Invoke(layerIndex);
    void OnAnimatorMove() => AnimatorMoveAction?.Invoke();
}
