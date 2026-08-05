using Lucy.Core;
using UnityEngine;

public class CollitionDamage : MonoBehaviour
{
    public float damageMultiplier = 1f;
    public float trashold = 1f;
    void OnCollisionEnter(Collision col) => ApplyDamage(col);
    public void ApplyDamage(Collision col)
    {
        if (col.relativeVelocity.magnitude > trashold && gameObject.TryGetComponent(out Entity entity)) entity.TakeDamage(col.relativeVelocity.magnitude * damageMultiplier, col.gameObject);
    }
}