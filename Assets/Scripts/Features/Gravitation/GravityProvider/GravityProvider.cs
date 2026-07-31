using UnityEngine;

public abstract class GravityProvider : MonoBehaviour
{
    public abstract Vector3 GetAcceleration(Rigidbody body);
}