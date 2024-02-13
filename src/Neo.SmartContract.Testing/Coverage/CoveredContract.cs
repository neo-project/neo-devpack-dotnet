using Neo.VM;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Neo.SmartContract.Testing.Coverage
{
    [DebuggerDisplay("{ToString()}")]
    public class CoveredContract : CoverageBase
    {
        private readonly TestEngine _engine;
        internal readonly Dictionary<int, CoverageData> CoverageData = new();

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
        public override IEnumerable<CoverageData> Coverage => CoverageData.Values;

        /// <summary>
        /// Total instructions (could be different from Coverage.Count if, for example, a contract JUMPS to PUSHDATA content)
        /// </summary>
        public override int TotalInstructions { get; }

        /// <summary>
        /// Covered Instructions (OutOfScript are not taken into account)
        /// </summary>
        public override int CoveredInstructions => Coverage.Where(u => !u.OutOfScript && u.Hits > 0).Count();

        /// <summary>
        /// All instructions that have been touched
        /// </summary>
        public override int HitsInstructions => Coverage.Where(u => u.Hits > 0).Count();

        /// <summary>
        /// CoveredContract
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="hash">Hash</param>
        /// <param name="script">Script</param>
        internal CoveredContract(TestEngine engine, UInt160 hash, Script script)
        {
            Hash = hash;
            Script = script;
            _engine = engine;

            // Iterate all valid instructions

            int ip = 0;

            while (ip < script.Length)
            {
                var instruction = script.GetInstruction(ip);
                CoverageData[ip] = new CoverageData(ip, false);
                ip += instruction.Size;
                TotalInstructions++;
            }
        }

        /// <summary>
        /// Get method coverage
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="pcount">Parameter count</param>
        /// <returns>CoveredMethod</returns>
        public CoveredMethod? GetCoverage(string methodName, int pcount)
        {
            // Find contract method by Abi
            // Note: this could be changed if the contract was updated

            var state = _engine.Native.ContractManagement.GetContract(Hash);
            if (state == null) return null;

            var abiMethod = state.Manifest.Abi.GetMethod(methodName, pcount);
            if (abiMethod == null) return null;

            var to = Script.Length - 1;
            var next = state.Manifest.Abi.Methods.OrderBy(u => u.Offset).Where(u => u.Offset > abiMethod.Offset).FirstOrDefault();

            if (next is not null) to = next.Offset - 1;

            // Return method coverage

            return new CoveredMethod(this, methodName, pcount, abiMethod.Offset, to - abiMethod.Offset);
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
