using UnityEngine;

namespace Lucy.Core.Basic
{
    public class BasicHumanoidAnimations : MonoBehaviour
    {
        public AudioClip JumpStartAudioClip;
        public AudioClip LandAudioClip;
        public AudioClip[] FootstepAudioClips;
        public virtual void OnLand()
        {
            if (LandAudioClip == null) return;
            AudioSource.PlayClipAtPoint(LandAudioClip, transform.position);
        }
        public virtual void OnFootstep()
        {
            if (FootstepAudioClips.Length == 0) return;
            AudioSource.PlayClipAtPoint(FootstepAudioClips[Random.Range(0, FootstepAudioClips.Length)], transform.position);
        }
        public virtual void OnJumpStart()
        {
            if (JumpStartAudioClip == null) return;
            AudioSource.PlayClipAtPoint(JumpStartAudioClip, transform.position);
        }   
    }
}