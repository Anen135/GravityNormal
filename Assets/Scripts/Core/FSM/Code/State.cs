using System;
using System.Collections.Generic;

namespace Lucy.Core.FSM.Code
{
   public class State<T>
    {
        public T Context;
        public List<Transition<T>> Transitions { get; set; } = new List<Transition<T>>();
        public State(T context) => Context = context;
        public virtual void Enter() { }
        public virtual void Execute() { }
        public virtual void Exit() { }
        public void AddTransition(State<T> nextState, Decision<T> condition) => Transitions.Add(new Transition<T>(nextState, condition));
        public void AddTransition(State<T> nextState, params Decision<T>[] conditions)
        {
            foreach (var condition in conditions)
                Transitions.Add(new Transition<T>(nextState, condition));
        }
    }
}
