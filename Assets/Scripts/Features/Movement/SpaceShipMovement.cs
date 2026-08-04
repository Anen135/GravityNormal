using Lucy.Core;
using UnityEngine;

namespace Lucy.Features.SpaceShip.Runtime
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class SpaceShipMovement : Movement
    {
        #region Inspector

        [Header("References SpaceShip Movement")]
        public Rigidbody rb;

        [Header("Settings SpaceShip Movement")]
        [Min(0f)] public float forwardAcceleration = 15f;
        [Min(0f)] public float rotationAcceleration = 5f;

        [Header("Settings SpaceShip Stabilization")]
        public bool stabilizationEnabled = true;
        [Min(0.01f)] public float stabilizationTime = 0.5f;
        [Min(0f)] public float stabilizationDeadZone = 0.01f;

        [Header("Debug")]
        public bool isThrusting;
        public bool isStabilizing;
        public Vector2 rotationInput;
        public Vector3 appliedAcceleration;
        public Vector3 appliedAngularAcceleration;

        #endregion

        #region Properties

        public override Vector3 Velocity => rb.linearVelocity;
        public override bool IsGrounded => false;

        #endregion

        #region Unity messages

#if UNITY_EDITOR
        void OnValidate()
        {
            rb = rb != null ? rb : GetComponent<Rigidbody>();
        }
#endif

        void FixedUpdate()
        {
            isStabilizing = false;
            rotationInput = Vector2.zero;
            appliedAcceleration = Vector3.zero;
            appliedAngularAcceleration = Vector3.zero;

            if (!MovementAllowed) return;

            if (isThrusting)
            {
                appliedAcceleration = transform.forward * forwardAcceleration;
                rb.AddForce(appliedAcceleration, ForceMode.Acceleration);
            }

            rotationInput = new Vector2(moveDirection.x, moveDirection.z);
            if (rotationInput.sqrMagnitude > 1f) rotationInput.Normalize();

            if (stabilizationEnabled && rotationAcceleration > 0f && rotationInput.sqrMagnitude < 0.0001f)
            {
                Vector3 angularVelocity = Quaternion.Inverse(rb.rotation) * rb.angularVelocity;

                if (new Vector2(angularVelocity.x, angularVelocity.y).sqrMagnitude > stabilizationDeadZone * stabilizationDeadZone)
                {
                    rotationInput = new Vector2(-angularVelocity.y, angularVelocity.x) / (rotationAcceleration * Mathf.Max(stabilizationTime, Time.fixedDeltaTime));

                    rotationInput = Vector2.ClampMagnitude(rotationInput, 1f);
                    isStabilizing = true;
                }
            }

            if (rotationInput.sqrMagnitude < 0.0001f) return;

            appliedAngularAcceleration = new Vector3( -rotationInput.y, rotationInput.x, 0f ) * rotationAcceleration;

            rb.AddRelativeTorque(appliedAngularAcceleration, ForceMode.Acceleration);
        }

        #endregion

        #region Public methods

        public override void Jump() => isThrusting = true;

        public void SetThrusting(bool value) => isThrusting = value;

        #endregion
    }
}