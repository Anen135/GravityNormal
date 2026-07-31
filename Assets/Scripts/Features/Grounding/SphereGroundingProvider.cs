using UnityEngine;

namespace Lucy.Core
{
    public class SphereGroundingProvider : GroundingProvider
    {
        #region Inspector

        [Header("Settings")]
        public Transform pivot;
        [Min(0.01f)] public float radius = 0.25f;
        public LayerMask groundMask = ~0;
        public QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore;

        [Header("Debug")]
        public bool gizmosEnabled = true;

        #endregion

        Collider[] overlapBuffer = new Collider[16];

        #region Protected methods
        public override bool IsGrounded => CheckGrounded();

        public bool CheckGrounded()
        {
            if (pivot == null) return false;
            int overlapCount = Physics.OverlapSphereNonAlloc( pivot.position, radius, overlapBuffer, groundMask, triggerInteraction);
            for (int i = 0; i < overlapCount; i++)
            {
                if (overlapBuffer[i] == null || overlapBuffer[i].transform == transform || overlapBuffer[i].transform.IsChildOf(transform)) continue;
                return true;
            }
            return false;
        }

        #endregion

        #region Unity messages

        void OnDrawGizmosSelected()
        {
            if (!gizmosEnabled || pivot == null) return;
            Gizmos.DrawWireSphere(pivot.position, radius);
        }

        #endregion
    }
}
