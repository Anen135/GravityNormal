using Lucy.Core;
using UnityEngine;

namespace Lucy.Features.PlayerController.Runtime
{
    public sealed class CameraRelativeInputAdapter : MonoBehaviour
    {
        [Header("References")]
        public PlayerController playerController;
        public Movement movement;
        public Transform headSocket;
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
            if (playerController != null) playerController.onMove.AddListener(OnMoveInput);
        }
        void OnDisable()
        {
            if (playerController != null) playerController.onMove.RemoveListener(OnMoveInput);
            currentInput = Vector2.zero;
            if (movement != null) movement.moveDirection = Vector3.zero;
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
            Vector3 cameraEuler = mainCamera.transform.eulerAngles;
            if (rotateBodyWithCamera)
            {
                transform.rotation = Quaternion.Euler( 0f, cameraEuler.y, 0f);
            }
            if (rotateHeadWithCamera && headSocket != null)
            {
                float pitch = NormalizeAngle(cameraEuler.x);
                headSocket.localRotation = Quaternion.Euler( pitch, 0f, 0f);
            }
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