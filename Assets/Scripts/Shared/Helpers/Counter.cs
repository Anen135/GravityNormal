using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Counter : MonoBehaviour
{
    public int Count;
    public int TargetCount;
    public UnityEvent OnFinished;
    public UnityEvent<int> OnCountChanged;
    public void Increment()
    {
        Count += 1;
        OnCountChanged.Invoke(Count);
        if (Count == TargetCount) OnFinished.Invoke();
    }

    public void Decrement()
    {
        Count -= 1;
        OnCountChanged.Invoke(Count);
        if (Count == TargetCount) OnFinished.Invoke();
    }
}