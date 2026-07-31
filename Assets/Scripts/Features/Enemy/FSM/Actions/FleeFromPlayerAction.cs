using UnityEngine;
using Lucy.Core.FSM.DD;

namespace Lucy.Features.Enemy.FSM.Actions
{
    [CreateAssetMenu(menuName = "FSM/Actions/FleeFromPlayer")]
    public class FleeFromAction : FSMAction
    {
        public float fleeDistance = 6f;

        public override void Execute(BaseStateMachine machine)
        {
            var e = machine as EnemyController;
            if (e == null || !e.Agent.enabled || e.Target == null)
            {
                Debug.Log($"e is {e}, e.Agent is {e.Agent}, e.Target is {e.Target}");
            }

            Vector3 dir = (e.transform.position - e.Target.transform.position).normalized;
            Vector3 fleePos = e.transform.position + dir * fleeDistance;

            e.Agent.SetDestination(fleePos);
        }
    }
}