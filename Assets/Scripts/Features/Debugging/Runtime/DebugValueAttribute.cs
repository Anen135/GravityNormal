using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class DebugValueAttribute : Attribute
{
    public string name;

    public DebugValueAttribute(string name = null)
    {
        this.name = name;
    }
}