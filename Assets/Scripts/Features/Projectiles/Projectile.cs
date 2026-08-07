using System;
using Lucy.Core.Extensions;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float lifeTime = 0f;
    public LayerMask layerMask;
    
    public Action<Collision> OnHit;
    public Rigidbody rb;
    void Awake() => rb = GetComponent<Rigidbody>();

    void Start() => Destroy(gameObject, lifeTime);

    void OnCollisionEnter(Collision collision)
    {
        if (!layerMask.Contains(collision.gameObject)) return;

        ProcessHit(collision);
        OnHit?.Invoke(collision);
    }

    public virtual void ProcessHit(Collision collision) {}
    public virtual void Launch(float power) => rb.AddForce(transform.forward * power, ForceMode.Impulse);
}