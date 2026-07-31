using UnityEngine;

public class RigidbodyHelper : MonoBehaviour
{
    public Rigidbody rb;
    public Vector3 velocity;
    public Vector3 force;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) Debug.LogError("Rigidbody not found!");
    }

    void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }
    public void SetVelocity()
    {
        rb.linearVelocity = velocity;
    }

    public void SetForce()
    {
        rb.AddForce(force);
    }
}