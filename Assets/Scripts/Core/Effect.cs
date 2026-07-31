// Core layer
// Data Tampling
using System;
using UnityEngine;

namespace Lucy.Core
{
    public class Effect : ScriptableObject
    {
        public Action<object> OnEffect;
        public virtual void Apply(object ctx) => OnEffect?.Invoke(ctx);
        public virtual void Execute() => OnEffect?.Invoke(null);
    }
}