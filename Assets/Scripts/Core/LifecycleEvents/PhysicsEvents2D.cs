using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Events/Physics/Physics Events 2D")]
public sealed class PhysicsEvents2D : MonoBehaviour
{
    public Action<Collision2D> CollisionEnter2DAction;
    public Action<Collision2D> CollisionStay2DAction;
    public Action<Collision2D> CollisionExit2DAction;

    public Action<Collider2D> TriggerEnter2DAction;
    public Action<Collider2D> TriggerStay2DAction;
    public Action<Collider2D> TriggerExit2DAction;

    void OnCollisionEnter2D(Collision2D collision) => CollisionEnter2DAction?.Invoke(collision);
    void OnCollisionStay2D(Collision2D collision) => CollisionStay2DAction?.Invoke(collision);
    void OnCollisionExit2D(Collision2D collision) => CollisionExit2DAction?.Invoke(collision);

    void OnTriggerEnter2D(Collider2D other) => TriggerEnter2DAction?.Invoke(other);
    void OnTriggerStay2D(Collider2D other) => TriggerStay2DAction?.Invoke(other);
    void OnTriggerExit2D(Collider2D other) => TriggerExit2DAction?.Invoke(other);
}
