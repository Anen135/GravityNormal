using UnityEngine;

public static class ComponentExtensions
{
    public static T RequireComponent<T>(this Component source) where T : Component
    {
        return source.GetComponent<T>() ?? source.gameObject.AddComponent<T>();
    }
}