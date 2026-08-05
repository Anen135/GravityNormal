using UnityEngine;

public class DamageProjectile : Projectile
{
    public float damage = 0f;
    public bool canHit = true;

    public override void ProcessHit(Collision collision)
    {
        if (!canHit) return;
        if (collision.gameObject.TryGetComponent<Lucy.Core.Entity>(out var entity))
        {
            entity.TakeDamage(damage);
            canHit = false;
        }
    }
}