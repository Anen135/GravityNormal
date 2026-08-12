using System;
using UnityEngine;

namespace Lucy.Core.Extensions
{
    public static class GameObjectExtensions 
    {
        public static bool TryBind<T>(this GameObject externalСomponent, Action<T> bind) where T : Component
        {
            if (!externalСomponent.TryGetComponent(out T internalComponent)) return false;
            bind(internalComponent);
            return true;
        }
    }
}