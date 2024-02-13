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
        /// Total instructions (could be different from Coverage.Count if, for example, a contract JUMPS to PUSHDATA content)
        /// </summary>
        public int TotalInstructions { get; }

        /// <summary>
        /// Covered Instructions (OutOfScript are not taken into account)
        /// </summary>
        public int CoveredInstructions => Coverage.Values.Where(u => !u.OutOfScript && u.Hits > 0).Count();

        /// <summary>
        /// All instructions that have been touched
        /// </summary>
        public int HitsInstructions => Coverage.Values.Where(u => u.Hits > 0).Count();

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
                TotalInstructions++;
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
