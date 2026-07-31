using System;
using UnityEngine;
[CreateAssetMenu(menuName = "IMD/Items/Default", fileName = "Data item")]
public class ItemDef : ScriptableObject
{
    public string id;
    public string displayName;
    public bool isStackable;
    public int maxStack = 1;
    public bool canPickup = true;
    public bool canDrop = true;
    public bool canUse = false;
    public GameObject prefab;
    public GameObject model;
    public virtual void Use(object user) { }
}