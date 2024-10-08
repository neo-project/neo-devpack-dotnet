// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Framework.Attributes
{
    /// <summary>
    /// Specifies a string value for a static field within a smart contract,
    /// enabling the field to be initialized at compile time.
    /// </summary>
    /// <remarks>
    /// This attribute is used to initialize fields of type string with a literal string value.
    ///
    /// <para>Example:</para>
    /// <code>
    /// [String("Hello, NEO!")]
    /// private static readonly string greeting = default;
    /// </code>
    ///
    /// The value is assigned to the field at compile time,
    /// avoiding runtime initialization overhead.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class StringAttribute(string value) : InitialValueAttribute(value, ContractParameterType.String)
    {
    }
}
