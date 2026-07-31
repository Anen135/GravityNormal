using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(ParticleSystem))]
[AddComponentMenu("Events/Particles/Particle System Events")]
public sealed class ParticleSystemEvents : MonoBehaviour
{
    public Action<GameObject> ParticleCollisionAction;
    public Action ParticleTriggerAction;

    void OnParticleCollision(GameObject other) => ParticleCollisionAction?.Invoke(other);
    void OnParticleTrigger() => ParticleTriggerAction?.Invoke();
}
