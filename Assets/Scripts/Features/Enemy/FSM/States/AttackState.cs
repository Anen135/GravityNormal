using UnityEngine;
using Lucy.Core.FSM.DD;

namespace Lucy.Features.Enemy.FSM.States
{
    [CreateAssetMenu(menuName = "FSM/States/Attack")]
    public class AttackState : State
    {
        public override void Enter(BaseStateMachine machine)
        {
            base.Enter(machine);
        }

        public override void Exit()
        {
        }
    }
}