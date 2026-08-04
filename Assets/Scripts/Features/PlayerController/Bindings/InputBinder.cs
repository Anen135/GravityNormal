using System;
using System.Collections.Generic;
using Lucy.Core;
using Lucy.Core.Input;
using UnityEngine.InputSystem;

namespace Lucy.Features.PlayerController.Bindings
{
    [Serializable]
    public abstract class InputBinder<binding_t, action_t> where binding_t : struct, IInputBinding<action_t> where action_t : struct, Enum
    {
        public List<binding_t> bindings = new();
        Dictionary<(action_t action, InputPhase phase), Action<InputAction.CallbackContext>> subscribers = new();

        public void Bind(GlobalController.PlayerActions player, DisposableGroup subscriptions)
        {
            foreach (action_t actionType in Enum.GetValues(typeof(action_t)))
            {
                InputAction action = GetAction(player, actionType);
                foreach (InputPhase phase in Enum.GetValues(typeof(InputPhase))) subscriptions.Add(SubscribeInput(action, phase, context => Dispatch(actionType, phase, context)));
            }
        }

        void Subscribe(action_t action, InputPhase phase, Action<InputAction.CallbackContext> callback)
        {
            subscribers.TryGetValue((action, phase), out Action<InputAction.CallbackContext> callbacks);
            subscribers[(action, phase)] = callbacks + callback;
        }

        void Unsubscribe(action_t action, InputPhase phase, Action<InputAction.CallbackContext> callback)
        {
            var key = (action, phase);
            if (!subscribers.TryGetValue(key, out Action<InputAction.CallbackContext> callbacks)) return;
            callbacks -= callback;
            if (callbacks == null) subscribers.Remove(key);
            else subscribers[key] = callbacks;
        }

        void Dispatch(action_t action, InputPhase phase, InputAction.CallbackContext context)
        {
            foreach (binding_t binding in bindings)
            {
                if (!EqualityComparer<action_t>.Default.Equals(binding.Action, action)) continue;
                if (binding.Phase != phase) continue;
                InvokeBinding(binding, context);
            }

            if (subscribers.TryGetValue((action, phase), out Action<InputAction.CallbackContext> callbacks)) callbacks?.Invoke(context);

        }

        IDisposable SubscribeInput(InputAction action, InputPhase phase, Action<InputAction.CallbackContext> callback) => phase switch
        {
            InputPhase.Started => InputProvider.Global.SubscribeStarted(action, callback),
            InputPhase.Performed => InputProvider.Global.SubscribePerformed(action, callback),
            InputPhase.Canceled => InputProvider.Global.SubscribeCanceled(action, callback),
            _ => throw new ArgumentOutOfRangeException(nameof(phase), phase, null)
        };

        protected Subscription CreateSubscription(action_t action, InputPhase phase, Action<InputAction.CallbackContext> callback) => new(this, action, phase, callback);

        #region Abstract Methods
        protected abstract InputAction GetAction(GlobalController.PlayerActions player, action_t action);
        protected abstract void InvokeBinding(binding_t binding, InputAction.CallbackContext context);

        #endregion

        #region API

        public sealed class Subscription
        {
            InputBinder<binding_t, action_t> binder;
            action_t action;
            InputPhase phase;
            Action<InputAction.CallbackContext> callback;

            public bool IsSubscribed { get; private set; }

            internal Subscription(InputBinder<binding_t, action_t> binder, action_t action, InputPhase phase, Action<InputAction.CallbackContext> callback)
            {
                this.binder = binder;
                this.action = action;
                this.phase = phase;
                this.callback = callback;
            }

            public void Subscribe()
            {
                if (IsSubscribed) return;
                binder.Subscribe(action, phase, callback);
                IsSubscribed = true;
            }

            public void Unsubscribe()
            {
                if (!IsSubscribed) return;
                binder.Unsubscribe(action, phase, callback);
                IsSubscribed = false;
            }
        }

        #endregion
    }
}