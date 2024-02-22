using System;
using System.Diagnostics;

namespace Neo.SmartContract.Testing.Coverage
{
    [DebuggerDisplay("Offset={Offset}, Count={Count}, Hits={Hits}")]
    public class CoverageBranch
    {
        /// <summary>
        /// Offset
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Count
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// The branch is out of the script
        /// </summary>
        public bool OutOfScript { get; }

        /// <summary>
        /// Hits
        /// </summary>
        public int Hits => (PositivePathHits > 0 ? 1 : 0) + (NegativePathHits > 0 ? 1 : 0);

        /// <summary>
        /// Positive path hits
        /// </summary>
        public int PositivePathHits { get; set; }

        /// <summary>
        /// Negative Path hits
        /// </summary>
        public int NegativePathHits { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="outOfScript">Out of script</param>
        public CoverageBranch(int offset, bool outOfScript = false)
        {
            Offset = offset;
            Count = 2;
            OutOfScript = outOfScript;
        }

        /// <summary>
        /// Hit branch
        /// </summary>
        /// <param name="value">Value</param>
        public void Hit(bool value)
        {
            if (value) PositivePathHits++;
            else NegativePathHits++;
        }

        /// <summary>
        /// Hit branch
        /// </summary>
        /// <param name="value">Value</param>
        public void Hit(CoverageBranch value)
        {
            PositivePathHits += value.PositivePathHits;
            NegativePathHits += value.NegativePathHits;
        }

        /// <summary>
        /// Clone branch
        /// </summary>
        /// <returns>CoverageBranch</returns>
        public CoverageBranch Clone()
        {
            return new CoverageBranch(Offset, OutOfScript)
            {
                NegativePathHits = NegativePathHits,
                PositivePathHits = PositivePathHits
            };
        }
    }
}
