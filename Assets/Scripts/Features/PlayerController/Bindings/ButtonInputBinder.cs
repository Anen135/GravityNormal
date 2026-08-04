using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Lucy.Features.PlayerController.Bindings
{
    public enum ButtonActionType { Sprint, Crouch, Jump, Attack, Interact }

    [Serializable]
    public struct ButtonBinding : IInputBinding<ButtonActionType>
    {
        public ButtonActionType action;
        public InputPhase phase;
        public UnityEvent onEvent;

        public ButtonActionType Action => action;
        public InputPhase Phase => phase;
    }

    [Serializable]
    public sealed class ButtonInputBinder : InputBinder<ButtonBinding, ButtonActionType>
    {
        Dictionary< (ButtonActionType action, InputPhase phase, Action callback), Subscription> cachedSubscriptions = new();

        public Subscription Started( ButtonActionType action, Action callback) => GetSubscription( action, InputPhase.Started, callback);
        public Subscription Performed( ButtonActionType action, Action callback) => GetSubscription( action, InputPhase.Performed, callback);
        public Subscription Canceled(ButtonActionType action, Action callback) => GetSubscription(action, InputPhase.Canceled, callback);

        Subscription GetSubscription( ButtonActionType action, InputPhase phase, Action callback)
        {
            if (cachedSubscriptions.TryGetValue((action, phase, callback), out Subscription subscription)) return subscription;
            // subscription = CreateSubscription( action, phase, _ => callback());
            subscription = new(this, action, phase, _ => callback());
            cachedSubscriptions.Add((action, phase, callback), subscription);
            return subscription;
        }

        protected override InputAction GetAction( GlobalController.PlayerActions player, ButtonActionType action) => action switch
        {
            ButtonActionType.Sprint => player.Sprint,
            ButtonActionType.Crouch => player.Crouch,
            ButtonActionType.Jump => player.Jump,
            ButtonActionType.Attack => player.Attack,
            ButtonActionType.Interact => player.Interact,
            _ => throw new ArgumentOutOfRangeException( nameof(action), action, null)
        };

        protected override void InvokeBinding( ButtonBinding binding, InputAction.CallbackContext context) => binding.onEvent?.Invoke();
    }
}