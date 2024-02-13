using Neo.VM;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Neo.SmartContract.Testing.Coverage
{
    [DebuggerDisplay("{ToString()}")]
    public class CoveredContract
    {
        private readonly TestEngine _engine;

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

            return new CoveredMethod()
            {
                Contract = this,
                MethodName = methodName,
                PCount = pcount,
                Offset = abiMethod.Offset,
                MethodLength = to - abiMethod.Offset
            };
        }

        /// <summary>
        /// Get Coverage from the Contract coverage
        /// </summary>
        /// <param name="offset">Offset</param>
        /// <param name="length">Length</param>
        /// <returns>Coverage</returns>
        public IDictionary<int, CoverageData> GetCoverageFrom(int offset, int length)
        {
            var to = offset + length;
            var entries = new Dictionary<int, CoverageData>();

            foreach (var kvp in Coverage)
            {
                if (kvp.Key >= offset && kvp.Key <= to)
                {
                    entries[kvp.Key] = kvp.Value;
                }
            }

            return entries;
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
