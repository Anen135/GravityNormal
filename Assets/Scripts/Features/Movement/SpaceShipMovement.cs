using UnityEngine;

namespace Lucy.Features.SpaceShip.Runtime
{
    public class SpaceShipMovement : SpaceShipEngine
    {
        #region Inspector

        [Header("Settings SpaceShip Movement With Stabilizator")]
        public bool stabilizationEnabled = true;
        [Min(0.01f)] public float stabilizationTime = 0.5f;
        [Min(0f)] public float stabilizationDeadZone = 0.01f;

        [Header("Debug")]
        public bool isStabilizing;


        #endregion

        #region Unity messages

        protected override void FixedUpdate()
        {
            isStabilizing = false;
            base.FixedUpdate();
        }

        #endregion

        #region Movement

        protected override Vector3 GetRotationInput()
        {
            Vector3 input = base.GetRotationInput();

            if (!stabilizationEnabled || rotationAcceleration <= 0f) return input;

            Vector3 angularVelocity = Quaternion.Inverse(rb.rotation) * rb.angularVelocity;

            float stabilizationFactor = 1f / (rotationAcceleration * Mathf.Max(stabilizationTime, Time.fixedDeltaTime));

            if (Mathf.Abs(input.y) < 0.001f && Mathf.Abs(angularVelocity.x) > stabilizationDeadZone)
            {
                input.y = Mathf.Clamp( angularVelocity.x * stabilizationFactor, -1f, 1f );
                isStabilizing = true;
            }

            if (Mathf.Abs(input.x) < 0.001f && Mathf.Abs(angularVelocity.y) > stabilizationDeadZone)
            {
                input.x = Mathf.Clamp( -angularVelocity.y * stabilizationFactor, -1f, 1f );
                isStabilizing = true;
            }

            if (Mathf.Abs(input.z) < 0.001f && Mathf.Abs(angularVelocity.z) > stabilizationDeadZone)
            {
                input.z = Mathf.Clamp( -angularVelocity.z * stabilizationFactor, -1f, 1f );
                isStabilizing = true;
            }

            return input;
        }

        #endregion
    }
}