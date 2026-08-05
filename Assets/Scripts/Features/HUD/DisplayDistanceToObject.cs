using System.Collections.Generic;
using UnityEngine;

public class DisplayDistanceToObject : MonoBehaviour
{
    public Transform agent;
    public GameObject[] targetObjects;
    public TMPro.TMP_Text distanceText;

    void FixedUpdate()
    {
        if (targetObjects.Length == 0)
        {
            distanceText.text = "No target objects assigned.";
            return;
        }

        float closestDistance = float.MaxValue;
        string closestObjectName = "";
        foreach (var target in targetObjects)
        {
            if (target == null || agent == null) continue;
            float distance = Vector3.Distance(agent.position, target.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObjectName = target.name;
            }
        }

        distanceText.text = $"Distance to {closestObjectName}: {closestDistance:F2} m";
    }
}