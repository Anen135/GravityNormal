using UnityEngine;

namespace Lucy.Features.Enemy.FSM
{
    public static class MachineLog
    {
        public static bool Check(EnemyController e)
        {
            if (e == null) 
            {
                Debug.LogWarning("[FSM] Machine is not EnemyController");
                return false;
            }
            if (e.Agent == null || e.Target == null || e.Agent.enabled == false || e.Target.enabled == false)
            {
                Debug.LogWarning($"[FSM] Agent={e.Agent}, Target={e.Target}");
                Debug.LogWarning($"[FSM] Agent.enabled={e.Agent.enabled}, Target.enabled={e.Target.enabled}");
                return false;
            }
            return true;
        }
    }
}