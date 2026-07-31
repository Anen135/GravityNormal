using UnityEngine;
public class Decision<T>
{
    public T Context;
    public bool invert;
    public string debugstring = "Decide";
    public Decision(T context) { Context = context; }
    public bool Decide()
    {
        // Debug.Log($"{debugstring} : {invert} - {Condition()}");
        return invert ? !Condition() : Condition();
    }

    public virtual bool Condition() => true;
}