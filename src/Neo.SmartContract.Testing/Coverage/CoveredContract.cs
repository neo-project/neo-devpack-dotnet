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

        private readonly Dictionary<int, CoverageHit> _coverageData = new();

        #endregion

        /// <summary>
        /// Contract Hash
        /// </summary>
        public UInt160 Hash { get; }

        /// <summary>
        /// Methods
        /// </summary>
        public CoveredMethod[] Methods { get; private set; }

        /// <summary>
        /// Coverage
        /// </summary>
        public override IEnumerable<CoverageHit> Coverage => _coverageData.Values;

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

            GenerateMethods(abi, script);

            // Iterate all valid instructions

            int ip = 0;

            while (ip < script.Length)
            {
                var instruction = script.GetInstruction(ip);
                _coverageData[ip] = new CoverageHit(ip, false);
                ip += instruction.Size;
            }
        }

        internal void GenerateMethods(ContractAbi? abi, Script? script)
        {
            Methods = Array.Empty<CoveredMethod>();

            if (script is null || abi is null) return;

            Methods = abi.Methods
                .Select(s => CreateMethod(abi, script, s))
                .OrderBy(o => o.Offset)
                .ToArray()!;
        }

        private CoveredMethod CreateMethod(ContractAbi abi, Script script, ContractMethodDescriptor abiMethod)
        {
            var to = script.Length - 1;
            var next = abi.Methods.OrderBy(u => u.Offset).Where(u => u.Offset > abiMethod.Offset).FirstOrDefault();

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
        public void Join(IEnumerable<CoverageHit>? coverage)
        {
            if (coverage is null || coverage.Any() == false) return;

            // Join the coverage between them

            foreach (var c in coverage)
            {
                if (c.Hits == 0) continue;

                lock (_coverageData)
                {
                    if (_coverageData.TryGetValue(c.Offset, out var kvpValue))
                    {
                        kvpValue.Hit(c);
                    }
                    else
                    {
                        _coverageData.Add(c.Offset, c.Clone());
                    }
                }
            }
        }

        /// <summary>
        /// Dump coverage
        /// </summary>
        /// <returns>Coverage dump</returns>
        public string Dump()
        {
            var builder = new StringBuilder();
            using var sourceCode = new StringWriter(builder)
            {
                NewLine = "\n"
            };

            var cover = $"{CoveredPercentage:P2}";
            sourceCode.WriteLine($"{Hash} [{cover}]");

            List<string[]> rows = new();
            var max = new int[] { "Method".Length, "Line  ".Length };

            foreach (var method in Methods.OrderBy(u => u.Method.Name).OrderByDescending(u => u.CoveredPercentage))
            {
                cover = $"{method.CoveredPercentage:P2}";
                rows.Add(new string[] { method.Method.ToString(), cover });

                max[0] = Math.Max(method.Method.ToString().Length, max[0]);
                max[1] = Math.Max(cover.Length, max[1]);
            }

            sourceCode.WriteLine($"┌-{"─".PadLeft(max[0], '─')}-┬-{"─".PadLeft(max[1], '─')}-┐");
            sourceCode.WriteLine($"│ {string.Format($"{{0,-{max[0]}}}", "Method", max[0])} │ {string.Format($"{{0,{max[1]}}}", "Line  ", max[1])} │");
            sourceCode.WriteLine($"├-{"─".PadLeft(max[0], '─')}-┼-{"─".PadLeft(max[1], '─')}-┤");

            foreach (var print in rows)
            {
                sourceCode.WriteLine($"│ {string.Format($"{{0,-{max[0]}}}", print[0], max[0])} │ {string.Format($"{{0,{max[1]}}}", print[1], max[1])} │");
            }

            sourceCode.WriteLine($"└-{"─".PadLeft(max[0], '─')}-┴-{"─".PadLeft(max[1], '─')}-┘");

            return builder.ToString();
        }

        /// <summary>
        /// Hit
        /// </summary>
        /// <param name="instructionPointer">Instruction pointer</param>
        /// <param name="gas">Gas</param>
        public void Hit(int instructionPointer, long gas)
        {
            lock (_coverageData)
            {
                if (!_coverageData.TryGetValue(instructionPointer, out var coverage))
                {
                    // Note: This call is unusual, out of the expected

                    _coverageData[instructionPointer] = coverage = new CoverageHit(instructionPointer, true);
                }
                coverage.Hit(gas);
            }
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns>Hash</returns>
        public override string ToString() => Hash.ToString();
    }
}
