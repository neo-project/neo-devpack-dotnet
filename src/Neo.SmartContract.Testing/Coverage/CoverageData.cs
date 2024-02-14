using System;
using System.Diagnostics;

namespace Neo.SmartContract.Testing.Coverage
{
    [DebuggerDisplay("{ToString()}")]
    public class CoverageData
    {
        /// <summary>
        /// The instruction offset
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// The instruction is out of the script
        /// </summary>
        public bool OutOfScript { get; }

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
        /// Constructor
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="outOfScript">Out of script</param>
        public CoverageData(int offset, bool outOfScript = false)
        {
            Offset = offset;
            OutOfScript = outOfScript;
        }

        /// <summary>
        /// Hits
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
        /// Hits
        /// </summary>
        /// <param name="value">Value</param>
        public void Hit(CoverageData value)
        {
            Hits += value.Hits;

            if (Hits == 1)
            {
                GasMin = value.GasMin;
                GasMax = value.GasMax;
            }
            else
            {
                GasMin = Math.Min(GasMin, value.GasMin);
                GasMax = Math.Max(GasMax, value.GasMax);
            }

            GasTotal += value.GasTotal;
        }

        /// <summary>
        /// Clone data
        /// </summary>
        /// <returns>CoverageData</returns>
        public CoverageData Clone()
        {
            return new CoverageData(Offset, OutOfScript)
            {
                GasMax = GasMax,
                GasMin = GasMin,
                GasTotal = GasTotal,
                Hits = Hits
            };
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Offset:{Offset}, OutOfScript:{OutOfScript}, Hits:{Hits}, GasTotal:{GasTotal}, GasMin:{GasMin}, GasMax:{GasMax}, GasAvg:{GasAvg}";
        }
    }
}
