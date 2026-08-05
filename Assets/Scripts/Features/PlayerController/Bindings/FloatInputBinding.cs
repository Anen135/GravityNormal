using System;
using Lucy.Core.Input;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Lucy.Features.PlayerController.Bindings
{
    public enum FloatActionType { Roll }
    [Serializable]
    public struct FloatInputBinding : IInputBinding<FloatActionType>
    {
        public FloatActionType action;
        public InputPhase phase;
        public UnityEvent<float> callback;

        public FloatActionType Action => action;
        public InputPhase Phase => phase;
    }

    [Serializable]
    public sealed class FloatInputBinder : InputBinder<FloatInputBinding, FloatActionType>
    {
        protected override InputAction GetAction(GlobalController.PlayerActions player, FloatActionType action) => action switch
        {
            FloatActionType.Roll => player.Roll,
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };

        protected override void InvokeBinding(FloatInputBinding binding, InputAction.CallbackContext context) =>
            binding.callback.Invoke(context.ReadValue<float>());

        public Subscription Subscribe(FloatActionType action, InputPhase phase, Action<float> callback) =>
            CreateSubscription(action, phase, context => callback(context.ReadValue<float>()));
    }
}