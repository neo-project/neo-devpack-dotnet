using System.Collections.Generic;
using System.Linq;

namespace Neo.SmartContract.Testing.Coverage
{
    public abstract class CoverageBase
    {
        /// <summary>
        /// Coverage
        /// </summary>
        public abstract IEnumerable<CoverageData> Coverage { get; }

        /// <summary>
        /// Total instructions (could be different from Coverage.Count if, for example, a contract JUMPS to PUSHDATA content)
        /// </summary>
        public virtual int TotalInstructions => Coverage.Where(u => !u.OutOfScript).Count();

        /// <summary>
        /// Covered Instructions (OutOfScript are not taken into account)
        /// </summary>
        public virtual int CoveredInstructions => Coverage.Where(u => !u.OutOfScript && u.Hits > 0).Count();

        /// <summary>
        /// All instructions that have been touched
        /// </summary>
        public virtual int HitsInstructions => Coverage.Where(u => u.Hits > 0).Count();

        /// <summary>
        /// Covered Percentage
        /// </summary>
        public float CoveredPercentage => (float)TotalInstructions / CoveredInstructions * 100F;

        /// <summary>
        /// Get Coverage from the Contract coverage
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="length">Length</param>
        /// <returns>Coverage</returns>
        public IEnumerable<CoverageData> GetCoverageFrom(int offset, int length)
        {
            var to = offset + length;

            foreach (var kvp in Coverage)
            {
                if (kvp.Offset >= offset && kvp.Offset <= to)
                {
                    yield return kvp;
                }
            }
        }
    }
}
