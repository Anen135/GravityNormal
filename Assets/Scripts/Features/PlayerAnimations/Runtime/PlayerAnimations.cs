using Lucy.Core;
using UnityEngine;

namespace Lucy.Features.PlayerAnimations.Runtime
{
    public class PlayerAnimations : MonoBehaviour
    {
        [Header("Internal References")]
        public Animator animator;
        public Movement movement;
        void Awake() => movement.OnJumpPerformed += () => animator.SetTrigger("Jump");
        void Update()
        {
            animator.SetFloat("Speed", movement.HorizontalVelocity.magnitude);
            animator.SetBool("FreeFall", !movement.IsGrounded);
            animator.SetBool("Grounded", movement.IsGrounded);
        }
    }
}