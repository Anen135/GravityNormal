using UnityEngine;
using Lucy.Core.FSM.DD;

namespace Lucy.Features.Enemy.FSM.Actions
{
    [CreateAssetMenu(menuName = "FSM/Actions/ChaseRanged")]
    public class ChaseRangedAction : FSMAction
    {
        public override void Execute(BaseStateMachine machine)
        {
            var e = machine as EnemyController;
            if (e == null || !e.Agent.enabled || e.Target == null)
            {
                Debug.Log($"e is {e}, e.Agent is {e.Agent}, e.Target is {e.Target}");
            }

            float dist = Vector3.Distance(e.transform.position, e.Target.transform.position);

            if (dist > e.distance)
                e.Agent.SetDestination(e.Target.transform.position);
            else
                e.Agent.ResetPath(); 
        }
    }
}