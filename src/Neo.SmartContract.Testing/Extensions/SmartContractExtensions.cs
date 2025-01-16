// Copyright (C) 2015-2024 The Neo Project.
//
// SmartContractExtensions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Testing.Coverage;
using System;
using System.Linq.Expressions;

namespace Neo.SmartContract.Testing
{
    public static class SmartContractExtensions
    {
        /// <summary>
        /// Get Coverage by contract
        /// </summary>
        /// <param name="contract">Contract</param>
        /// <returns>CoveredContract</returns>
        public static CoveredContract? GetCoverage(this SmartContract contract)
        {
            return contract.Engine.GetCoverage(contract);
        }

        /// <summary>
        /// Get Coverage by method
        /// </summary>
        /// <typeparam name="T">Contract</typeparam>
        /// <param name="contract">Contract</param>
        /// <param name="method">Method</param>
        /// <returns>CoveredContract</returns>
        public static CoverageBase? GetCoverage<T>(this T contract, Expression<Action<T>> method) where T : SmartContract
        {
            return contract.Engine.GetCoverage(contract, method);
        }

        /// <summary>
        /// Get Coverage by method
        /// </summary>
        /// <typeparam name="T">Contract</typeparam>
        /// <typeparam name="TResult">Result</typeparam>
        /// <param name="contract">Contract</param>
        /// <param name="method">Method</param>
        /// <returns>CoveredContract</returns>
        public static CoverageBase? GetCoverage<T, TResult>(this T contract, Expression<Func<T, TResult>> method) where T : SmartContract
        {
            return contract.Engine.GetCoverage(contract, method);
        }
    }
}
