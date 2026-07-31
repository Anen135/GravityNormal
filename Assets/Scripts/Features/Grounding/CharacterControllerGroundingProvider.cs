using UnityEngine;

namespace Lucy.Core
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerGroundingProvider : GroundingProvider
    {
        [Header("Dependencies")]
        public CharacterController controller;
        void Awake() => controller = GetComponent<CharacterController>();
        void OnValidate() => controller = GetComponent<CharacterController>();
        public override bool IsGrounded => controller is { isGrounded: true };

    }
}
