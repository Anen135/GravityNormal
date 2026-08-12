using UnityEngine;

public static class MonobehaviourExtensions
{
    public static void SetEnabled(this MonoBehaviour source, bool value) => source.enabled = value;
}