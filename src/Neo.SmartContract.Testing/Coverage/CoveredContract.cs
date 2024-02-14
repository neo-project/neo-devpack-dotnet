using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Neo.SmartContract.Testing.Coverage
{
    [DebuggerDisplay("{Hash.ToString()}")]
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
        /// Methods
        /// </summary>
        public CoveredMethod[] Methods { get; }

        /// <summary>
        /// Coverage
        /// </summary>
        public override IEnumerable<CoverageData> Coverage => CoverageData.Values;

        /// <summary>
        /// CoveredContract
        /// </summary>
        /// <param name="hash">Hash</param>
        /// <param name="abi">Contract abi</param>
        /// <param name="script">Script</param>
        public CoveredContract(UInt160 hash, ContractAbi? abi, Script? script)
        {
            Hash = hash;
            Methods = Array.Empty<CoveredMethod>();

            if (script is null) return;

            // Extract all methods

            if (abi is not null)
            {
                Methods = abi.Methods
                   .Select(u => CreateMethod(abi, script, u))
                   .Where(u => u is not null)
                   .OrderBy(u => u!.Offset)
                   .ToArray()!;
            }

            // Iterate all valid instructions

            int ip = 0;

            while (ip < script.Length)
            {
                var instruction = script.GetInstruction(ip);
                CoverageData[ip] = new CoverageData(ip, false);
                ip += instruction.Size;
            }
        }

        private CoveredMethod? CreateMethod(ContractAbi abi, Script script, ContractMethodDescriptor abiMethod)
        {
            var to = script.Length - 1;
            var next = abi.Methods/*.OrderBy(u => u.Offset)*/.Where(u => u.Offset > abiMethod.Offset).FirstOrDefault();

            if (next is not null) to = next.Offset - 1;

            // Return method coverage

            return new CoveredMethod(this, abiMethod, to - abiMethod.Offset);
        }

        /// <summary>
        /// Get method coverage
        /// </summary>
        /// <param name="methodName">Method name</param>
        /// <param name="pcount">Parameter count</param>
        /// <returns>CoveredMethod</returns>
        public CoveredMethod? GetCoverage(string methodName, int pcount)
        {
            return GetCoverage(new AbiMethod(methodName, pcount));
        }

        /// <summary>
        /// Get method coverage
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>CoveredMethod</returns>
        public CoveredMethod? GetCoverage(AbiMethod? method = null)
        {
            if (method is null) return null;

            return Methods.FirstOrDefault(m => m.Method.Equals(method));
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
        /// Dump coverage
        /// </summary>
        /// <returns>Coverage dump</returns>
        public string Dump()
        {
            // TODO: improve dump later

            var builder = new StringBuilder();
            using var sourceCode = new StringWriter(builder)
            {
                NewLine = "\n"
            };

            var cover = CoveredPercentage.ToString("0.00").ToString();
            sourceCode.WriteLine($"| {Hash,-50} | {cover,7}% |");

            foreach (var method in Methods)
            {
                sourceCode.WriteLine(method.Dump());
            }

            return builder.ToString();
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
