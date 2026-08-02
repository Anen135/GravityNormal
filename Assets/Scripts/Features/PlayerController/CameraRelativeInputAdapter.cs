using Lucy.Core;
using Lucy.Features.PlayerController.Bindings;
using UnityEngine;

namespace Lucy.Features.PlayerController.Runtime
{
    public sealed class CameraRelativeInputAdapter : MonoBehaviour
    {
        [Header("References")]
        public PlayerController playerController;
        public Movement movement;
        public Transform headOrigin;
        public Transform bodyOrigin;
        public Camera mainCamera;

        [Header("Settings")]
        public bool rotateBodyWithCamera = true;
        public bool rotateHeadWithCamera = true;

        Vector2 currentInput;

        void Awake()
        {
            if (mainCamera == null) mainCamera = Camera.main;
        }

        void OnEnable()
        {
            playerController.vector2 .Performed(Vector2ActionType.Move, OnMoveInput).Subscribe();
            playerController.vector2 .Canceled(Vector2ActionType.Move, OnMoveInput).Subscribe();
        }

        void OnValidate()
        {
            playerController = playerController != null ? playerController : GetComponent<PlayerController>();
            if (mainCamera == null) mainCamera = Camera.main;
            if (movement == null) movement = GetComponent<Movement>();
            if (movement == null) movement = GetComponentInParent<Movement>();
            if (movement == null) movement = GetComponentInChildren<Movement>();
            if (movement == null) throw new System.InvalidOperationException("Movement component not found in the GameObject hierarchy.");

        }

        void OnDisable()
        {
            playerController.vector2 .Performed(Vector2ActionType.Move, OnMoveInput).Unsubscribe();
            playerController.vector2 .Canceled(Vector2ActionType.Move, OnMoveInput).Unsubscribe();
            currentInput = Vector2.zero;
            movement.moveDirection = Vector3.zero;
        }

        void Update()
        {
            if (mainCamera == null || movement == null) return;

            UpdateRotation();
            UpdateMovement();
        }

        void OnMoveInput(Vector2 input) => currentInput = input;

        void UpdateRotation()
        {
            if (rotateBodyWithCamera) bodyOrigin.rotation = Quaternion.Euler( 0f, mainCamera.transform.eulerAngles.y, 0f);
            if (rotateHeadWithCamera && headOrigin != null) headOrigin.localRotation = Quaternion.Euler( NormalizeAngle(mainCamera.transform.eulerAngles.x), 0f, 0f);
        }

        void UpdateMovement()
        {
            Transform cameraTransform = mainCamera.transform;

            Vector3 forward = Vector3.ProjectOnPlane( cameraTransform.forward, Vector3.up);

            Vector3 right = Vector3.ProjectOnPlane( cameraTransform.right, Vector3.up);

            if (forward.sqrMagnitude > 0.0001f) forward.Normalize();
            if (right.sqrMagnitude > 0.0001f) right.Normalize();

            Vector3 direction = forward * currentInput.y + right * currentInput.x;

            movement.moveDirection = Vector3.ClampMagnitude(direction, 1f);
        }

        static float NormalizeAngle(float angle)
        {
            if (angle > 180f) angle -= 360f;
            return angle;
        }
    }
}