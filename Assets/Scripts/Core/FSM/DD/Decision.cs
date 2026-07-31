using UnityEngine;
namespace Lucy.Core.FSM.DD
{
    // TODO: Use Init
    public abstract class Decision : ScriptableObject
    {
        public abstract bool Decide(BaseStateMachine machine);
    }
}