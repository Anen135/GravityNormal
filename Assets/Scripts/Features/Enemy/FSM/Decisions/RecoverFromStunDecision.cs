using UnityEngine;
using Lucy.Core.FSM.DD;

namespace Lucy.Features.Enemy.FSM.Decisions
{
    [CreateAssetMenu(menuName = "FSM/Decisions/RecoverFromStun")]
    public class RecoverFromStunDecision : Decision
    {
        public float stunTime = 1.5f;
        private float timer;

        public override bool Decide(BaseStateMachine machine)
        {
            timer += Time.deltaTime;
            return timer >= stunTime;
        }
    }
}