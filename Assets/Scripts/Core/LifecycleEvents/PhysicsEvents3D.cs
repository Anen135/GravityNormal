using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Events/Physics/Physics Events 3D")]
public sealed class PhysicsEvents3D : MonoBehaviour
{
    public Action<Collision> CollisionEnterAction;
    public Action<Collision> CollisionStayAction;
    public Action<Collision> CollisionExitAction;

    public Action<Collider> TriggerEnterAction;
    public Action<Collider> TriggerStayAction;
    public Action<Collider> TriggerExitAction;

    void OnCollisionEnter(Collision collision) => CollisionEnterAction?.Invoke(collision);
    void OnCollisionStay(Collision collision) => CollisionStayAction?.Invoke(collision);
    void OnCollisionExit(Collision collision) => CollisionExitAction?.Invoke(collision);

    void OnTriggerEnter(Collider other) => TriggerEnterAction?.Invoke(other);
    void OnTriggerStay(Collider other) => TriggerStayAction?.Invoke(other);
    void OnTriggerExit(Collider other) => TriggerExitAction?.Invoke(other);
}
