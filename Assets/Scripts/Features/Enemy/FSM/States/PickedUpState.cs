using UnityEngine;
using Lucy.Core.FSM.DD;

namespace Lucy.Features.Enemy.FSM.States
{
    [CreateAssetMenu(menuName = "FSM/States/PickedUp")]
    public class PickedUpState : State
    {
        public override void Enter(BaseStateMachine machine)
        {
            base.Enter(machine);
            var e = machine as EnemyController;
            if (e == null) return;

            e.Agent.enabled = false;
            var PlayerHand = GameObject.FindWithTag("PlayerHand");
            e.transform.SetParent(PlayerHand.transform);
        }

        public override void Exit()
        {
            base.Exit();
            var e = Machine as EnemyController;
            if (e == null) return;

            e.transform.SetParent(null);
        }
    }
}