using UnityEngine;
using Lucy.Core.FSM.DD;

namespace Lucy.Features.Enemy.FSM.Decisions
{
    [CreateAssetMenu(menuName = "FSM/Decisions/IsCooldownOverude")]
    public class IsCooldownOverude : Decision
    {

        public override bool Decide(BaseStateMachine machine)
        {
            var e = machine as EnemyController;
            if (!MachineLog.Check(e)) return false;   
            return Time.time - e.lastAttackT > e.cooldown;
        }
    }
}