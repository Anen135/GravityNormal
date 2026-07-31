using System;
using System.Collections.Generic;

namespace Lucy.Core.Input
{
    public sealed class DisposableGroup : IDisposable
    {
        List<IDisposable> items = new();
        public bool IsDisposed;
        public void Add(IDisposable disposable)
        {
            if (disposable == null) throw new ArgumentNullException(nameof(disposable));
            if (IsDisposed) { disposable.Dispose(); throw new ObjectDisposedException(nameof(DisposableGroup)); }
            items.Add(disposable);
        }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            for (int i = items.Count - 1; i >= 0; i--) items[i]?.Dispose();
            items.Clear();
        }
    }
}