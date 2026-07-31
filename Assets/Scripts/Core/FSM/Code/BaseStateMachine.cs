using System;
using Lucy.Core.FSM.Code;
using Lucy.Core.FSM.DD;
using UnityEngine;

namespace Lucy.Core.FSM.Code
{
    public class BaseStateMachine<T> 
    {
        public State<T> currentState;

        public BaseStateMachine(State<T> initialState)
        {
            currentState = initialState;
            currentState?.Enter();
        }

        public void ChangeState(State<T> newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState?.Enter();
        }

        public void Tick()
        {
            if (currentState == null)
            {
                Debug.LogError("Current state is NULL!");
                return;
            }

            if (currentState.Transitions == null)
            {
                Debug.LogError($"Transitions are NULL in state {currentState}");
            }

            foreach (var transition in currentState.Transitions)
            {
                if (transition == null)
                {
                    Debug.LogError("Transition is NULL!");
                    continue;
                }
                transition.Execute(this);
            }
            currentState.Execute();
        }

        static public BaseStateMachine<T> operator +(BaseStateMachine<T> stateMachine, State<T> state)
        {
            stateMachine.ChangeState(state);
            return stateMachine;
        }

        static public BaseStateMachine<T> Factory() => new(null);
    }
}