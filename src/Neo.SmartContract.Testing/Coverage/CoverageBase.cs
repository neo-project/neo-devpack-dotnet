using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Testing.Coverage
{
    public abstract class CoverageBase
    {
        /// <summary>
        /// Coverage Branches
        /// </summary>
        public abstract IEnumerable<CoverageBranch> Branches { get; }

        /// <summary>
        /// Coverage Lines
        /// </summary>
        public abstract IEnumerable<CoverageHit> Lines { get; }

        /// <summary>
        /// Total lines instructions (could be different from Coverage.Count if, for example, a contract JUMPS to PUSHDATA content)
        /// </summary>
        public int TotalLines => Lines.Where(u => !u.OutOfScript).Count();

        /// <summary>
        /// Total branches
        /// </summary>
        public int TotalBranches => Branches.Where(u => !u.OutOfScript).Sum(u => u.Count);

        /// <summary>
        /// Covered lines (OutOfScript are not taken into account)
        /// </summary>
        public int CoveredLines => Lines.Where(u => !u.OutOfScript && u.Hits > 0).Count();

        /// <summary>
        /// Covered lines (OutOfScript are not taken into account)
        /// </summary>
        public int CoveredBranches => Branches.Where(u => !u.OutOfScript).Sum(u => u.Hits);

        /// <summary>
        /// All lines that have been touched
        /// </summary>
        public int CoveredLinesAll => Lines.Where(u => u.Hits > 0).Count();

        /// <summary>
        /// All branches that have been touched
        /// </summary>
        public int CoveredBranchesAll => Branches.Where(u => u.Hits > 0).Count();

        /// <summary>
        /// Covered lines percentage
        /// </summary>
        public decimal CoveredLinesPercentage => CalculateHitRate(TotalLines, CoveredLines);

        /// <summary>
        /// Covered branch percentage
        /// </summary>
        public decimal CoveredBranchPercentage => CalculateHitRate(TotalBranches, CoveredBranches);

        /// <summary>
        /// Get Coverage lines from the Contract coverage
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="length">Length</param>
        /// <returns>Coverage</returns>
        public IEnumerable<CoverageHit> GetCoverageLinesFrom(int offset, int length)
        {
            var to = offset + length;

            foreach (var kvp in Lines)
            {
                if (kvp.Offset >= offset && kvp.Offset <= to)
                {
                    yield return kvp;
                }
            }
        }

        /// <summary>
        /// Get Coverage branch from the Contract coverage
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="length">Length</param>
        /// <returns>Coverage</returns>
        public IEnumerable<CoverageBranch> GetCoverageBranchFrom(int offset, int length)
        {
            var to = offset + length;

            foreach (var kvp in Branches)
            {
                if (kvp.Offset >= offset && kvp.Offset <= to)
                {
                    yield return kvp;
                }
            }
        }

        public static decimal CalculateHitRate(int total, int hits)
                    => total == 0 ? 1m : new decimal(hits) / new decimal(total);

        // Allow to sum coverages

        public static CoverageBase? operator +(CoverageBase? a, CoverageBase? b)
        {
            if (a is null) return b;
            if (b is null) return a;

            return new CoveredCollection(a, b);
        }
    }
}
