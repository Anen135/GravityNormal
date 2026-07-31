using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : MonoBehaviour
{
    public List<GameObject> targets;

    public void Toggle()
    {
        targets.ForEach(go => go.SetActive(!go.activeSelf));
    }
}