using UnityEngine;
namespace Lucy.Core.FSM.DD
{
    [CreateAssetMenu(menuName = "FSM/Transition")]
    public class Transition : ScriptableObject
    {
        [SerializeField] private Decision decision;
        [SerializeField] private BaseState nextState;
        [SerializeField] private bool inverted;
        public void Execute(BaseStateMachine machine) { if (inverted ? !decision.Decide(machine) : decision.Decide(machine) && nextState != null) machine.ChangeState(nextState); }
    }
}