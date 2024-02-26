using System;

namespace Neo.SmartContract.Testing.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class FieldOrderAttribute : Attribute
{
    /// <summary>
    /// Gets the deserialization order of the property.
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="order">Order</param>
    public FieldOrderAttribute(int order)
    {
        Order = order;
    }
}
