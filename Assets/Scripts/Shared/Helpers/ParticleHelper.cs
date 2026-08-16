using System.Collections.Generic;
using UnityEngine;

public class ParticleHelper : MonoBehaviour
{
    public List<GameObject> particles = new();
    public float sizeMultiplier = 1f;
    public List<ParticleSystem> particleSystems = new();

    public void Play()
    {
        foreach (var particle in particles)
        {
            var p = Instantiate(particle, transform.position, transform.rotation);
            p.transform.localScale *= sizeMultiplier;
        }
    }

    public void SetLoop(bool value)
    {
        foreach (var particle in particleSystems)
        {
            particle.loop = value;
        }
    }
}