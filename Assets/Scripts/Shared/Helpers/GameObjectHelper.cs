using UnityEngine;

public class GameObjectHelper : MonoBehaviour
{
    public void DestroyOnTime(float time) => Destroy(gameObject, time);
}