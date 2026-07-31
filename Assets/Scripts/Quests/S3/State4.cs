using System.Collections;
using Lucy.Core.FSM.Graph;
using UnityEngine;

public class State4_Dead : State
{
    [Header("References")]
    public SceneSnapshotRestorer Restorer;
    public OneHandInventory Inventory;
    public Transform spawnPoint;
    public override void Enter()
    {
        Restorer.RestoreSnapshot();
        Inventory.ClearInventory();
        Inventory.gameObject.transform.position = spawnPoint.position;
        Inventory.gameObject.transform.rotation = spawnPoint.rotation;
    }

    void OnValidate()
    {
        if (Restorer == null && TryGetComponent(out SceneSnapshotRestorer r)) Restorer = r;
    }

    public override IEnumerator Execute() { return null; }

    public override void Exit() { }
}