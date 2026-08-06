using Lucy.Features.Projectiles.Runtime;
using UnityEngine;

namespace Lucy.Features.SpaceShip.Runtime
{
    public class PlayerSpaceShipWeapon : SpaceShipWeapon
    {
        public override void Fire() => launcher.Launch();
    }
}