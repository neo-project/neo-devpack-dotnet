using System.Diagnostics;

namespace Neo.SmartContract.Testing.Coverage
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="offset">Offset</param>
    /// <param name="outOfScript">Out of script</param>
    [DebuggerDisplay("Offset={Offset}, Count={Count}, Hits={Hits}")]
    public class CoverageBranch(int offset, bool outOfScript = false)
    {
        /// <summary>
        /// Offset
        /// </summary>
        public int Offset { get; } = offset;

        /// <summary>
        /// Count
        /// </summary>
        public int Count { get; } = 2;

        /// <summary>
        /// The branch is out of the script
        /// </summary>
        public bool OutOfScript { get; } = outOfScript;

        /// <summary>
        /// Hits
        /// </summary>
        public int Hits => (HitsPath1 > 0 ? 1 : 0) + (HitsPath2 > 0 ? 1 : 0);

        /// <summary>
        /// Path 2 hits
        /// </summary>
        public int HitsPath1 { get; set; }

        /// <summary>
        /// Path 2 hits
        /// </summary>
        public int HitsPath2 { get; set; }

        /// <summary>
        /// Hit branch
        /// </summary>
        /// <param name="value">Value</param>
        public void Hit(bool value)
        {
            if (value) HitsPath1++;
            else HitsPath2++;
        }

        /// <summary>
        /// Hit branch
        /// </summary>
        /// <param name="value">Value</param>
        public void Hit(CoverageBranch value)
        {
            HitsPath1 += value.HitsPath1;
            HitsPath2 += value.HitsPath2;
        }

        /// <summary>
        /// Clone branch
        /// </summary>
        /// <returns>CoverageBranch</returns>
        public CoverageBranch Clone()
        {
            return new CoverageBranch(Offset, OutOfScript)
            {
                HitsPath1 = HitsPath1,
                HitsPath2 = HitsPath2
            };
        }
    }
}
