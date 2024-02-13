using Neo.VM;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Neo.SmartContract.Testing.Coverage
{
    [DebuggerDisplay("{ToString()}")]
    public class CoveredContract
    {
        /// <summary>
        /// Contract Hash
        /// </summary>
        public UInt160 Hash { get; }

        /// <summary>
        /// Contract Script
        /// </summary>
        public Script Script { get; }

        /// <summary>
        /// Coverage
        /// </summary>
        public IDictionary<int, CoverageData> Coverage { get; }

        /// <summary>
        /// Total instructions (it could increase if the script get out of bounds)
        /// </summary>
        public int TotalInstructions => Coverage.Count;

        /// <summary>
        /// Covered instructions (it could increase if the script get out of bounds)
        /// </summary>
        public int CoveredInstructions => Coverage.Values.Where(u => u.Hits > 0).Count();

        /// <summary>
        /// CoveredContract
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="script">Script</param>
        public CoveredContract(UInt160 hash, Script script)
        {
            Hash = hash;
            Script = script;

            // Iterate all the valid instructions

            int ip = 0;
            Dictionary<int, CoverageData> coverage = new();

            while (ip < script.Length)
            {
                var instruction = script.GetInstruction(ip);
                coverage[ip] = new CoverageData();
                ip += instruction.Size;
            }

            Coverage = coverage;
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Hash:{Hash}";
        }
    }
}
