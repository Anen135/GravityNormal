using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Counter : MonoBehaviour
{
    public int Count;
    public int TargetCount;
    public UnityEvent OnFinished;
    public void Increment()
    {
        Count += 1;
        if (Count == TargetCount) OnFinished.Invoke();
    }

    public void Decrement()
    {
        Count -= 1;
        if (Count == TargetCount) OnFinished.Invoke();
    }
}