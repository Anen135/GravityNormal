using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyHelper : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;

    [Header("Default Directions")]
    public Vector3 velocity = Vector3.forward;
    public Vector3 angularVelocity = Vector3.up;

    [Header("Movement")]
    public float speed = 10f;
    public float acceleration = 15f;

    [Header("Rotation")]
    public float angularSpeed = 180f;
    public float angularAcceleration = 360f;

    [Header("Physics")]
    public ForceMode forceMode = ForceMode.Acceleration;

    Vector3 targetVelocity;
    Vector3 targetAngularVelocity;

    void Awake()
    {
        if (!rb) throw new System.Exception("RigidbodyHelper requires a Rigidbody component.");
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!rb) Debug.LogWarning("RigidbodyHelper requires a Rigidbody component.", this);
    }
#endif

    void FixedUpdate()
    {
        UpdateVelocity();
        UpdateRotation();
    }

    #region Movement

    public void Move()
    {
        Move(velocity);
    }

    public void Move(float multiplier)
    {
        Move(velocity * multiplier);
    }

    public void Move(Vector2 direction)
    {
        Move(new Vector3(direction.x, 0f, direction.y));
    }

    public void Move(Vector3 direction)
    {
        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        targetVelocity = direction * speed;
    }

    public void StopMove()
    {
        targetVelocity = Vector3.zero;
    }

    #endregion

    #region Rotation

    public void Rotate()
    {
        Rotate(angularVelocity);
    }

    public void Rotate(float multiplier)
    {
        Rotate(angularVelocity * multiplier);
    }

    public void Rotate(Vector2 direction)
    {
        Rotate(new Vector3(0f, direction.x, direction.y));
    }

    public void Rotate(Vector3 direction)
    {
        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        targetAngularVelocity = direction * angularSpeed;
    }

    public void StopRotate()
    {
        targetAngularVelocity = Vector3.zero;
    }

    #endregion

    #region Forces

    public void AddForce()
    {
        rb.AddRelativeForce(velocity * speed, forceMode);
    }

    public void AddForce(Vector3 force)
    {
        rb.AddRelativeForce(force, forceMode);
    }

    public void AddForce(Vector2 force)
    {
        AddForce(new Vector3(force.x, 0f, force.y));
    }

    public void AddForce(float multiplier)
    {
        rb.AddRelativeForce(velocity * speed * multiplier, forceMode);
    }

    public void AddTorque()
    {
        rb.AddRelativeTorque(angularVelocity * angularSpeed, forceMode);
    }

    public void AddTorque(Vector3 torque)
    {
        rb.AddRelativeTorque(torque, forceMode);
    }

    public void AddTorque(Vector2 torque)
    {
        AddTorque(new Vector3(0f, torque.x, torque.y));
    }

    public void AddTorque(float multiplier)
    {
        rb.AddRelativeTorque(angularVelocity * angularSpeed * multiplier, forceMode);
    }

    #endregion

    #region Instant

    public void SetVelocity()
    {
        rb.linearVelocity = velocity * speed;
    }

    public void SetVelocity(Vector3 velocity)
    {
        rb.linearVelocity = velocity;
    }

    public void SetAngularVelocity()
    {
        rb.angularVelocity = angularVelocity * angularSpeed * Mathf.Deg2Rad;
    }

    public void SetAngularVelocity(Vector3 angularVelocity)
    {
        rb.angularVelocity = angularVelocity * Mathf.Deg2Rad;
    }

    #endregion

    void UpdateVelocity()
    {
        rb.linearVelocity = Vector3.MoveTowards(
            rb.linearVelocity,
            targetVelocity,
            acceleration * Time.fixedDeltaTime);
    }

    void UpdateRotation()
    {
        Vector3 current = rb.angularVelocity * Mathf.Rad2Deg;

        current = Vector3.MoveTowards(
            current,
            targetAngularVelocity,
            angularAcceleration * Time.fixedDeltaTime);

        rb.angularVelocity = current * Mathf.Deg2Rad;
    }
}