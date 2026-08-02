using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Lucy.Features.PlayerController.Bindings
{
    public enum ButtonActionType
    {
        Sprint,
        Crouch,
        Jump,
        Attack,
        Interact
    }

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
    public sealed class ButtonInputBinder
        : InputBinder<ButtonBinding, ButtonActionType>
    {
        struct Subscription
        {
            public ButtonActionType action;
            public InputPhase phase;
            public Action<InputAction.CallbackContext> callback;

            public Subscription( ButtonActionType action, InputPhase phase, Action<InputAction.CallbackContext> callback)
            {
                this.action = action;
                this.phase = phase;
                this.callback = callback;
            }
        }

        Dictionary<Action, Subscription> externalSubscriptions = new();

        public void Subscribe(
            ButtonActionType action,
            InputPhase phase,
            Action callback)
        {
            Action<InputAction.CallbackContext> wrapper = _ => callback();

            externalSubscriptions.Add(
                callback,
                new Subscription(action, phase, wrapper));

            Subscribe(action, phase, wrapper);
        }

        public void Unsubscribe(Action callback)
        {
            if (!externalSubscriptions.Remove( callback, out Subscription subscription))
                return;

            Unsubscribe( subscription.action, subscription.phase, subscription.callback);
        }

        protected override InputAction GetAction(GlobalController.PlayerActions player, ButtonActionType action) => action switch
        {
            ButtonActionType.Sprint => player.Sprint,
            ButtonActionType.Crouch => player.Crouch,
            ButtonActionType.Jump => player.Jump,
            ButtonActionType.Attack => player.Attack,
            ButtonActionType.Interact => player.Interact,
            _ => throw new ArgumentOutOfRangeException()
        };

        protected override void InvokeBinding(ButtonBinding binding, InputAction.CallbackContext context) => binding.onEvent?.Invoke();
    }
}