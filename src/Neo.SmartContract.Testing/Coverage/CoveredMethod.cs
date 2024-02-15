using Neo.SmartContract.Manifest;
using System.Collections.Generic;
using System.Diagnostics;

namespace Neo.SmartContract.Testing.Coverage
{
    [DebuggerDisplay("{Method}")]
    public class CoveredMethod : CoverageBase
    {
        /// <summary>
        /// Contract
        /// </summary>
        public CoveredContract Contract { get; }

        /// <summary>
        /// Method
        /// </summary>
        public AbiMethod Method { get; }

        /// <summary>
        /// Offset
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Method length
        /// </summary>
        public int MethodLength { get; }

        /// <summary>
        /// Coverage
        /// </summary>
        public override IEnumerable<CoverageHit> Coverage => Contract.GetCoverageFrom(Offset, MethodLength);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract">Contract</param>
        /// <param name="method">Method</param>
        /// <param name="methodLength">Method length</param>
        public CoveredMethod(CoveredContract contract, ContractMethodDescriptor method, int methodLength)
        {
            Contract = contract;
            Method = new AbiMethod(method.Name, method.Parameters.Length);
            Offset = method.Offset;
            MethodLength = methodLength;
        }

        public override string ToString() => Method.ToString();
    }
}
