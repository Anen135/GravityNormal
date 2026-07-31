using UnityEngine;

public class TransformHelper : MonoBehaviour
{
    public Vector3 deltaPosition;
    public Vector3 deltaRotation;
    public void SetPosition() => transform.position = deltaPosition;
    public void AddPosition() => transform.position = transform.position + deltaPosition;
    public void SetRotation() => transform.rotation = transform.rotation * Quaternion.Euler(deltaRotation);
    public void AddRotation() => transform.rotation *= Quaternion.Euler(deltaRotation);
}