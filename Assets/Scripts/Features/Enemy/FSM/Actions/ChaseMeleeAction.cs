using UnityEngine;
using Lucy.Core.FSM.DD;

namespace Lucy.Features.Enemy.FSM.Actions
{
    [CreateAssetMenu(menuName = "FSM/Actions/ChaseMelee")]
    public class ChaseMeleeAction : FSMAction
    {
        public override void Execute(BaseStateMachine machine)
        {
            var e = machine as EnemyController;
            if (!MachineLog.Check(e)) return;        
            e.Agent.SetDestination(e.Target.transform.position);
        }
    }
}