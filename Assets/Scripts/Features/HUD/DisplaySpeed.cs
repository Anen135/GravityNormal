using UnityEngine;
using UnityEngine.UI;

public class DisplaySpeed : MonoBehaviour
{
    [Header("Dependencies")]
    public Rigidbody rb;
    public TMPro.TMP_Text speedText;
    public Image speedBar;

    void FixedUpdate()
    {
        if (rb == null) {Debug.LogWarning("Rigidbody is not assigned.", this); return;}
        if (speedText != null) speedText.text = $"{rb.linearVelocity.magnitude:F2} m/s";
        if (speedBar != null) speedBar.fillAmount = rb.linearVelocity.magnitude / 100f;
    }
}