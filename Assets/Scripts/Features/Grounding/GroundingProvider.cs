using UnityEngine;

namespace Lucy.Core.Interfaces
{
    public abstract class GroundingProvider
    {
        public abstract bool IsGrounded(Movement movement);
    }
}