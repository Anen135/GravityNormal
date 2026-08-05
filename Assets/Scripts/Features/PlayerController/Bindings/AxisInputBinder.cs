using System;
using Lucy.Core;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Lucy.Features.PlayerController.Bindings
{
    public enum AxisActionType { Roll, Thrust }

    [Serializable]
    public struct AxisInputBinding : IInputBinding<AxisActionType>
    {
        public AxisActionType action;
        public InputPhase phase;
        public UnityEvent<float> callback;

        public AxisActionType Action => action;
        public InputPhase Phase => phase;
    }

    [Serializable]
    public sealed class AxisInputBinder : InputBinder<AxisInputBinding, AxisActionType>
    {
        protected override InputAction GetAction(GlobalController.PlayerActions player, AxisActionType action) => action switch
        {
            AxisActionType.Roll => player.Roll,
            AxisActionType.Thrust => player.Thrust,
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };

        protected override void InvokeBinding(AxisInputBinding binding, InputAction.CallbackContext context) => binding.callback.Invoke(context.ReadValue<float>());

        public Subscription Subscribe(AxisActionType action, InputPhase phase, Action<float> callback)
        {
            Subscription subscription = CreateSubscription(action, phase, context => callback(context.ReadValue<float>()));
            subscription.Subscribe();
            return subscription;
        }
    }
}