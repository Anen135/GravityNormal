using Lucy.Features.SpaceShip.Runtime;
using UnityEngine;
using Lucy.Core.Extensions;

[RequireComponent(typeof(Collider))]
public class RedShipSpawner : GameObjectSpawner
{
    public GameObject target;
    public override void Spawn()
    {
        base.Spawn();
        if (lastSpawned.TryGetComponent(out SpaceShipAIController ai)) ai.SetTarget(target);
        else Debug.LogError("SpaceShipAIController not found.", this);
    }
}