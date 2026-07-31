using UnityEngine;

public class DetectorInZone : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Половина длины стороны квадрата зоны обнаружения.")]
    public float detectionHalfSize = 5f;

    [Tooltip("Слой, в котором ищем объекты.")]
    public LayerMask layerMask;

    [Header("Требуемые объекты (по тегам)")]
    public int requiredStones = 2;
    public int requiredWood = 3;
    public int requiredCrystals = 1;

    [Header("Префаб для спавна, когда все условия выполнены")]
    public GameObject prefabToSpawn;

    // внутренние счётчики
    private int stoneCount;
    private int woodCount;
    private int crystalCount;

    void Update() => CheckObjectsInSquare();

    private void CheckObjectsInSquare()
    {
        // Находим все коллайдеры в квадрате (используем BoxOverlap)
        Collider[] hits = Physics.OverlapBox(transform.position, Vector3.one * detectionHalfSize, Quaternion.identity, layerMask);

        // Обнуляем счётчики
        stoneCount = 0;
        woodCount = 0;
        crystalCount = 0;

        // Подсчитываем теги
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Stone"))
                stoneCount++;
            else if (hit.CompareTag("Wood"))
                woodCount++;
            else if (hit.CompareTag("Crystal"))
                crystalCount++;
        }

        // Проверяем, достаточно ли собрали
        if (stoneCount >= requiredStones &&
            woodCount >= requiredWood &&
            crystalCount >= requiredCrystals)
        {
            ReplaceSelf();
        }
    }

    private void ReplaceSelf()
    {
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * detectionHalfSize * 2f);
    }
#endif
}
