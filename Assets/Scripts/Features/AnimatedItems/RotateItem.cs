using Lucy.Core.Helpers;
using UnityEngine;

public class RotateItem : RotateHelper
{
    public Vector3 axis;
    public float angle;
    public float duration;
    public void Rotate() => RotateTo(axis, angle, duration);
}