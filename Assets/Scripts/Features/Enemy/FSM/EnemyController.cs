using UnityEngine;
using UnityEngine.AI;
using Lucy.Core.FSM.DD;
using Lucy.Core;

namespace Lucy.Features.Enemy.FSM
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : BaseStateMachine
    {
        [Header("Config")]
        public string targetTag = "Player";
        public bool isHeavy = false; // Can be picked up

        [Header("States and Actions")]
        public float distance = 0f; // ChaseDistance, RangeDistance
        public float lastAttackT = 0f;
        public float cooldown = 0f;
        
        [Header("Components")]
        public NavMeshAgent Agent; 
        public Entity Target;

        [Header("Debug")]
        public bool isPickedUp = false;
        public bool isCooldown = false;
        void Reset()
        {
            Agent = GetComponent<NavMeshAgent>();
            Target = GameObject.FindWithTag(targetTag)?.GetComponent<Entity>();
        }
    }
}