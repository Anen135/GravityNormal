using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lucy.Core.FSM.DD;

namespace Lucy.Features.Enemy.FSM.Decisions
{
    [CreateAssetMenu(menuName = "FSM/Decisions/IsInDistance")]
    public class IsInDistance : Decision
    {
        public override bool Decide(BaseStateMachine machine)
        {
            var e = machine as EnemyController;
            if (!MachineLog.Check(e)) return false;
            float dist = Vector3.Distance(e.transform.position, e.Target.transform.position);
            return dist <= e.distance;
        }
    }
}