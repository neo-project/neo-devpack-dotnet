using System.Collections.Generic;

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
        public abstract int TotalInstructions { get; }

        /// <summary>
        /// Covered Instructions (OutOfScript are not taken into account)
        /// </summary>
        public abstract int CoveredInstructions { get; }

        /// <summary>
        /// All instructions that have been touched
        /// </summary>
        public abstract int HitsInstructions { get; }

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
