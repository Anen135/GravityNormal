using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class VisionSensor : MonoBehaviour
{
    [SerializeField] private string detectableTag = "Contraband";
    [SerializeField] private float visionRadius = 50f;
    [SerializeField] private LayerMask visionObstacles = ~0;
    [SerializeField] private Transform eyesOrigin;
    [SerializeField] private bool debugRays = false;

    public event System.Action<GameObject> OnItemDetected;
    public event System.Action<GameObject> OnItemLost;

    private readonly List<GameObject> visibleObjects = new List<GameObject>();
    private readonly List<GameObject> trackedObjects = new List<GameObject>();

    private void Reset()
    {
        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = visionRadius;
    }

    private void Awake()
    {
        var col = GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = visionRadius;

        if (eyesOrigin == null)
            eyesOrigin = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(detectableTag) && !trackedObjects.Contains(other.gameObject))
            trackedObjects.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(detectableTag))
        {
            trackedObjects.Remove(other.gameObject);

            if (visibleObjects.Contains(other.gameObject))
            {
                visibleObjects.Remove(other.gameObject);
                OnItemLost?.Invoke(other.gameObject);
            }
        }
    }

    private void Update()
    {
        for (int i = trackedObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = trackedObjects[i];
            if (obj == null)
            {
                trackedObjects.RemoveAt(i);
                continue;
            }

            Vector3 origin = eyesOrigin.position;
            Vector3 target = obj.GetComponent<Collider>()?.bounds.center ?? obj.transform.position;
            Vector3 dir = (target - origin).normalized;
            float distance = Vector3.Distance(origin, target);

            bool canSee = false;

            if (Physics.Raycast(origin, dir, out RaycastHit hit, distance, visionObstacles))
            {
                canSee = hit.collider.gameObject == obj;
                if (debugRays) Debug.DrawRay(origin, dir * distance, canSee ? Color.green : Color.red, 0.1f);
            }

            if (canSee)
            {
                if (!visibleObjects.Contains(obj))
                {
                    visibleObjects.Add(obj);
                    OnItemDetected?.Invoke(obj);
                }
            }
            else if (visibleObjects.Contains(obj))
            {
                visibleObjects.Remove(obj);
                OnItemLost?.Invoke(obj);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.8f, 1f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}
