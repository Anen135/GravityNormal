using UnityEngine;

public class DisplaySpeed : MonoBehaviour
{
    [Header("Dependencies")]
    public Rigidbody rb;
    public TMPro.TMP_Text speedText;

    void FixedUpdate()
    {
        if (rb == null || speedText == null) Debug.LogWarning("Rigidbody or Speed Text is not assigned.", this);
        else speedText.text = $"Speed: {rb.linearVelocity.magnitude:F2} m/s";
    }
}