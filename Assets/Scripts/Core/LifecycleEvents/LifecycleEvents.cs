using System;
using UnityEngine;

namespace Lucy.Core.LifecycleEvents
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Events/Lifecycle Events")]
    public sealed class LifecycleEvents : MonoBehaviour
    {
        public Action<LifecycleEvents> AwakeAction;
        public Action<LifecycleEvents> StartAction;
        public Action<LifecycleEvents> EnableAction;
        public Action<LifecycleEvents> DisableAction;
        public Action<LifecycleEvents> DestroyAction;

        void Awake() => AwakeAction?.Invoke(this);
        void Start() => StartAction?.Invoke(this);
        void OnEnable() => EnableAction?.Invoke(this);
        void OnDisable() => DisableAction?.Invoke(this);
        void OnDestroy() => DestroyAction?.Invoke(this);
    }
}

