using System.Collections.Generic;
using Lucy.Core.LifecycleEvents;
using UnityEngine;

namespace Lucy.Gravity
{
    [DisallowMultipleComponent]
    public sealed class GlobalGravityField : GravityField
    {
        [SerializeField]
        LayerMask affectedLayers = ~0;

        readonly Dictionary<LifecycleEvents, GravityBody> observedBodies = new();

        void OnEnable()
        {
            ObserveExistingBodies();
        }

        void OnDisable()
        {
            StopObservingAll();
        }

        void ObserveExistingBodies()
        {
            GravityBody[] bodies = FindObjectsByType<GravityBody>(FindObjectsInactive.Include);
            foreach (GravityBody body in bodies) Observe(body);
        }

        public void Observe(GravityBody body)
        {
            if (body == null)
                return;

            if (!body.TryGetComponent(out LifecycleEvents lifecycle))
            {
                Debug.LogError(
                    $"{body.name} requires {nameof(LifecycleEvents)}.",
                    body);

                return;
            }

            if (!observedBodies.TryAdd(lifecycle, body))
                return;

            lifecycle.EnableAction += HandleBodyEnabled;
            lifecycle.DisableAction += HandleBodyDisabled;
            lifecycle.DestroyAction += HandleBodyDestroyed;

            /*
             * Событие OnEnable могло произойти до подписки,
             * поэтому синхронизируем текущее состояние.
             */
            if (body.isActiveAndEnabled)
                AddField(body);
        }

        public void StopObserving(GravityBody body)
        {
            if (body == null)
                return;

            if (!body.TryGetComponent(out LifecycleEvents lifecycle))
                return;

            StopObserving(lifecycle);
        }

        void StopObserving(LifecycleEvents lifecycle)
        {
            if (lifecycle == null)
                return;

            if (!observedBodies.Remove(
                    lifecycle,
                    out GravityBody body))
            {
                return;
            }

            lifecycle.EnableAction -= HandleBodyEnabled;
            lifecycle.DisableAction -= HandleBodyDisabled;
            lifecycle.DestroyAction -= HandleBodyDestroyed;

            if (body != null)
                body.RemoveField(this);
        }

        void StopObservingAll()
        {
            foreach (
                KeyValuePair<LifecycleEvents, GravityBody> pair
                in observedBodies)
            {
                LifecycleEvents lifecycle = pair.Key;
                GravityBody body = pair.Value;

                if (lifecycle != null)
                {
                    lifecycle.EnableAction -= HandleBodyEnabled;
                    lifecycle.DisableAction -= HandleBodyDisabled;
                    lifecycle.DestroyAction -= HandleBodyDestroyed;
                }

                if (body != null)
                    body.RemoveField(this);
            }

            observedBodies.Clear();
        }

        void HandleBodyEnabled(LifecycleEvents sender)
        {
            if (!TryGetBody(sender, out GravityBody body))
                return;

            AddField(body);
        }

        void HandleBodyDisabled(LifecycleEvents sender)
        {
            if (!TryGetBody(sender, out GravityBody body))
                return;

            body.RemoveField(this);
        }

        void HandleBodyDestroyed(LifecycleEvents sender)
        {
            StopObserving(sender);
        }

        void AddField(GravityBody body)
        {
            if (Affects(body))
                body.AddField(this);
        }

        bool TryGetBody(
            LifecycleEvents sender,
            out GravityBody body)
        {
            if (sender == null)
            {
                body = null;
                return false;
            }

            if (observedBodies.TryGetValue(sender, out body))
                return body != null;

            return sender.TryGetComponent(out body);
        }

        bool Affects(GravityBody body)
        {
            if (body == null || !body.isActiveAndEnabled)
                return false;

            int layer = body.gameObject.layer;

            return
                (affectedLayers.value & (1 << layer)) != 0;
        }
    }
}