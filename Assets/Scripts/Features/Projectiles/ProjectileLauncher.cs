using UnityEngine;

namespace Lucy.Features.Projectiles.Runtime
{
    public class ProjectileLauncher : MonoBehaviour
    {
        #region Inspector

        [Header("References Projectile Launcher")]
        public Projectile projectilePrefab;
        public Transform firePoint;
        public Rigidbody ownerRigidbody;
        public Collider[] ownerColliders;

        [Header("Settings Projectile Launcher")]
        [Min(0f)] public float launchPower = 30f;

        #endregion

        #region Unity messages

#if UNITY_EDITOR
        void OnValidate()
        {
            ownerRigidbody = ownerRigidbody != null ? ownerRigidbody : GetComponentInParent<Rigidbody>();

            ownerColliders = ownerRigidbody != null ? ownerRigidbody.GetComponentsInChildren<Collider>(true) : ownerColliders;
        }
#endif

        #endregion

        #region Public methods

        public Projectile Launch()
        {
            Projectile projectile = Instantiate( projectilePrefab, firePoint.position, firePoint.rotation );

            Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
            Collider[] projectileColliders = projectile.GetComponentsInChildren<Collider>(true);

            foreach (Collider ownerCollider in ownerColliders)
            foreach (Collider projectileCollider in projectileColliders)
                Physics.IgnoreCollision(ownerCollider, projectileCollider);

            projectileRigidbody.linearVelocity = ownerRigidbody.GetPointVelocity(firePoint.position);

            projectile.Launch(launchPower);
            return projectile;
        }

        #endregion
    }
}