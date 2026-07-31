using UnityEngine;

public class ItemShooter : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Точка, из которой вылетают предметы.")]
    public Transform spawnPoint;

    [Tooltip("Список возможных префабов предметов.")]
    public GameObject[] itemPrefabs;

    [Tooltip("Общее количество доступных предметов для спавна.")]
    public int totalItems = 20;

    [Tooltip("Максимум предметов, которые могут выпасть за одно столкновение.")]
    public int maxItemsPerShot = 5;

    [Header("Shooting Force")]
    [Tooltip("Сила выстрела предметов.")]
    public float shootForce = 10f;

    private int itemsSpawned = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Cart"))
        {
            ShootItems();
        }
    }

    private void ShootItems()
    {
        if (itemPrefabs.Length == 0 || spawnPoint == null) return;

        int itemsToSpawn = Mathf.Min(maxItemsPerShot, totalItems - itemsSpawned);

        for (int i = 0; i < itemsToSpawn; i++)
        {
            GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            GameObject spawned = Instantiate(prefab, spawnPoint.position, Random.rotation);

            Rigidbody rb = spawned.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = spawned.AddComponent<Rigidbody>();
            }

            Vector3 shootDir = spawnPoint.forward + Random.insideUnitSphere * 0.3f;
            rb.AddForce(shootDir.normalized * shootForce, ForceMode.Impulse);

            itemsSpawned++;
            if (itemsSpawned >= totalItems)
                break;
        }
    }
}
