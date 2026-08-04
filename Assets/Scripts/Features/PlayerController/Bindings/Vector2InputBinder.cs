using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Lucy.Features.PlayerController.Bindings
{
    public enum Vector2ActionType { Move, Look }
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
        Dictionary< (Vector2ActionType action, InputPhase phase, Action<Vector2> callback), Subscription> cachedSubscriptions = new();

        public Subscription Started(Vector2ActionType action, Action<Vector2> callback) => GetSubscription(action, InputPhase.Started, callback);

        public Subscription Performed(Vector2ActionType action, Action<Vector2> callback) => GetSubscription(action, InputPhase.Performed, callback);

        public Subscription Canceled( Vector2ActionType action, Action<Vector2> callback) => GetSubscription( action, InputPhase.Canceled, callback);

        Subscription GetSubscription( Vector2ActionType action, InputPhase phase, Action<Vector2> callback)
        {

            if (cachedSubscriptions.TryGetValue( (action, phase, callback), out Subscription subscription)) return subscription;

            // subscription = CreateSubscription( action, phase, phase == InputPhase.Canceled ? _ => callback(Vector2.zero) : context => callback(context.ReadValue<Vector2>()));
            subscription = new(this, action, phase, phase == InputPhase.Canceled ? _ => callback(Vector2.zero) : context => callback(context.ReadValue<Vector2>()));
            cachedSubscriptions.Add((action, phase, callback), subscription);

            return subscription;
        }

        protected override InputAction GetAction( GlobalController.PlayerActions player, Vector2ActionType action) => action switch
        {
            Vector2ActionType.Move => player.Move,
            Vector2ActionType.Look => player.Look,
            _ => throw new ArgumentOutOfRangeException( nameof(action), action, null)
        };

        protected override void InvokeBinding( Vector2Binding binding, InputAction.CallbackContext context) => binding.onEvent?.Invoke( binding.phase == InputPhase.Canceled ? Vector2.zero : context.ReadValue<Vector2>());
    }
}