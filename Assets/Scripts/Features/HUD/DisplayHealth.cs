using System;
using Lucy.Core;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHealth : MonoBehaviour
{
    public Slider healthBar;
    public Entity entity;

    void OnEnable()
    {
        if (entity == null) Debug.LogError("Entity is not assigned.", this);
        healthBar.maxValue = entity.MaxHealth;
        healthBar.value = entity.Health;
        entity.OnDamageTaken += DamageTaken;
        SetHealthBarColor();
    }

    void OnDisable()
    {
        entity.OnDamageTaken -= DamageTaken;
    } 

    void DamageTaken(float arg1, object arg2)
    {
        healthBar.value = entity.Health;
        SetHealthBarColor();
    }

    void SetHealthBarColor()
    {
        if (entity.Health < 30) healthBar.fillRect.GetComponent<Image>().color = Color.red;
        else if (entity.Health < 60) healthBar.fillRect.GetComponent<Image>().color = Color.yellow;
        else if (entity.Health < 100) healthBar.fillRect.GetComponent<Image>().color = Color.green;
    }
}