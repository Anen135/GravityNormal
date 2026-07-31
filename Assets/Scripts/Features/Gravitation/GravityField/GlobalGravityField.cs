using System.Collections.Generic;
using Lucy.Core.Extensions;
using Lucy.Core.LifecycleEvents;
using Lucy.Shared.Extensions;
using UnityEngine;

namespace Lucy.Gravity
{
    [DisallowMultipleComponent]
    public sealed class GlobalGravityField : GravityField
    {
        [Header("Observation")]
        Dictionary<LifecycleEvents, GravityBody> observedBodies = new();

        void OnEnable() => ObserveExistingBodies();
        void OnDisable() => StopObservingAll();

        void ObserveExistingBodies()
        {
            GravityBody[] bodies = FindObjectsByType<GravityBody>(FindObjectsInactive.Include);
            foreach (GravityBody body in bodies) StartObserving(body);
        }

        void StopObservingAll()
        {
            foreach (var (lifecycle, body) in observedBodies)
            {
                Unsubscribe(lifecycle);
                body.RemoveField(this);
            }

            observedBodies.Clear();
        }

        public void StartObserving(GravityBody body) => StartObserving(body.RequireComponent<LifecycleEvents>(), body);
        public void StopObserving(GravityBody body) { if (body.TryGetComponent(out LifecycleEvents lifecycle)) StopObserving(lifecycle); }

        void StopObserving(LifecycleEvents lifecycle)
        {
            if (!observedBodies.Remove(lifecycle, out GravityBody body)) return;
            Unsubscribe(lifecycle);
            body.RemoveField(this);
        }

        void StartObserving(LifecycleEvents lifecycle, GravityBody body)
        {
            if (!observedBodies.TryAdd(lifecycle, body)) return;
            Subscribe(lifecycle);
            if (body.isActiveAndEnabled && layerMask.Contains(body.gameObject)) body.AddField(this);
        }

        void HandleBodyEnabled(LifecycleEvents sender)
        {
            if (observedBodies.TryGetValue(sender, out GravityBody body) && body.isActiveAndEnabled && layerMask.Contains(body.gameObject)) body.AddField(this);
        }

        void HandleBodyDisabled(LifecycleEvents sender)
        {
            if (observedBodies.TryGetValue(sender, out GravityBody body)) body.RemoveField(this);
        }

        void HandleBodyDestroyed(LifecycleEvents sender)
        {
            StopObserving(sender);
        }

        void Subscribe(LifecycleEvents lifecycle)
        {
            lifecycle.EnableAction += HandleBodyEnabled;
            lifecycle.DisableAction += HandleBodyDisabled;
            lifecycle.DestroyAction += HandleBodyDestroyed;
        }

        void Unsubscribe(LifecycleEvents lifecycle)
        {
            lifecycle.EnableAction -= HandleBodyEnabled;
            lifecycle.DisableAction -= HandleBodyDisabled;
            lifecycle.DestroyAction -= HandleBodyDestroyed;
        }

    }
}