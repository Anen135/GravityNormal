using UnityEngine;
namespace Lucy.Core.FSM.DD
{
    public class BaseStateMachine : MonoBehaviour
    {
        public BaseState InitialState;  

        private BaseState currentState;

        private void Awake()
        {
            currentState = InitialState;
            currentState?.Enter(this);
        }

        private void Update()
        {
            currentState?.Execute();
        }

        public void ChangeState(BaseState nextState)
        {
            currentState?.Exit();
            currentState = nextState;
            currentState?.Enter(this);
        }
    }
}