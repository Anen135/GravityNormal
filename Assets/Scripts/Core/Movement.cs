using System;
using Lucy.Core.Interfaces;
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
        public virtual Vector3 Velocity { get; set;}
        public virtual bool IsGrounded => groundProvider.IsGrounded(this);
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
        public Vector3 MovementDirection
        {
            get => moveDirection;
            set
            {
                value.y = 0f;
                moveDirection = value;
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
        public void SetSprinting(bool value)
        {
            isSprinting = value;
        }
        public void SetCrouching(bool value)
        {
            isCrouching = value;
        }
        public virtual void SetVelocity(Vector3 value) => moveDirection = value;
        public virtual void Jump() => moveDirection.y = jumpHeight;
        #endregion
        #region Protected methods
        public Vector3 CalculateTargetHorizontalVelocity()
        {
            if (!MovementAllowed)
                return Vector3.zero;
            return moveDirection * CurrentSpeed;
        }
        public Quaternion CalculateTargetRotation(
            Quaternion currentRotation,
            Vector3 direction,
            float deltaTime)
        {
            direction.y = 0f;
            if (!rotateTowardsMovement ||
                direction.sqrMagnitude < 0.01f)
            {
                return currentRotation;
            }
            Quaternion targetRotation =
                Quaternion.LookRotation(direction.normalized);
            return Quaternion.RotateTowards(
                currentRotation,
                targetRotation,
                rotationSpeed * deltaTime);
        }
        #endregion
    }
}