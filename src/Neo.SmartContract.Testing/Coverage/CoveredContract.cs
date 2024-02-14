using Neo.VM;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Neo.SmartContract.Testing.Coverage
{
    [DebuggerDisplay("{ToString()}")]
    public class CoveredContract : CoverageBase
    {
        #region Internal

        /// <summary>
        /// Coverage Data
        /// </summary>
        internal Dictionary<int, CoverageData> CoverageData { get; } = new();

        #endregion

        /// <summary>
        /// Contract Hash
        /// </summary>
        public UInt160 Hash { get; }

        /// <summary>
        /// Coverage
        /// </summary>
        public override IEnumerable<CoverageData> Coverage => CoverageData.Values;

        /// <summary>
        /// CoveredContract
        /// </summary>
        /// <param name="hash">Hash</param>
        internal CoveredContract(UInt160 hash, Script? script = null)
        {
            Hash = hash;

            if (script is null) return;

            // Iterate all valid instructions

            int ip = 0;

            while (ip < script.Length)
            {
                var instruction = script.GetInstruction(ip);
                CoverageData[ip] = new CoverageData(ip, false);
                ip += instruction.Size;
            }
        }

        /// <summary>
        /// Get method coverage
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="methodName">Method name</param>
        /// <param name="pcount">Parameter count</param>
        /// <returns>CoveredMethod</returns>
        public CoveredMethod? GetCoverage(TestEngine engine, string methodName, int pcount)
        {
            return GetCoverage(engine, new AbiMethod(methodName, pcount));
        }

        /// <summary>
        /// Get method coverage
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="method">Method</param>
        /// <returns>CoveredMethod</returns>
        public CoveredMethod? GetCoverage(TestEngine engine, AbiMethod? method = null)
        {
            if (method is null) return null;

            // Find contract method by Abi
            // Note: this could be changed if the contract was updated

            var state = engine.Native.ContractManagement.GetContract(Hash);
            if (state == null) return null;

            var abiMethod = state.Manifest.Abi.GetMethod(method.Name, method.PCount);
            if (abiMethod == null) return null;

            var to = state.Script.Length - 1;
            var next = state.Manifest.Abi.Methods.OrderBy(u => u.Offset).Where(u => u.Offset > abiMethod.Offset).FirstOrDefault();

            if (next is not null) to = next.Offset - 1;

            // Return method coverage

            return new CoveredMethod(this, method, abiMethod.Offset, to - abiMethod.Offset);
        }

        /// <summary>
        /// Join coverage
        /// </summary>
        /// <param name="coverage">Coverage</param>
        public void Join(IEnumerable<CoverageData> coverage)
        {
            // Join the coverage between them

            foreach (var c in coverage)
            {
                if (c.Hits == 0) continue;

                if (CoverageData.TryGetValue(c.Offset, out var kvpValue))
                {
                    kvpValue.Hit(c);
                }
                else
                {
                    CoverageData.Add(c.Offset, c.Clone());
                }
            }
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
