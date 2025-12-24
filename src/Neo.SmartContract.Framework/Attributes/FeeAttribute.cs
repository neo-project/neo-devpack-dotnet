// Copyright (C) 2015-2025 The Neo Project.
//
// FeeAttribute.cs file belongs to the neo project and is free
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
    /// Specifies a custom contract fee for a method, paid by the caller to the beneficiary.
    /// This fee is in addition to standard network GAS costs.
    /// </summary>
    /// <remarks>
    /// Fees are denominated in datoshi (1e-8 GAS). For example:
    /// - 100000000 datoshi = 1.0 GAS
    /// - 50000000 datoshi = 0.5 GAS
    ///
    /// <para>
    /// <b>Fixed Fee Example:</b>
    /// <code>
    /// [Fee(Amount = 100000000, Beneficiary = "NM7g6DAeN3Nx8iK53GqY7fpeyqX5bUq5fJ")]
    /// public static string GetPrice(string symbol) { ... }
    /// </code>
    /// </para>
    ///
    /// <para>
    /// <b>Dynamic Fee Example:</b>
    /// <code>
    /// [Fee(Mode = FeeMode.Dynamic, Calculator = "0xb2a4cff31913016155e38e474a2c06d08be296cf", Beneficiary = "NM7g6DAeN3Nx8iK53GqY7fpeyqX5bUq5fJ")]
    /// public static UInt160 GetReport(uint tier) { ... }
    /// </code>
    /// </para>
    ///
    /// For dynamic fees, the calculator contract must implement <see cref="Interfaces.IFeeCalculator"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class FeeAttribute : Attribute
    {
        /// <summary>
        /// The fee amount in datoshi (1e-8 GAS).
        /// Required when <see cref="Mode"/> is <see cref="FeeMode.Fixed"/>.
        /// </summary>
        public long Amount { get; set; }

        /// <summary>
        /// The Neo address or script hash of the fee recipient.
        /// This field is required and must be explicitly specified.
        /// </summary>
        public string Beneficiary { get; set; } = string.Empty;

        /// <summary>
        /// The fee calculation mode. Defaults to <see cref="FeeMode.Fixed"/>.
        /// </summary>
        public FeeMode Mode { get; set; } = FeeMode.Fixed;

        /// <summary>
        /// The script hash of the fee calculator contract.
        /// Required when <see cref="Mode"/> is <see cref="FeeMode.Dynamic"/>.
        /// The calculator must implement <see cref="Interfaces.IFeeCalculator"/>.
        /// </summary>
        public string Calculator { get; set; } = string.Empty;
    }
}
