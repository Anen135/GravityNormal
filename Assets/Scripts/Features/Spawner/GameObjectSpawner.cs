using UnityEngine;

public class GameObjectSpawner : MonoBehaviour
{
    public GameObject spawningPrefab;
    public Collider spawnArea;
    protected GameObject lastSpawned;
    void OnValidate()
    {
        spawnArea = spawnArea != null ? spawnArea : GetComponent<Collider>();
        spawningPrefab = spawningPrefab != null ? spawningPrefab : Resources.Load<GameObject>("Prefabs/RedShip");
    }

    void Awake()
    {
        spawnArea = spawnArea != null ? spawnArea : GetComponent<Collider>();
        spawningPrefab = spawningPrefab != null ? spawningPrefab : Resources.Load<GameObject>("Prefabs/RedShip");
        if (spawnArea == null) Debug.LogError("Spawn area is not assigned.", this);
        if (spawningPrefab == null) Debug.LogError("Spawn area is not assigned.", this);
    }
    public virtual void Spawn()
    {
        lastSpawned = Instantiate(spawningPrefab, new Vector3(
            Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y),
            Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z)
        ), Quaternion.Euler(
            Random.Range(0f, 360f),
            Random.Range(0f, 360f),
            Random.Range(0f, 360f)
        ));
    }

    public virtual void SpawnMany(int i) { for (int j = 0; j < i; j++) { Spawn(); } }

}