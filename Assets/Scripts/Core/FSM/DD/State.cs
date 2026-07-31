using UnityEngine;
using System.Collections.Generic;
namespace Lucy.Core.FSM.DD
{
    [CreateAssetMenu(menuName = "FSM/State")]
    public class State : BaseState
    {
        [SerializeField] private List<FSMAction> actions = new();
        [SerializeField] private List<Transition> transitions = new();

        public override void Execute()
        {
            foreach (var action in actions) action?.Execute(Machine);
            foreach (var transition in transitions) transition?.Execute(Machine);
        }
    }
}