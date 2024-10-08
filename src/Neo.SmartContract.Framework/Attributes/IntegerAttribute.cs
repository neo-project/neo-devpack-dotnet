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
    /// Specifies an integer value for a static field within a smart contract,
    /// enabling the field to be initialized at compile time.
    /// </summary>
    /// <remarks>
    /// This attribute is used to initialize fields of numeric types (e.g., int, long, BigInteger)
    /// with a string representation of an integer value.
    ///
    /// <para>Example:</para>
    /// <code>
    /// [Integer("1000000")]
    /// private static readonly BigInteger largeNumber = default;
    /// </code>
    ///
    /// The value is converted to the appropriate numeric type at compile time,
    /// avoiding runtime conversion overhead.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class IntegerAttribute(string value) : InitialValueAttribute(value, ContractParameterType.Integer)
    {
    }
}
