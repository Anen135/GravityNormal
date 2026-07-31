using UnityEngine;
using Lucy.Core.FSM.DD;

namespace Lucy.Features.Enemy.FSM.States
{
    [CreateAssetMenu(menuName = "FSM/States/KnockedDown")]
    public class KnockedDownState : State
    {
        public override void Enter(BaseStateMachine machine)
        {
            base.Enter(machine);
            var e = machine as EnemyController;
            if (e == null) return;
            e.Agent.enabled = false;
        }

        public override void Exit()
        {
            base.Exit();
            var e = Machine as EnemyController;
            if (e == null) return;
            e.Agent.enabled = true;
        }
    }
}