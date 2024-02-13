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
        public static CoveredMethod? GetCoverage<T>(this T contract, Expression<Action<T>> method) where T : SmartContract
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
        public static CoveredMethod? GetCoverage<T, TResult>(this T contract, Expression<Func<T, TResult>> method) where T : SmartContract
        {
            return contract.Engine.GetCoverage(contract, method);
        }
    }
}
