using UnityEngine;

namespace Lucy.Features.Enemy
{
    public class MeleeAttack : Lucy.Core.Effect
    {
        Transform enemy;
        Transform player;
        float damage;
        float cooldown;
        float timer;

        public MeleeAttack(Transform enemy, Transform player, float damage, float cooldown)
        {
            this.enemy = enemy;
            this.player = player;
            this.damage = damage;
            this.cooldown = cooldown;
        }

        public override void Execute()
        {
            timer += Time.deltaTime;

            if (timer >= cooldown)
            {
                // enemy.GetComponent<Animator>()?.SetTrigger("Attack");

                timer = 0f;
            }
        }
    }
}