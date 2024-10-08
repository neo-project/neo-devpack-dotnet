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
    /// Specifies a public key value for a static field within a smart contract,
    /// enabling the field to be initialized at compile time.
    /// </summary>
    /// <remarks>
    /// This attribute is used to initialize fields of type ECPoint or byte[]
    /// with a public key in hexadecimal format.
    ///
    /// <para>Example:</para>
    /// <code>
    /// [PublicKey("03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c")]
    /// private static readonly ECPoint publicKey = default;
    /// </code>
    ///
    /// The value is converted to the appropriate type at compile time,
    /// avoiding runtime conversion overhead.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class PublicKeyAttribute(string value) : InitialValueAttribute(value, ContractParameterType.PublicKey)
    {
    }
}
