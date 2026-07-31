using System;
using Lucy.Core;
using UnityEngine;

namespace Lucy.Features.MovementSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyMovement : Movement
    {
        #region Inspector

        [Header("Rigidbody Settings")]
        [Min(0f)] public float acceleration = 30f;
        [Range(0f, 1f)] public float airControl = 0.35f;

        [Header("Debug")]
        public bool jumpRequested;
        public bool isGrounded;
        public Rigidbody rb;

        #endregion

        #region Properties


        public override Vector3 Velocity =>
            rb != null ? rb.linearVelocity : Vector3.zero;

        #endregion

        #region Unity messages

        void Awake() => rb = GetComponent<Rigidbody>();

        void FixedUpdate()
        {
            RefreshGrounding();
            HandleHorizontalMovement();
            HandleJump();
            HandleRotation();
        }

        void RefreshGrounding() => isGrounded = IsGrounded;
        void OnDisable() => jumpRequested = false;

        #endregion

        #region Public methods

        public override void SetVelocity(Vector3 value)
        {
            if (rb == null) return;
            rb.linearVelocity = value;
        }

        public override void Jump()
        {
            if (!MovementAllowed) return;
            jumpRequested = true;
        }

        #endregion

        #region Movement

        void HandleHorizontalMovement() => rb.AddForce(Vector3.ClampMagnitude(CalculateTargetHorizontalVelocity() - new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z), (float)(acceleration * (float)(IsGrounded ? 1f : airControl) * Time.fixedDeltaTime)), ForceMode.VelocityChange);

        void HandleJump()
        {
            if (!jumpRequested) return;
            jumpRequested = false;
            if (!MovementAllowed) return;
            if (!IsGrounded) return;
            Vector3 currentVelocity = rb.linearVelocity;
            if (currentVelocity.y < 0f)
            {
                currentVelocity.y = 0f;
                rb.linearVelocity = currentVelocity;
            }
            rb.AddForce( Vector3.up * (float)Mathf.Sqrt( jumpHeight * 2f * (float)Mathf.Abs(Physics.gravity.y)), ForceMode.VelocityChange);
            OnJumpPerformed?.Invoke();
        }

        void HandleRotation()
        {
            Vector3 direction = CalculateTargetHorizontalVelocity();
            Quaternion targetRotation = CalculateTargetRotation( rb.rotation, direction, Time.fixedDeltaTime);
            rb.MoveRotation(targetRotation);
        }

        #endregion
    }
}
