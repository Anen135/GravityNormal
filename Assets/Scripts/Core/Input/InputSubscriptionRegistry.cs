using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Lucy.Core.Input
{
    public sealed class InputSubscriptionRegistry : IDisposable
    {
        readonly InputActionAsset ownedAsset;
        readonly HashSet<InputSubscription> subscriptions = new();

        bool disposed;

        public int Count => subscriptions.Count;

        public bool IsDisposed => disposed;

        public InputSubscriptionRegistry(InputActionAsset ownedAsset)
        {
            this.ownedAsset = ownedAsset ?? throw new ArgumentNullException(nameof(ownedAsset));
        }

        public IDisposable SubscribePerformed( InputAction action, Action<InputAction.CallbackContext> callback)
        {
            Validate(action, callback);
            action.performed += callback;
            return Register( () => action.performed -= callback);
        }

        public IDisposable SubscribeStarted( InputAction action, Action<InputAction.CallbackContext> callback)
        {
            Validate(action, callback);
            action.started += callback;
            return Register( () => action.started -= callback);
        }

        public IDisposable SubscribeCanceled( InputAction action, Action<InputAction.CallbackContext> callback)
        {
            Validate(action, callback);
            action.canceled += callback;
            return Register( () => action.canceled -= callback);
        }

        IDisposable Register(Action unsubscribe)
        {
            InputSubscription subscription = null;
            subscription = new InputSubscription( unsubscribe, () => { if (subscription != null) subscriptions.Remove(subscription); });
            subscriptions.Add(subscription);
            return subscription;
        }

        void Validate( InputAction action, Action<InputAction.CallbackContext> callback)
        {
            if (disposed) throw new ObjectDisposedException(nameof(InputSubscriptionRegistry));
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (callback == null) throw new ArgumentNullException(nameof(callback));
            if (action.actionMap?.asset != ownedAsset) throw new ArgumentException( $"InputAction '{action.name}' не принадлежит " + "InputActionAsset этого провайдера.", nameof(action));
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            if (subscriptions.Count == 0) return;
            InputSubscription[] activeSubscriptions = new InputSubscription[subscriptions.Count];
            subscriptions.CopyTo(activeSubscriptions);
            subscriptions.Clear();
            foreach (InputSubscription subscription in activeSubscriptions) subscription.Dispose();
        }
    }
}