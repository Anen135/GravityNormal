using System;
using Lucy.Core;
using UnityEngine;
using UnityEngine.Events;

public class DeathHandler : MonoBehaviour
{
    public UnityEvent OnDeath;
    public Entity entity;
    void OnEnable() => entity.OnDeath += OnEntityDeath;
    void OnDisable() => entity.OnDeath -= OnEntityDeath;
    void OnEntityDeath(object obj) { if (!gameObject.scene.isLoaded) return; OnDeath?.Invoke(); }
}