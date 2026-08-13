using System;
using Lucy.Core;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHealth : MonoBehaviour
{
    public TMPro.TMP_Text healthText;
    public Image healthBar;
    public Entity entity;

    void OnEnable()
    {
        if (healthBar == null) { Debug.LogError("Health bar is not assigned.", this); return; }
        if (entity == null) Debug.LogError("Entity is not assigned.", this);
        entity.OnDamageTaken += UpdateHealth;
        UpdateHealth(0f, null);
        SetHealthBarColor();
    }

    void OnDisable()
    {
        entity.OnDamageTaken -= UpdateHealth;
    } 

    void UpdateHealth(float arg1, object arg2)
    {
        healthBar.fillAmount = entity.Health/entity.MaxHealth;
        healthText.text = $"{entity.Health/entity.MaxHealth*100f:0}%";
        SetHealthBarColor();
    }

    void SetHealthBarColor()
    {
        if (entity.Health < 30) healthBar.color = Color.red;
        else if (entity.Health < 60) healthBar.color = Color.yellow;
        else if (entity.Health < 100) healthBar.color = Color.green;
    }
}