using Lucy.Features.Projectiles.Runtime;
using UnityEngine;

namespace Lucy.Features.SpaceShip.Runtime
{
    public abstract class SpaceShipWeapon : MonoBehaviour
    {
        #region Inspector

        [Header("References")]
        public ProjectileLauncher launcher;

        #endregion

        #region Public methods

        public abstract void Fire();

        public virtual void SetTarget(GameObject value) { }

        #endregion
    }
}