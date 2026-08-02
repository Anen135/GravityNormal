using System;

namespace Lucy.Core.Input
{
    internal sealed class InputSubscription : IDisposable
    {
        Action unsubscribe;
        Action disposed;
        public bool IsDisposed { get; private set; }

        public InputSubscription( Action unsubscribe, Action disposed = null)
        {
            this.unsubscribe = unsubscribe ?? throw new ArgumentNullException(nameof(unsubscribe));
            this.disposed = disposed;
        }

        public void Dispose()
        {
            if (!IsDisposed) {
                try { unsubscribe.Invoke(); }
                finally
                {
                    disposed?.Invoke();
                    disposed = null;
                    unsubscribe = null;
                    IsDisposed = true;
                }
            }
        }
    }
}