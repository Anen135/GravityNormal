using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Lucy.Core.FSM.Graph
{
    public abstract class State : MonoBehaviour
    {
        public string StateResult { get; set; }
        public abstract void Enter();
        public abstract IEnumerator Execute();
        public abstract void Exit();
        public virtual bool Decide() { return true; }
        public virtual void Test() { }
        public virtual void Reset() { }
    }
}