// Copyright (C) 2015-2026 The Neo Project.
//
// ByteArrayAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Framework.Attributes
{
    /// <summary>
    /// Specifies a byte array value for a static field within a smart contract,
    /// enabling the field to be initialized at compile time.
    /// </summary>
    /// <remarks>
    /// This attribute is used to initialize fields of type byte[] with a hexadecimal string.
    ///
    /// <para>Example:</para>
    /// <code>
    /// [ByteArray("0123456789ABCDEF")]
    /// private static readonly byte[] data = default;
    /// </code>
    ///
    /// The value is converted to a byte array at compile time,
    /// avoiding runtime conversion overhead.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class ByteArrayAttribute(string value) : InitialValueAttribute(value, ContractParameterType.ByteArray)
    {
    }
}
