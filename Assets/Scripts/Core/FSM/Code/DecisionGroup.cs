using System.Collections.Generic;
using System.Linq;

namespace Lucy.Core.FSM.Code
{
    public enum LogicGate { AND, OR }
    public class DecisionGroup<T> : Decision<T>
    {
        public LogicGate gate = LogicGate.AND;
        public List<Decision<T>> decisions = new();
        public DecisionGroup(T context, List<Decision<T>> decisions) : base(context) => this.decisions = decisions;
        public DecisionGroup(T context, params Decision<T>[] decisions) : base(context) => this.decisions = decisions.ToList();

        public override bool Condition()
        {
            if (decisions == null || decisions.Count == 0) return false;
            return gate switch
            {
                LogicGate.AND => decisions.All(d => d != null && d.Decide()),
                LogicGate.OR  => decisions.Any(d => d != null && d.Decide()),
                _             => false
            };
        }
    }
}