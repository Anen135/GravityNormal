using UnityEngine;

public class ShardSpawner : GameObjectSpawner
{
    public float scaleMin = 1f;
    public float scaleMax = 10f;
    public bool debugEnabled;
    public override void Spawn()
    {
        base.Spawn();
        if (debugEnabled)
        {
            Debug.Log($"Spawned {lastSpawned.name} from {gameObject.name}, Callstack: {System.Environment.StackTrace}");
        }
        Vector3 scaleMod = Vector3.one * Random.Range(scaleMin, scaleMax);
        lastSpawned.transform.localScale = scaleMod;
        // if (lastSpawned.TryGetComponent(out Rigidbody rb))
        // {
        //     float force = Random.Range(1f, 10f);
        //     float torque = Random.Range(1f, 10f);
        //     rb.AddForce(transform.forward * force, ForceMode.Impulse);
        //     rb.AddTorque(transform.up * torque, ForceMode.Impulse);
        // }
    }
}