using System.Collections.Generic;
using UnityEngine;

public class ToggleMonoBehaviour : MonoBehaviour
{
    public List<MonoBehaviour> targets;
    int index = 0;
    public void Toggle() => targets.ForEach(static mb => mb.enabled = !mb.enabled);
    public void All(bool value) => targets.ForEach(mb => mb.enabled = value);

    public void Next()
    {
        SetEnabled(index, false);
        index = (index + 1) % targets.Count;
        SetEnabled(index, true);
    }

    public void Previous()
    {
        SetEnabled(index, false);
        index = (index + targets.Count - 1) % targets.Count;
        SetEnabled(index, true);
    }

    public void Set(int i)
    {
        SetEnabled(index, false);
        index = i % targets.Count;
        SetEnabled(index, true);
    }

    public void Set(MonoBehaviour mb)
    {
        SetEnabled(index, false);
        index = targets.IndexOf(mb);
        SetEnabled(index, true);
    }

    private void SetEnabled(int i, bool value)
    {
        if (i < 0 || i >= targets.Count) return;

        // Explicit null check rather than null propagation (?.), so Unity's
        // overloaded ==/!= (which also treats destroyed objects as null) is used.
        MonoBehaviour target = targets[i];
        if (target != null)
        {
            target.enabled = value;
        }
    }

}