using Lucy.Core.Helpers;
using UnityEngine;

public class SlideItem : SlideHelper
{
    public Vector3 direction;
    public float distance;
    public float duration;
    public void Move() => MoveTo(direction, distance, duration);
}