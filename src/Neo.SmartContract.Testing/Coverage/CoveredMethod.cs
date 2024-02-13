using System.Collections.Generic;

namespace Neo.SmartContract.Testing.Coverage
{
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
        public override IEnumerable<CoverageData> Coverage => Contract.GetCoverageFrom(Offset, MethodLength);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract">Contract</param>
        /// <param name="method">Method</param>
        /// <param name="pCount">Parameters count</param>
        /// <param name="offset">Offset</param>
        /// <param name="methodLength">Method length</param>
        public CoveredMethod(CoveredContract contract, AbiMethod method, int offset, int methodLength)
        {
            Contract = contract;
            Method = method;
            Offset = offset;
            MethodLength = methodLength;
        }
    }
}
