using UnityEngine;

namespace Lucy.Core
{
    [RequireComponent(typeof(CharacterController)), RequireComponent(typeof(GroundingProvider))]
    public class CharacterControllerMovement : Movement
    {
        #region Inspector

        [Header("Gravity Settings")]
        public float groundedGravity = -2f;
        public float gravity = -9.81f;

        [Header("Debug")]
        public Vector3 velocity;
        public bool isGrounded;
        public GroundingProvider groundProvider;

        #endregion

        #region Properties

        public CharacterController Controller { get; protected set; }
        public override Vector3 Velocity => velocity;

        #endregion

        #region Unity messages
        void Update()
        {
            isGrounded = groundProvider.IsGrounded;
            HandleMovement(Time.deltaTime);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (groundProvider == null) groundProvider = GetComponent<GroundingProvider>();
            if (Controller == null) Controller = GetComponent<CharacterController>();
        }
#endif

        #endregion

        #region Public methods

        public override void SetVelocity(Vector3 value) => velocity = value;


        public override void Jump()
        {
            if (!IsGrounded) return;
            velocity.y = Mathf.Sqrt( jumpHeight * -2f * gravity);
            OnJumpPerformed?.Invoke();
        }

        #endregion

        #region Movement

        void HandleMovement(float deltaTime)
        {
            Vector3 horizontalVelocity = CalculateTargetHorizontalVelocity();

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
