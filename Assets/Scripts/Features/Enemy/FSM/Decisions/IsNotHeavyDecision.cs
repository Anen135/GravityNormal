using UnityEngine;
using Lucy.Core.FSM.DD;

namespace Lucy.Features.Enemy.FSM.Decisions
{
    [CreateAssetMenu(menuName = "FSM/Decisions/IsNotGolem")]
    public class IsHeavyDecision : Decision
    {
        public override bool Decide(BaseStateMachine machine)
        {
            return (machine as EnemyController).isHeavy;
        }
    }
}