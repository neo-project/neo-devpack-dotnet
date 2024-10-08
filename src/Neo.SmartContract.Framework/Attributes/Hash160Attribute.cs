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
    /// Specifies a Hash160 value for a static field within a smart contract,
    /// enabling the field to be initialized at compile time.
    /// </summary>
    /// <remarks>
    /// This attribute is used to initialize fields of type UInt160 or byte[]
    /// with a Hash160 value, which can be either a NEO address or a 20-byte hexadecimal string.
    ///
    /// <para>Examples:</para>
    /// <code>
    /// // Using a NEO address
    /// [Hash160("NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq")]
    /// private static readonly UInt160 ownerAddress = default;
    ///
    /// // Using a 20-byte script hash
    /// [Hash160("0x0123456789abcdef0123456789abcdef01234567")]
    /// private static readonly byte[] contractHash = default;
    /// </code>
    ///
    /// The value is converted to the appropriate type at compile time,
    /// avoiding runtime conversion overhead.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class Hash160Attribute(string value) : InitialValueAttribute(value, ContractParameterType.Hash160)
    {
    }
}
