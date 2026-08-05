using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CustomTrigger : MonoBehaviour
{
    public UnityEvent triggerEvent;
    private void OnTriggerEnter(Collider other) => triggerEvent.Invoke();
}   