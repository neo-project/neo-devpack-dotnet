using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Coverage.Formats;
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Neo.SmartContract.Testing.Coverage
{
    [DebuggerDisplay("{Name}")]
    public class CoveredContract : CoverageBase
    {
        #region Internal

        private readonly SortedDictionary<int, CoverageHit> _lines = new();
        private readonly SortedDictionary<int, CoverageBranch> _branches = new();

        #endregion

        /// <summary>
        /// Contract name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Contract Hash
        /// </summary>
        public UInt160 Hash { get; }

        /// <summary>
        /// Methods
        /// </summary>
        public CoveredMethod[] Methods { get; private set; }

        /// <summary>
        /// Coverage Lines
        /// </summary>
        public override IEnumerable<CoverageHit> Lines => _lines.Values;

        /// <summary>
        /// Coverage Branches
        /// </summary>
        public override IEnumerable<CoverageBranch> Branches => _branches.Values;

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
                Name = state.Manifest.Name + $" [{Hash}]";

                // Extract all methods
                GenerateMethods(mechanism, state);
            }
            else
            {
                Name = Hash.ToString();
            }
        }

        internal void GenerateMethods(MethodDetectionMechanism mechanism, ContractState state)
        {
            Name = state.Manifest.Name + $" [{Hash}]";

            Script script = state.Script;
            HashSet<int> privateAdded = new();
            List<ContractMethodDescriptor> methods = new(state.Manifest.Abi.Methods);

            // Iterate all valid instructions

            int ip = 0;

            while (ip < script.Length)
            {
                var instruction = script.GetInstruction(ip);

                if (!_lines.ContainsKey(ip))
                {
                    AddLine(instruction, new CoverageHit(ip, CoverageHit.DescriptionFromInstruction(instruction, state.Nef.Tokens), false));
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

        private void AddLine(Instruction instruction, CoverageHit hit)
        {
            _lines[hit.Offset] = hit;

            // Check if we should add a branc

            switch (instruction.OpCode)
            {
                case OpCode.JMPIF:
                case OpCode.JMPIF_L:
                case OpCode.JMPIFNOT:
                case OpCode.JMPIFNOT_L:
                case OpCode.JMPEQ:
                case OpCode.JMPEQ_L:
                case OpCode.JMPNE:
                case OpCode.JMPNE_L:
                case OpCode.JMPGT:
                case OpCode.JMPGT_L:
                case OpCode.JMPGE:
                case OpCode.JMPGE_L:
                case OpCode.JMPLT:
                case OpCode.JMPLT_L:
                case OpCode.JMPLE:
                case OpCode.JMPLE_L:
                    {
                        _branches[hit.Offset] = new CoverageBranch(hit.Offset, hit.OutOfScript);
                        break;
                    }
            }
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

        internal bool TryGetLine(int offset, [NotNullWhen(true)] out CoverageHit? lineHit)
        {
            return _lines.TryGetValue(offset, out lineHit);
        }

        internal bool TryGetBranch(int offset, [NotNullWhen(true)] out CoverageBranch? branch)
        {
            return _branches.TryGetValue(offset, out branch);
        }

        /// <summary>
        /// Join coverage
        /// </summary>
        /// <param name="coverage">Coverage</param>
        public void Join(CoverageBase? coverage)
        {
            if (coverage is null) return;

            // Join the coverage lines

            foreach (var c in coverage.Lines)
            {
                if (c.Hits == 0) continue;

                lock (_lines)
                {
                    if (_lines.TryGetValue(c.Offset, out var kvpValue))
                    {
                        kvpValue.Hit(c);
                    }
                    else
                    {
                        _lines.Add(c.Offset, c.Clone());
                    }
                }
            }

            // Join the coverage branches

            foreach (var c in coverage.Branches)
            {
                if (c.Hits == 0) continue;

                lock (_branches)
                {
                    if (_branches.TryGetValue(c.Offset, out var kvpValue))
                    {
                        kvpValue.Hit(c);
                    }
                    else
                    {
                        _branches.Add(c.Offset, c.Clone());
                    }
                }
            }
        }

        /// <summary>
        /// Dump coverage
        /// </summary>
        /// <param name="format">Format</param>
        /// <returns>Coverage dump</returns>
        public override string Dump(DumpFormat format = DumpFormat.Console)
        {
            switch (format)
            {
                case DumpFormat.Console:
                    {
                        return new ConsoleFormat(this).Dump();
                    }
                case DumpFormat.Html:
                    {
                        return new IntructionHtmlFormat(this).Dump();
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        /// <summary>
        /// Hit
        /// </summary>
        /// <param name="instructionPointer">Instruction pointer</param>
        /// <param name="instruction">Instruction</param>
        /// <param name="gas">Gas</param>
        /// <param name="branchPath">Branch path</param>
        public void Hit(int instructionPointer, Instruction instruction, long gas, bool? branchPath)
        {
            lock (_lines)
            {
                if (!_lines.TryGetValue(instructionPointer, out var coverage))
                {
                    // Note: This call is unusual, out of the expected

                    coverage = new(instructionPointer, CoverageHit.DescriptionFromInstruction(instruction), true);
                    AddLine(instruction, coverage);
                }

                if (branchPath is not null && _branches.TryGetValue(instructionPointer, out var branch))
                {
                    branch.Hit(branchPath.Value);
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
