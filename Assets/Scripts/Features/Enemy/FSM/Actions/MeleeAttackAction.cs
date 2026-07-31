using UnityEngine;
using UnityEngine.AI;
using Lucy.Core.FSM.DD;

namespace Lucy.Features.Enemy.FSM.Actions
{
    [CreateAssetMenu(menuName = "FSM/Actions/MeleeAttack")]
    public class MeleeAttackAction : FSMAction
    {

        public override void Execute(BaseStateMachine machine)
        {
            var e = machine as EnemyController;
            if (!MachineLog.Check(e)) return;
            if (Vector3.Distance(e.transform.position, e.Target.transform.position) > e.distance) return;
            if (Time.time - e.lastAttackT >= e.cooldown)
            {
                e.isCooldown = false;
                e.lastAttackT = Time.time;
                // Можно триггерить анимацию
                // e.GetComponent<Animator>().SetTrigger("Attack");
            } else
            {
                e.isCooldown = true;
            }
        }
    }
}