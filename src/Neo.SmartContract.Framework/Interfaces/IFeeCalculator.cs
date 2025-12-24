// Copyright (C) 2015-2025 The Neo Project.
//
// IFeeCalculator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Numerics;

namespace Neo.SmartContract.Framework.Interfaces
{
    /// <summary>
    /// Interface for contracts that calculate dynamic fees for method invocations.
    /// </summary>
    /// <remarks>
    /// Contracts implementing this interface can be referenced by the
    /// <see cref="Attributes.FeeAttribute.Calculator"/> property when using
    /// <see cref="Attributes.FeeMode.Dynamic"/> mode.
    ///
    /// <para>
    /// <b>Implementation Requirements:</b>
    /// <list type="bullet">
    /// <item>The <see cref="CalculateFee"/> method must be marked with [Safe] attribute</item>
    /// <item>The method must not modify contract state</item>
    /// <item>Execution is limited to 100,000 GAS to prevent DoS attacks</item>
    /// </list>
    /// </para>
    ///
    /// <para>
    /// <b>Example Implementation:</b>
    /// <code>
    /// public class MyFeeCalculator : SmartContract, IFeeCalculator
    /// {
    ///     [Safe]
    ///     public BigInteger CalculateFee(ByteString method, object[] args)
    ///     {
    ///         // Tiered pricing based on method name
    ///         if (method == "premium") return 200000000; // 2 GAS
    ///         if (method == "standard") return 50000000; // 0.5 GAS
    ///         return 10000000; // 0.1 GAS default
    ///     }
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    public interface IFeeCalculator
    {
        /// <summary>
        /// Calculates the fee for a method invocation.
        /// </summary>
        /// <param name="method">The name of the method being invoked.</param>
        /// <param name="args">The arguments passed to the method.</param>
        /// <returns>The fee amount in datoshi (1e-8 GAS).</returns>
        BigInteger CalculateFee(ByteString method, object[] args);
    }
}
