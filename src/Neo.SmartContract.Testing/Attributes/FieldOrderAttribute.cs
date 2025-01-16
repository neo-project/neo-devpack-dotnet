// Copyright (C) 2015-2024 The Neo Project.
//
// FieldOrderAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

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
