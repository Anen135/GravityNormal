using UnityEngine;

public class DebugVector2 : MonoBehaviour
{
    public Vector2 value;
    public void SetVector2(Vector2 newValue)
    {
        value = newValue;
        Debug.Log($"DebugVector2: {newValue}");
    }
}