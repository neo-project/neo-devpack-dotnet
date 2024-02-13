using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Testing.Coverage
{
    public class CoveredMethod
    {
        /// <summary>
        /// Contract
        /// </summary>
        public required CoveredContract Contract { get; init; }

        /// <summary>
        /// Method name
        /// </summary>
        public required string MethodName { get; init; }

        /// <summary>
        /// PCount
        /// </summary>
        public required int PCount { get; init; }

        /// <summary>
        /// Offset
        /// </summary>
        public required int Offset { get; init; }

        /// <summary>
        /// Method length
        /// </summary>
        public required int MethodLength { get; init; }

        /// <summary>
        /// Coverage
        /// </summary>
        public IDictionary<int, CoverageData> Coverage => Contract.GetCoverageFrom(Offset, MethodLength);

        /// <summary>
        /// Total instructions (could be different from Coverage.Count if, for example, a contract JUMPS to PUSHDATA content)
        /// </summary>
        public int TotalInstructions => Coverage.Values.Where(u => !u.OutOfScript).Count();

        /// <summary>
        /// Covered Instructions (OutOfScript are not taken into account)
        /// </summary>
        public int CoveredInstructions => Coverage.Values.Where(u => !u.OutOfScript && u.Hits > 0).Count();

        /// <summary>
        /// All instructions that have been touched
        /// </summary>
        public int HitsInstructions => Coverage.Values.Where(u => u.Hits > 0).Count();
    }
}
