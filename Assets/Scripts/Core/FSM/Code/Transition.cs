using Lucy.Core.FSM.Code;


namespace Lucy.Core.FSM.Code
{
    public class Transition<T> 
    {
        public State<T> nextState;
        public Decision<T> decision;
        public Transition(State<T> nextState, Decision<T> decision)
        { 
            this.nextState = nextState; 
            this.decision = decision; 
        }

        public void Execute(BaseStateMachine<T> machine) 
        { 
            if (decision.Decide()) machine.ChangeState(nextState);
        }

    }
}