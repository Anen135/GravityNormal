using UnityEngine;

namespace Lucy.Core
{
    public abstract class GroundingProvider : MonoBehaviour
    {
        public abstract bool IsGrounded { get; }        
    }
}
