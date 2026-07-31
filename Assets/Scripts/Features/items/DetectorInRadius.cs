using System.Collections.Generic;
using UnityEngine;

public class DetectorInRadius : MonoBehaviour
{
    [Header("Detection Settings")]
    [Tooltip("Радиус поиска объектов.")]
    public float detectionRadius = 5f;

    [Tooltip("Слой, в котором ищем объекты.")]
    public LayerMask layerMask;

    public List<GameObject> DetectObjectsInRadius()
    {
        List<GameObject> foundObjects = new List<GameObject>();
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, layerMask);

        foreach (Collider hit in hits)
        {
          foundObjects.Add(hit.gameObject);
        }

        return foundObjects;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
#endif
}