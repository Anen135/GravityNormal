using UnityEngine;

namespace Lucy.Core
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerMovement : Movement
    {
        #region Inspector

        [Header("Gravity Settings")]
        public float groundedGravity = -2f;

        public float gravity = -9.81f;

        [Header("Debug")]
        public Vector3 velocity;

        #endregion

        #region Properties

        public CharacterController Controller { get; protected set; }

        public override Vector3 Velocity => velocity;

        public override bool IsGrounded => Controller != null && Controller.isGrounded;

        #endregion

        #region Unity events

        void Awake()
        {
            Controller = GetComponent<CharacterController>();
        }

        void Update()
        {
            HandleMovement(Time.deltaTime);
        }

        #endregion

        #region Public methods

        public override void SetVelocity(Vector3 value)
        {
            velocity = value;
        }

        public override void Jump()
        {
            if (!MovementAllowed)
                return;

            if (!IsGrounded)
                return;

            velocity.y = Mathf.Sqrt(
                jumpHeight * -2f * gravity);

            OnJumpPerformed?.Invoke();
        }

        #endregion

        #region Movement

        void HandleMovement(float deltaTime)
        {
            Vector3 horizontalVelocity =
                CalculateTargetHorizontalVelocity();

            velocity.x = horizontalVelocity.x;
            velocity.z = horizontalVelocity.z;

            HandleGravity(deltaTime);

            Controller.Move(velocity * deltaTime);

            transform.rotation = CalculateTargetRotation(
                transform.rotation,
                horizontalVelocity,
                deltaTime);
        }

        void HandleGravity(float deltaTime)
        {
            if (IsGrounded && velocity.y < 0f)
            {
                velocity.y = groundedGravity;
                return;
            }

            velocity.y += gravity * deltaTime;
        }

        #endregion
    }
}