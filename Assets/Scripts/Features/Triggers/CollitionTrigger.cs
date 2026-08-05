using UnityEngine;
using UnityEngine.Events;

public class CollitionTrigger : MonoBehaviour
{
    public UnityEvent<Collision> onCollisionEnter, onCollisionExit;
    private void OnCollisionEnter(Collision col) => onCollisionEnter.Invoke(col);
    private void OnCollisionExit(Collision col) => onCollisionExit.Invoke(col);
}