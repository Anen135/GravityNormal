using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Lucy.Features.PlayerController.Bindings
{
    public enum Vector2ActionType
    {
        Move,
        Look
    }

    [Serializable]
    public struct Vector2Binding : IInputBinding<Vector2ActionType>
    {
        public Vector2ActionType action;
        public InputPhase phase;
        public UnityEvent<Vector2> onEvent;

        public Vector2ActionType Action => action;
        public InputPhase Phase => phase;
    }

    [Serializable]
    public sealed class Vector2InputBinder : InputBinder<Vector2Binding, Vector2ActionType>
    {
        Dictionary< (Vector2ActionType action, InputPhase phase, Action<Vector2> callback), Subscription> subscriptions = new();
        public Subscription Performed(Vector2ActionType action, Action<Vector2> callback) => GetSubscription(action, InputPhase.Performed, callback);
        public Subscription Started(Vector2ActionType action, Action<Vector2> callback) => GetSubscription(action, InputPhase.Started, callback);
        public Subscription Canceled(Vector2ActionType action, Action<Vector2> callback) => GetSubscription(action, InputPhase.Canceled, callback);

        Subscription GetSubscription( Vector2ActionType action, InputPhase phase, Action<Vector2> callback)
        {
            var key = (action, phase, callback);
            if (subscriptions.TryGetValue(key, out Subscription subscription)) return subscription;
            subscription = phase == InputPhase.Canceled ? base.Canceled(action, _ => callback(Vector2.zero)) : phase == InputPhase.Started ? base.Started( action, context => callback(context.ReadValue<Vector2>())) : base.Performed( action, context => callback(context.ReadValue<Vector2>()));
            subscriptions.Add(key, subscription);
            return subscription;
        }

        protected override InputAction GetAction(GlobalController.PlayerActions player, Vector2ActionType action) => action switch
        {
            Vector2ActionType.Move => player.Move,
            Vector2ActionType.Look => player.Look,
            _ => throw new ArgumentOutOfRangeException()
        };

        protected override void InvokeBinding(Vector2Binding binding, InputAction.CallbackContext context) => binding.onEvent?.Invoke(binding.phase == InputPhase.Canceled ? Vector2.zero : context.ReadValue<Vector2>());
    }
}