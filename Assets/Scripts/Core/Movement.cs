using System;
using UnityEngine;

namespace Lucy.Core
{
    [RequireComponent(typeof(GroundingProvider))]
    public class Movement : MonoBehaviour
    {
        #region Inspector

        [Header("Movement Settings")]
        [Min(0f)] public float baseSpeed = 5f;
        [Min(0f)] public float sprintMultiplier = 1.5f;
        [Min(0f)] public float crouchMultiplier = 0.5f;
        [Min(0f)] public float jumpHeight = 1.5f;

        [Header("Rotation Settings")]
        public bool rotateTowardsMovement;
        [Min(0f)] public float rotationSpeed = 720f;

        [Header("Grounding Settings")]
        public GroundingProvider groundProvider;

        [Header("Debug")]
        public Vector3 moveDirection;
        public bool isSprinting;
        public bool isCrouching;
        public bool canMove = true;
        public bool isMovementLocked;

        #endregion

        #region Properties

        public virtual Vector3 Velocity => Vector3.zero;
        public virtual bool IsGrounded => groundProvider.IsGrounded;
        public bool IsMoving => moveDirection.sqrMagnitude > 0.01f;
        public bool MovementAllowed => canMove && !isMovementLocked;

        public float CurrentSpeed
        {
            get
            {
                float speed = baseSpeed;
                if (isSprinting) speed *= sprintMultiplier;
                if (isCrouching) speed *= crouchMultiplier;
                return speed;
            }
        }

        public Vector3 HorizontalVelocity
        {
            get
            {
                Vector3 value = Velocity;
                value.y = 0f;
                return value;
            }
        }

        #endregion

        #region Events

        public Action OnJumpPerformed;
        public Action OnCrouchPerformed;
        public Action OnSprintPerformed;

        #endregion

        #region Unity messages

        protected virtual void OnValidate()
        {
            if (groundProvider == null) groundProvider = GetComponent<GroundingProvider>();
        }

        #endregion

        #region Public methods

        public void SetSprinting(bool value) => isSprinting = value;

        public void SetCrouching(bool value) => isCrouching = value;

        public virtual void SetVelocity(Vector3 value) => moveDirection = value;

        public virtual void Jump() => moveDirection.y = jumpHeight;

        #endregion

        #region Protected methods
        public Vector3 CalculateTargetHorizontalVelocity() => !MovementAllowed ? Vector3.zero : moveDirection * CurrentSpeed;

        public Quaternion CalculateTargetRotation(Quaternion currentRotation, Vector3 direction, float deltaTime)
        {
            direction.y = 0f;
            return (!rotateTowardsMovement || direction.sqrMagnitude < 0.01f) ? currentRotation : Quaternion.RotateTowards( currentRotation, Quaternion.LookRotation(direction.normalized), rotationSpeed * deltaTime);
        }

        #endregion
    }
}
