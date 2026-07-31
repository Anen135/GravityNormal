using UnityEngine;

namespace Lucy.Core.FSM.DD
{
    public class BaseState : ScriptableObject
    {
        protected BaseStateMachine Machine { get; private set; }

        public virtual void Enter(BaseStateMachine machine) => Machine = machine;

        public virtual void Execute() {}

        public virtual void Exit() => Machine = null;
    }
}
