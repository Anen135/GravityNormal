using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Projectile))]
public class ProjectileOnHit : MonoBehaviour
{
    public Projectile projectile;
    public UnityEvent<Collision> OnHit;
    void Awake() { if (projectile == null) projectile = GetComponent<Projectile>(); }
    void OnEnable() { if (projectile != null) projectile.OnHit += HandleHit; }
    void OnDisable() { if (projectile != null) projectile.OnHit -= HandleHit; }
#if UNITY_EDITOR
    void OnValidate() { if (projectile == null) projectile = GetComponent<Projectile>(); }
#endif
    void HandleHit(Collision collision) => OnHit?.Invoke(collision);
}