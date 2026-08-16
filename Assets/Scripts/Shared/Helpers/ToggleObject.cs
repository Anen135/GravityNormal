using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    public List<GameObject> targets;
    int index = 0;
    public void Toggle() => targets.ForEach(go => go?.SetActive(!go.activeSelf));
    public void All(bool value) => targets.ForEach(go => go.SetActive(value));

    public void Next()
    {
        targets[index]?.SetActive(false);
        index = (index + 1) % targets.Count;
        targets[index]?.SetActive(true);
    }

    public void Previous()
    {
        targets[index]?.SetActive(false);
        index = (index + targets.Count - 1) % targets.Count;
        targets[index]?.SetActive(true);
    }

    public void Set(int i)
    {
        targets[index]?.SetActive(false);
        index = i % targets.Count;
        targets[index]?.SetActive(true);
    }

    public void Set(GameObject go)
    {
        targets[index]?.SetActive(false);
        index = targets.IndexOf(go);
        targets[index]?.SetActive(true);
    }

}