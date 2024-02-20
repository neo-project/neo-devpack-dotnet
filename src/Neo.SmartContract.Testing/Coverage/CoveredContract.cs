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
        /// <param name="mechanism">Method detection mechanism</param>
        /// <param name="hash">Hash</param>
        /// <param name="state">Contract state</param>
        public CoveredContract(MethodDetectionMechanism mechanism, UInt160 hash, ContractState? state)
        {
            Hash = hash;
            Methods = Array.Empty<CoveredMethod>();

            if (state is not null)
            {
                // Extract all methods
                GenerateMethods(mechanism, state);
            }
        }

        internal void GenerateMethods(MethodDetectionMechanism mechanism, ContractState state)
        {
            Script script = state.Script;
            HashSet<int> privateAdded = new();
            List<ContractMethodDescriptor> methods = new(state.Manifest.Abi.Methods);

            // Iterate all valid instructions

            int ip = 0;

            while (ip < script.Length)
            {
                var instruction = script.GetInstruction(ip);

                if (!_coverageData.ContainsKey(ip))
                {
                    _coverageData[ip] = new CoverageHit(ip, CoverageHit.DescriptionFromInstruction(instruction, state.Nef.Tokens), false);
                }

                if (mechanism == MethodDetectionMechanism.NextMethod)
                {
                    // Find private methods

                    switch (instruction.OpCode)
                    {
                        case OpCode.CALL_L:
                            {
                                var offset = ip + instruction.TokenI32;
                                if (privateAdded.Add(offset))
                                {
                                    methods.Add(new ContractMethodDescriptor()
                                    {
                                        Name = "_private" + offset,
                                        Offset = offset,
                                        ReturnType = ContractParameterType.Void,
                                        Safe = false,
                                        Parameters = Array.Empty<ContractParameterDefinition>(),
                                    });
                                }
                                break;
                            }
                        case OpCode.CALLT:
                            {
                                var offset = ip + instruction.TokenI8;
                                if (privateAdded.Add(offset))
                                {
                                    methods.Add(new ContractMethodDescriptor()
                                    {
                                        Name = "_private" + offset,
                                        Offset = offset,
                                        ReturnType = ContractParameterType.Void,
                                        Safe = false,
                                        Parameters = Array.Empty<ContractParameterDefinition>(),
                                    });
                                }
                                break;
                            }
                    }
                }

                ip += instruction.Size;
            }

            Methods = methods
                .Select(s => CreateMethod(mechanism, script, methods, s))
                .OrderBy(o => o.Offset)
                .ToArray()!;
        }

        private CoveredMethod CreateMethod(
            MethodDetectionMechanism mechanism, Script script,
            List<ContractMethodDescriptor> allMethods, ContractMethodDescriptor abiMethod
            )
        {
            int ip = abiMethod.Offset;
            var to = script.Length - 1;

            switch (mechanism)
            {
                case MethodDetectionMechanism.FindRET:
                    {
                        while (ip < script.Length)
                        {
                            var instruction = script.GetInstruction(ip);
                            if (instruction.OpCode == OpCode.RET) break;
                            ip += instruction.Size;
                            to = ip;
                        }
                        break;
                    }
                case MethodDetectionMechanism.NextMethod:
                case MethodDetectionMechanism.NextMethodInAbi:
                    {
                        var next = allMethods.OrderBy(u => u.Offset).Where(u => u.Offset > abiMethod.Offset).FirstOrDefault();
                        if (next is not null) to = next.Offset - 1;
                        break;
                    }
            }

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
            return Methods.FirstOrDefault(m => m.Method.Name == methodName && m.Method.PCount == pcount);
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
        public string Dump(DumpFormat format = DumpFormat.Console)
        {
            return Dump(format, Methods);
        }

        /// <summary>
        /// Dump coverage
        /// </summary>
        /// <returns>Coverage dump</returns>
        internal string Dump(DumpFormat format, params CoveredMethod[] methods)
        {
            var builder = new StringBuilder();
            using var sourceCode = new StringWriter(builder)
            {
                NewLine = "\n"
            };

            switch (format)
            {
                case DumpFormat.Console:
                    {
                        var cover = $"{CoveredPercentage:P2}";
                        sourceCode.WriteLine($"{Hash} [{cover}]");

                        List<string[]> rows = new();
                        var max = new int[] { "Method".Length, "Line  ".Length };

                        foreach (var method in methods.OrderBy(u => u.Method.Name).OrderByDescending(u => u.CoveredPercentage))
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
                        break;
                    }
                case DumpFormat.Html:
                    {
                        sourceCode.WriteLine(@"
<!DOCTYPE html>
<html lang=""en"">
<head>
<meta charset=""UTF-8"">
<title>NEF coverage Report</title>
<style>
    body { font-family: Arial, sans-serif; margin: 0; padding: 0; }
    .bar { background-color: #f2f2f2; padding: 10px; cursor: pointer; }
    .hash { float: left; }
    .method-name { float: left; }
    .coverage { float: right; }
    .method { cursor: pointer; margin-top: 5px; padding: 2px; }
    .details { display: none; padding-left: 20px; }
    .container { padding-left: 20px; }
    .opcode { margin-left: 20px; position: relative; padding: 2px; margin-bottom: 2px; display: flex; align-items: center; }
    .hit { background-color: #eafaea; } /* Light green for hits */
    .no-hit { background-color: #ffcccc; } /* Light red for no hits */
    .hits { margin-left: 5px; font-size: 0.6em; margin-right: 10px; }
    .icon { margin-right: 5px; }

    .high-coverage { background-color: #ccffcc; } /* Lighter green for high coverage */
    .medium-coverage { background-color: #ffffcc; } /* Yellow for medium coverage */
    .low-coverage { background-color: #ffcccc; } /* Lighter red for low coverage */
</style>
</head>
<body>
");

                        sourceCode.WriteLine($@"
<div class=""bar"">
    <div class=""hash"">{Hash}</div>
    <div class=""coverage"">{CoveredPercentage:P2}</div>
    <div style=""clear: both;""></div>
</div>
<div class=""container"">
");

                        foreach (var method in methods.OrderBy(u => u.Method.Name).OrderByDescending(u => u.CoveredPercentage))
                        {
                            var kind = "low";
                            if (method.CoveredPercentage > 0.7) kind = "medium";
                            if (method.CoveredPercentage > 0.8) kind = "high";

                            sourceCode.WriteLine($@"
<div class=""method {kind}-coverage"">
    <div class=""method-name"">{method.Method}</div>
    <div class=""coverage"">{method.CoveredPercentage:P2}</div>
    <div style=""clear: both;""></div>
</div>
");
                            sourceCode.WriteLine($@"<div class=""details"">");

                            foreach (var hit in method.Coverage)
                            {
                                var noHit = hit.Hits == 0 ? "no-" : "";
                                var icon = hit.Hits == 0 ? "✘" : "✔";

                                sourceCode.WriteLine($@"<div class=""opcode {noHit}hit""><span class=""icon"">{icon}</span><span class=""hits"">{hit.Hits} Hits</span>{hit.Description}</div>");
                            }

                            sourceCode.WriteLine($@"</div>
");
                        }

                        sourceCode.WriteLine(@"
</div>
<script>
    document.querySelector('.bar').addEventListener('click', () => {
        const container = document.querySelector('.container');
        container.style.display = container.style.display === 'none' ? 'block' : 'none';
    });

    document.querySelectorAll('.method').forEach(item => {
        item.addEventListener('click', function() {
            const details = this.nextElementSibling;
            if(details.style.display === '' || details.style.display === 'none') {
                details.style.display = 'block';
            } else {
                details.style.display = 'none';
            }
        });
    });
</script>

</body>
</html>
");
                        break;
                    }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Hit
        /// </summary>
        /// <param name="instructionPointer">Instruction pointer</param>
        /// <param name="instruction">Instruction</param>
        /// <param name="gas">Gas</param>
        public void Hit(int instructionPointer, Instruction instruction, long gas)
        {
            lock (_coverageData)
            {
                if (!_coverageData.TryGetValue(instructionPointer, out var coverage))
                {
                    // Note: This call is unusual, out of the expected

                    _coverageData[instructionPointer] = coverage = new(instructionPointer, CoverageHit.DescriptionFromInstruction(instruction), true);
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
