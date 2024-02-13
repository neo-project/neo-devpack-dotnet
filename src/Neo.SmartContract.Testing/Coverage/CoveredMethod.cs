using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Testing.Coverage
{
    public class CoveredMethod : CoverageBase
    {
        /// <summary>
        /// Contract
        /// </summary>
        public CoveredContract Contract { get; }

        /// <summary>
        /// Method name
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// PCount
        /// </summary>
        public int PCount { get; }

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
        /// Total instructions (could be different from Coverage.Count if, for example, a contract JUMPS to PUSHDATA content)
        /// </summary>
        public override int TotalInstructions => Coverage.Where(u => !u.OutOfScript).Count();

        /// <summary>
        /// Covered Instructions (OutOfScript are not taken into account)
        /// </summary>
        public override int CoveredInstructions => Coverage.Where(u => !u.OutOfScript && u.Hits > 0).Count();

        /// <summary>
        /// All instructions that have been touched
        /// </summary>
        public override int HitsInstructions => Coverage.Where(u => u.Hits > 0).Count();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract">Contract</param>
        /// <param name="methodName">Method name</param>
        /// <param name="pCount">Parameters count</param>
        /// <param name="offset">Offset</param>
        /// <param name="methodLength">Method length</param>
        public CoveredMethod(CoveredContract contract, string methodName, int pCount, int offset, int methodLength)
        {
            Contract = contract;
            MethodName = methodName;
            PCount = pCount;
            Offset = offset;
            MethodLength = methodLength;
        }
    }
}
