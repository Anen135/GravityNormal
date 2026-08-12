using System;
using Lucy.Features.Projectiles.Runtime;
using UnityEngine;

namespace Lucy.Features.SpaceShip.Runtime
{
    public class PlayerSpaceShipWeapon : SpaceShipWeapon
    {
        [Header("Settings")]
        public bool IsFiring = false;
        public float FireRate = 0.5f;
        [Header("Debug")]
        public float rate = 0.5f;
        void Update() => TimerTick(Fire);

        void TimerTick(Action tick)
        {
            if (IsFiring)
            {
                rate -= Time.deltaTime;
                if (rate <= 0)
                {
                    rate = FireRate;
                    tick();
                }
            } else
            {
                rate = 0;
            }
        }
        public override void Fire() => launcher.Launch();

        public void SetFiring(bool value) => IsFiring = value;
    }
}