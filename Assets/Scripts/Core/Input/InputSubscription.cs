using System;

namespace Lucy.Core.Input
{
    internal sealed class InputSubscription : IDisposable
    {
        private Action unsubscribe;
        private Action disposed;

        public bool IsDisposed { get; private set; }

        public InputSubscription(
            Action unsubscribe,
            Action disposed = null)
        {
            this.unsubscribe = unsubscribe
                ?? throw new ArgumentNullException(nameof(unsubscribe));

            this.disposed = disposed;
        }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            Action unsubscribeAction = unsubscribe;
            Action disposedAction = disposed;

            unsubscribe = null;
            disposed = null;

            try
            {
                unsubscribeAction.Invoke();
            }
            finally
            {
                disposedAction?.Invoke();
            }
        }
    }
}