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
        [Min(0.001f)] public float movementThrashold = 0.001f;
        [Min(0f)] public float forwardAcceleration = 15f;
        [Min(0f)] public float brakingAcceleration = 20f;
        [Min(0f)] public float rotationAcceleration = 5f;
        [Min(0f)] public float redirectAcceleration = 20f;

        [Header("Debug")]
        public bool isThrusting;
        public bool isBraking;
        public bool isRedirecting;
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

            // Spaceship stopped
            if (!MovementAllowed)
            {
                OnRotationApplied?.Invoke(Vector3.zero);
                return;
            }
            
            if (isThrusting) appliedAcceleration += transform.forward * forwardAcceleration;

            // Spaceship in motion
            if (isBraking && rb.linearVelocity.sqrMagnitude > movementThrashold)
            {
                float acceleration = Mathf.Min( brakingAcceleration, rb.linearVelocity.magnitude / Time.fixedDeltaTime );

                appliedAcceleration -= rb.linearVelocity.normalized * acceleration;
            }

            if (isRedirecting && rb.linearVelocity.sqrMagnitude > movementThrashold)
            {
                Vector3 velocity = rb.linearVelocity;
                float speed = velocity.magnitude;
                if (speed < movementThrashold) return;
                Vector3 acceleration = (transform.forward * speed - velocity) / Time.fixedDeltaTime;
                acceleration = Vector3.ClampMagnitude(acceleration, redirectAcceleration );
                appliedAcceleration += acceleration;

            }

            if (appliedAcceleration.sqrMagnitude > movementThrashold) rb.AddForce(appliedAcceleration, ForceMode.Acceleration);

            // Spaceship rotation
            rotationInput = GetRotationInput();

            if (rotationInput.sqrMagnitude < movementThrashold)
            {
                OnRotationApplied?.Invoke(Vector3.zero);
                return;
            }

            appliedAngularAcceleration = new Vector3(-rotationInput.y, rotationInput.x, rotationInput.z) * rotationAcceleration;
            rb.AddRelativeTorque( appliedAngularAcceleration, ForceMode.Acceleration );
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

        public void SetRedirecting(bool value)
        {
            if (isRedirecting == value) return;
            isRedirecting = value;
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