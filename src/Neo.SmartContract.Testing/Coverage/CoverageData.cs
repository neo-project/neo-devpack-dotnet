using System;
using System.Diagnostics;

namespace Neo.SmartContract.Testing.Coverage
{
    [DebuggerDisplay("{ToString()}")]
    public class CoverageData
    {
        /// <summary>
        /// The instruction is out of the script
        /// </summary>
        public bool OutOfScript { get; init; } = false;

        /// <summary>
        /// Hits
        /// </summary>
        public int Hits { get; private set; }

        /// <summary>
        /// Minimum used gas
        /// </summary>
        public long GasMin { get; private set; }

        /// <summary>
        /// Minimum used gas
        /// </summary>
        public long GasMax { get; private set; }

        /// <summary>
        /// Total used gas
        /// </summary>
        public long GasTotal { get; private set; }

        /// <summary>
        /// Average used gas
        /// </summary>
        public long GasAvg => Hits == 0 ? 0 : GasTotal / Hits;

        /// <summary>
        /// Hist
        /// </summary>
        /// <param name="gas">Gas</param>
        public void Hit(long gas)
        {
            Hits++;

            if (Hits == 1)
            {
                GasMin = gas;
                GasMax = gas;
            }
            else
            {
                GasMin = Math.Min(GasMin, gas);
                GasMax = Math.Max(GasMax, gas);
            }

            GasTotal += gas;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"OutOfScript:{OutOfScript}, Hits:{Hits}, GasTotal:{GasTotal}, GasMin:{GasMin}, GasMax:{GasMax}, GasAvg:{GasAvg}";
        }
    }
}
