using UnityEngine;
using Lucy.Core;

namespace Lucy.Features.MovementSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyMovement : Movement
    {
        #region Inspector

        [Header("Acceleration Settings")]
        [Min(0f)]
        public float acceleration = 30f;

        [Range(0f, 1f)]
        public float airControl = 0.35f;

        [Header("Ground Check")]
        public Transform pivot;

        [Min(0.01f)]
        public float groundCheckRadius = 0.25f;
        public LayerMask groundMask = ~0;
        public QueryTriggerInteraction groundTriggerInteraction = QueryTriggerInteraction.Ignore;

        [Header("Debug")]
        public bool gizmosEnabled;
        public bool isGrounded;
        public bool jumpRequested;
        public Rigidbody rb;

        #endregion

        #region Properties
        public override Vector3 Velocity => rb != null ? rb.linearVelocity : Vector3.zero;
        #endregion

        #region Unity Messages

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            UpdateGroundedState();
            HandleHorizontalMovement();
            HandleJump();
            HandleRotation();
        }

        void OnDisable()
        {
            jumpRequested = false;
        }

        void OnDrawGizmosSelected()
        {
            if (!gizmosEnabled) return;
            if (groundCheck == null) return;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        #endregion

        #region Public methods

        public override void SetVelocity(Vector3 value)
        {
            if (rb == null)
                return;

            rb.linearVelocity = value;
        }

        public override void Jump()
        {
            if (!MovementAllowed)
                return;

            jumpRequested = true;
        }

        #endregion

        #region Movement

        void HandleHorizontalMovement()
        {
            Vector3 targetVelocity =
                CalculateTargetHorizontalVelocity();

            Vector3 currentVelocity = rb.linearVelocity;

            Vector3 currentHorizontalVelocity = new(
                currentVelocity.x,
                0f,
                currentVelocity.z);

            Vector3 velocityDifference =
                targetVelocity -
                currentHorizontalVelocity;

            float currentControl = isGrounded ? 1f : airControl;

            float maximumVelocityChange =
                acceleration *
                currentControl *
                Time.fixedDeltaTime;

            Vector3 velocityChange =
                Vector3.ClampMagnitude(
                    velocityDifference,
                    maximumVelocityChange);

            rb.AddForce(
                velocityChange,
                ForceMode.VelocityChange);
        }

        void HandleJump()
        {
            if (!jumpRequested)
                return;

            jumpRequested = false;

            if (!MovementAllowed)
                return;

            if (!isGrounded) return;

            Vector3 currentVelocity = rb.linearVelocity;

            if (currentVelocity.y < 0f)
            {
                currentVelocity.y = 0f;
                rb.linearVelocity = currentVelocity;
            }

            float gravityMagnitude = Mathf.Abs(Physics.gravity.y);

            float jumpVelocity = Mathf.Sqrt( jumpHeight * 2f * gravityMagnitude);
            rb.AddForce( Vector3.up * jumpVelocity, ForceMode.VelocityChange);
            isGrounded = false;
            OnJumpPerformed?.Invoke();
        }

        void HandleRotation()
        {
            Vector3 direction =
                CalculateTargetHorizontalVelocity();

            Quaternion targetRotation =
                CalculateTargetRotation(
                    rb.rotation,
                    direction,
                    Time.fixedDeltaTime);

            rb.MoveRotation(targetRotation);
        }

        void UpdateGroundedState()
        {
            if (groundCheck == null)
            {
                isGrounded = false;
                return;
            }

            isGrounded = Physics.CheckSphere( groundCheck.position, groundCheckRadius, groundMask, groundTriggerInteraction);
        }

        #endregion
    }
}