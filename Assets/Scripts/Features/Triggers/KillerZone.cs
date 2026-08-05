using Lucy.Core;
using UnityEngine;

namespace Lucy.Features.Triggers.Runtime
{
    [RequireComponent(typeof(Collider))]
    public class KillerZone : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<Entity>() is Entity entity) entity.TakeDamage(entity.MaxHealth, this);
            else Destroy(other.gameObject, 5f);   
        }
    }
}