// Core layer
// Data Tampling
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Lucy.Core
{
    [CreateAssetMenu(fileName = "New Effect", menuName = "Lucy/Effect", order = 0)]
    public class Effect : ScriptableObject
    {
        public UnityEvent<object> OnEffect;
        public virtual void Apply(object ctx) => OnEffect?.Invoke(ctx);
        public virtual void Execute() => OnEffect?.Invoke(null);
    }
}