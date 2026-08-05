using System;
using Lucy.Core;
using UnityEngine;

namespace Lucy.Features.SpaceShip.Runtime
{
    [RequireComponent(typeof(Rigidbody))]
    public class SpaceShipEngine : Movement
    {
        #region Inspector

        [Header("References SpaceShip Movement")]
        public Rigidbody rb;

        [Header("Settings SpaceShip Movement")]
        [Min(0f)] public float forwardAcceleration = 15f;
        [Min(0f)] public float brakingAcceleration = 20f;
        [Min(0f)] public float rotationAcceleration = 5f;

        [Header("Debug")]
        public bool isThrusting;
        public bool isBraking;
        public float rollInput;
        public Vector3 rotationInput;
        public Vector3 appliedAcceleration;
        public Vector3 appliedAngularAcceleration;

        #endregion

        #region Properties

        public override Vector3 Velocity => rb.linearVelocity;
        public override bool IsGrounded => false;

        #endregion

        #region Events

        public event Action<bool> OnThrustingChanged;
        public event Action<bool> OnBrakingChanged;
        public event Action<Vector3> OnRotationApplied;

        #endregion

        #region Unity messages

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            rb = rb != null ? rb : GetComponent<Rigidbody>();
        }
#endif

        protected virtual void FixedUpdate()
        {
            appliedAcceleration = Vector3.zero;
            appliedAngularAcceleration = Vector3.zero;

            if (!MovementAllowed)
            {
                OnRotationApplied?.Invoke(Vector3.zero);
                return;
            }

            if (isThrusting)
                appliedAcceleration += transform.forward * forwardAcceleration;

            if (isBraking && rb.linearVelocity.sqrMagnitude > 0.001f)
            {
                float acceleration = Mathf.Min(
                    brakingAcceleration,
                    rb.linearVelocity.magnitude / Time.fixedDeltaTime
                );

                appliedAcceleration -= rb.linearVelocity.normalized * acceleration;
            }

            if (appliedAcceleration.sqrMagnitude > 0.0001f)
                rb.AddForce(appliedAcceleration, ForceMode.Acceleration);

            rotationInput = GetRotationInput();

            if (rotationInput.sqrMagnitude < 0.001f)
            {
                OnRotationApplied?.Invoke(Vector3.zero);
                return;
            }

            appliedAngularAcceleration =
                new Vector3(-rotationInput.y, rotationInput.x, rotationInput.z)
                * rotationAcceleration;

            rb.AddRelativeTorque(
                appliedAngularAcceleration,
                ForceMode.Acceleration
            );

            OnRotationApplied?.Invoke(appliedAngularAcceleration);
        }

        #endregion

        #region Movement

        protected virtual Vector3 GetRotationInput()
        {
            Vector2 input = new(moveDirection.x, moveDirection.z);

            if (input.sqrMagnitude > 1f)
                input.Normalize();

            return new Vector3(input.x, input.y, rollInput);
        }

        #endregion

        #region Public methods

        public override void Jump() => SetThrusting(true);

        public void SetThrusting(bool value)
        {
            if (isThrusting == value) return;

            isThrusting = value;
            OnThrustingChanged?.Invoke(value);
        }

        public void SetBraking(bool value)
        {
            if (isBraking == value) return;

            isBraking = value;
            OnBrakingChanged?.Invoke(value);
        }

        public void Roll(float input) => rollInput = Mathf.Clamp(input, -1f, 1f);

        #endregion
    }
}   