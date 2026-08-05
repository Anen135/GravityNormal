using Unity.Cinemachine;
using UnityEngine;

namespace Lucy.Core.Helpers
{
    public class SpeedCinemachineCameraModifiler : MonoBehaviour
    {
        [Header("References")]
        public CinemachineCamera cam;
        public Movement movement;

        [Header("FOV")]
        public float baseFov = 60f;
        public float maxFov = 75f;

        [Header("Speed Tuning")]
        public float speedThreshold = 5.2f;   
        public float fullFovSpeed = 8f;        

        [Header("Smoothing")]
        public float increaseSmooth = 10f;
        public float decreaseSmooth = 6f;

        [Header("Debug")] 
        public float currentFov;
        public float currentSpeed;

        void Awake()
        {
            currentFov = baseFov;
            ApplyFov(currentFov);
        }

        void LateUpdate()
        {
            if (cam == null || movement == null) return;
            float speed = movement.Velocity.magnitude;
            float targetFov = baseFov;

            currentSpeed = speed;

            if (speed > speedThreshold)
            {
                float t = Mathf.InverseLerp(speedThreshold, fullFovSpeed, speed);
                t = Mathf.SmoothStep(0f, 1f, t);
                targetFov = Mathf.Lerp(baseFov, maxFov, t);
            }

            float smooth = targetFov > currentFov ? increaseSmooth : decreaseSmooth;
            currentFov = Mathf.Lerp(currentFov, targetFov, 1f - Mathf.Exp(-smooth * Time.deltaTime));

            ApplyFov(currentFov);
        }

        void ApplyFov(float fov)
        {
            var lens = cam.Lens;
            lens.FieldOfView = fov;
            cam.Lens = lens;
        }
    }
}