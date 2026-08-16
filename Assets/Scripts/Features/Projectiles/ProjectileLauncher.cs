using Lucy.Features.Audio.Runtime;
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

        [Header("Settings Projectile Launcher Audio")]
        [Tooltip("Sound played each time a projectile is launched.")]
        public AudioClip fireSound;
        [Range(0f, 1f)] public float fireSoundVolume = 1f;
        [Range(-3f, 3f)] public float fireSoundPitch = 1f;

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
            PlayFireSound();
            return projectile;
        }

        #endregion

        #region Audio

        void PlayFireSound()
        {
            if (fireSound == null) return;

            float volume = Mathf.Clamp01(fireSoundVolume);
            float pitch = Mathf.Clamp(fireSoundPitch, -3f, 3f);

            if (Audio.Runtime.AudioSystem.Instance != null)
                Audio.Runtime.AudioSystem.Instance.PlaySFXAt(fireSound, firePoint.position, volume, pitch);
            else
                AudioSource.PlayClipAtPoint(fireSound, firePoint.position, volume);
        }

        #endregion
    }
}