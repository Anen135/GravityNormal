using Lucy.Features.Projectiles.Runtime;
using UnityEngine;

namespace Lucy.Features.SpaceShip.Runtime
{
    public abstract class SpaceShipWeapon : MonoBehaviour
    {
        #region Inspector

        [Header("References SpaceShip Weapon")]
        public ProjectileLauncher launcher;

        #endregion

        #region Public methods

        public abstract void Fire();

        public virtual void SetTarget(Transform value) { }

        #endregion
    }
}