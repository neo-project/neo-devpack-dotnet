// Copyright (C) 2015-2023 The Neo Project.
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
    /// Specifies an initial value for a static field within a smart contract,
    /// enabling the field to be initialized at compile time.
    /// </summary>
    ///
    /// <remarks>
    /// In smart contracts, it's necessary to initialize static variables.
    /// However, initializing some contract-specific types directly may introduce
    /// complex type conversions, leading to additional overhead during contract
    /// execution as the conversion operation is called each time.
    /// By using the <see cref="InitialValueAttribute"/>, variables can be assigned
    /// an initial value, allowing for compile-time initialization and avoiding runtime
    /// conversion overhead.
    ///
    /// <para>Examples:</para>
    /// <code>
    /// // Example of initializing a UInt160 field with a Hash160 address
    /// [InitialValue("NXV7ZhHiyM1aHXwpVsRZC6BwNFP2jghXAq", ContractParameterType.Hash160)]
    /// private static readonly UInt160 validUInt160 = default;
    ///
    /// // Example of initializing a byte array field with a hex string representing a UInt256 value
    /// [InitialValue("edcf8679104ec2911a4fe29ad7db232a493e5b990fb1da7af0c7b989948c8925", ContractParameterType.ByteArray)]
    /// private static readonly byte[] validUInt256 = default;
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class InitialValueAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitialValueAttribute"/> class
        /// with the specified initial value and contract parameter type.
        /// </summary>
        /// <param name="value">The initial value to assign to the field, represented
        /// as a string.</param>
        /// <param name="type">The <see cref="ContractParameterType"/> indicating the
        /// type of the field being initialized.</param>
        public InitialValueAttribute(string value, ContractParameterType type)
        {
        }
    }
}
