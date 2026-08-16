using Lucy.Features.Audio.Runtime;
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

        [Header("Settings SpaceShip Movement Audio")]
        [Tooltip("Looping sound played while the ship is accelerating (thrusting).")]
        public AudioClip thrustLoopSound;
        [Range(0f, 1f)] public float thrustLoopVolume = 1f;
        [Range(-3f, 3f)] public float thrustLoopPitch = 1f;

        [Header("Debug")]
        public bool isStabilizing;


        #endregion

        #region Unity messages

        protected override void FixedUpdate()
        {
            isStabilizing = false;
            base.FixedUpdate();
        }

        void OnEnable()
        {
            OnThrustingChanged += HandleThrustingChanged;

            PrepareThrustAudio();
            HandleThrustingChanged(isThrusting);
        }

        void OnDisable()
        {
            OnThrustingChanged -= HandleThrustingChanged;

            if (_thrustAudioSource != null) _thrustAudioSource.Stop();
        }

        #endregion

        #region Audio

        AudioSource _thrustAudioSource;

        void PrepareThrustAudio()
        {
            if (thrustLoopSound == null || _thrustAudioSource != null) return;

            _thrustAudioSource = gameObject.AddComponent<AudioSource>();
            _thrustAudioSource.clip = thrustLoopSound;
            _thrustAudioSource.loop = true;
            _thrustAudioSource.playOnAwake = false;
            _thrustAudioSource.volume = Mathf.Clamp01(thrustLoopVolume);
            _thrustAudioSource.pitch = Mathf.Clamp(thrustLoopPitch, -3f, 3f);
            _thrustAudioSource.spatialBlend = 1f;
            _thrustAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
            _thrustAudioSource.minDistance = 1f;
            _thrustAudioSource.maxDistance = 500f;

            // Respect the global SFX volume when the audio system is present.
            Audio.Runtime.AudioSystem audioSystem = Audio.Runtime.AudioSystem.Instance;
            if (audioSystem != null && audioSystem.sfxGroup != null)
                _thrustAudioSource.outputAudioMixerGroup = audioSystem.sfxGroup;
        }

        void HandleThrustingChanged(bool value)
        {
            // Starts the loop when acceleration begins, cuts it off when it is interrupted.
            if (_thrustAudioSource == null) return;

            if (value) _thrustAudioSource.Play();
            else _thrustAudioSource.Stop();
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