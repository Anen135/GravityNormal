using UnityEngine;

namespace Lucy.Features.SpaceShip.Runtime
{
    [RequireComponent(typeof(SpaceShipEngine))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class SpaceShipParticlesController : MonoBehaviour
    {
        #region Inspector

        [Header("References SpaceShip Particles Controller")]
        public SpaceShipEngine movement;
        public ParticleSystem[] rotationSmoke;
        public ParticleSystem[] thrustFire;
        public ParticleSystem[] brakingAura;

        #endregion

        #region Unity messages

        void OnEnable()
        {
            movement.OnThrustingChanged += HandleThrusting;
            movement.OnBrakingChanged += HandleBraking;
            movement.OnRotationApplied += HandleRotation;

            HandleThrusting(movement.isThrusting);
            HandleBraking(movement.isBraking);
        }

        void OnDisable()
        {
            movement.OnThrustingChanged -= HandleThrusting;
            movement.OnBrakingChanged -= HandleBraking;
            movement.OnRotationApplied -= HandleRotation;
        }

        #endregion

        #region Event handlers

        void HandleThrusting(bool value)
        {
            foreach (ParticleSystem particles in thrustFire)
            {
                if (value)
                {
                    particles.transform.forward = -movement.transform.forward;

                    particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    particles.Play();
                }
                else
                {
                    particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
        }

        void HandleBraking(bool value)
        {
            foreach (ParticleSystem particles in brakingAura)
            {
                if (value)
                {
                    particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    particles.Play();
                }
                else
                {
                    particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
        }

        void HandleRotation(Vector3 angularAcceleration)
        {
            foreach (ParticleSystem particles in rotationSmoke)
            {
                if (angularAcceleration.sqrMagnitude < 0.0001f)
                {
                    particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    continue;
                }

                Vector3 direction = movement.transform.TransformDirection(-angularAcceleration.normalized);
                // Вокруг X
                // particles.transform.forward = Quaternion.AngleAxis(90f, movement.transform.right) * direction;

                // // Вокруг Y
                // particles.transform.forward = Quaternion.AngleAxis(90f, movement.transform.up) * direction;

                // Вокруг Z
                particles.transform.forward = Quaternion.AngleAxis(-90f, movement.transform.forward) * direction;

                if (!particles.isEmitting) particles.Play();
            }
        }

        #endregion
    }
}