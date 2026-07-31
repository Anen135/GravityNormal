using UnityEngine;
namespace Lucy.Core.FSM.DD
{
    //TODO: Use Init
    public abstract class FSMAction : ScriptableObject
    {
        public abstract void Execute(BaseStateMachine machine);
    }
}