using System.Collections;
using System.Collections.Generic;
using Lucy.Core.Extensions; // Transform.ClearChildren
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class SceneSnapshotRestorer : MonoBehaviour
{
    #region Inspector
    [Header("Root object that contains all restoreable scene objects")]
    public Transform locationRoot;
    [Header("Options")]
    public bool captureOnAwake = true;
    public bool includeInactiveChildren = true;
    [Header("Events")]
    public UnityEvent onSnapshotSaving;
    public UnityEvent onSnapshotRestoring;
    public UnityEvent onSnapshotSaved;
    public UnityEvent onSnapshotRestored;
    [Header("Debug")]
    public bool debugEnabled;
    [Header("Runtime")]
    public GameObject snapshotRoot;
    public bool hasSnapshot;
    public readonly List<Transform> rootsToSave = new(64);
    #endregion
    
    void Awake()
    {
        if (locationRoot == null) locationRoot = transform;
        if (captureOnAwake) SaveSnapshot();
    }
    public void SaveSnapshot()
    {
        if (debugEnabled) Debug.Log("[SceneSnapshotRestorer] Saving snapshot.");
        StartCoroutine(SaveSnapshotRoutine());
    }

    public void RestoreSnapshot()
    {
        if (debugEnabled) Debug.Log("[SceneSnapshotRestorer] Restoring snapshot.");
        StartCoroutine(RestoreSnapshotRoutine());
    }

    public void ClearSnapshot()
    {
        if (snapshotRoot == null) return;
        if (Application.isPlaying) Destroy(snapshotRoot);
        else DestroyImmediate(snapshotRoot);
        snapshotRoot = null;
        hasSnapshot = false;
        rootsToSave.Clear();
    }

    IEnumerator SaveSnapshotRoutine()
    {
        onSnapshotSaving?.Invoke();
        yield return null;
        EnsureSnapshotRoot();
        snapshotRoot.transform.ClearChildren();
        rootsToSave.Clear();
        for (int i = 0; i < locationRoot.childCount; i++)
        {
            Transform child = locationRoot.GetChild(i);
            if (child == null) continue;
            if (!includeInactiveChildren && !child.gameObject.activeSelf) continue;
            rootsToSave.Add(child);
        }
        for (int i = 0; i < rootsToSave.Count; i++)
        {
            Transform source = rootsToSave[i];
            GameObject clone = Instantiate(source.gameObject, snapshotRoot.transform, false);
            clone.name = source.name;
            clone.SetActive(source.gameObject.activeSelf);
            if (i > 0 && i % 10 == 0) yield return null; 
        }
        hasSnapshot = true;
        onSnapshotSaved?.Invoke();
    }

    IEnumerator RestoreSnapshotRoutine()
    {
        if (!hasSnapshot || snapshotRoot == null || locationRoot == null)
        {
            if (debugEnabled) Debug.LogWarning("[SceneSnapshotRestorer] No snapshot to restore or location root is null.");  
            yield break;
        }
        onSnapshotRestoring?.Invoke();
        yield return null;
        locationRoot.ClearChildren();
        for (int i = 0; i < snapshotRoot.transform.childCount; i++)
        {
            Transform snapshotChild = snapshotRoot.transform.GetChild(i);
            if (snapshotChild == null) continue;
            GameObject clone = Instantiate(snapshotChild.gameObject, locationRoot, false);
            clone.name = snapshotChild.name;
            clone.SetActive(snapshotChild.gameObject.activeSelf);
            if (i > 0 && i % 10 == 0) yield return null; 
        }
        onSnapshotRestored?.Invoke();
    }
    
    void EnsureSnapshotRoot()
    {
        if (snapshotRoot != null) return;
        snapshotRoot = new GameObject($"{name}_Snapshot");
        snapshotRoot.SetActive(false);
        if (locationRoot == null) locationRoot = transform;
    }
    public void BakeInitializationBaseline()
    {
        RestoreSnapshot();
        SaveSnapshot();
    }
    void OnDestroy() => ClearSnapshot();
}