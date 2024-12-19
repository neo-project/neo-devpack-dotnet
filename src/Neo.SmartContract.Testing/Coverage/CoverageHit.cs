using Neo.VM;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Neo.Extensions;
using Neo.SmartContract.Testing.Interpreters;

namespace Neo.SmartContract.Testing.Coverage
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="offset">Offset</param>
    /// <param name="instruction">Instruction</param>
    /// <param name="outOfScript">Out of script</param>
    /// <param name="methodTokens">Method tokens</param>
    [DebuggerDisplay("Offset:{Offset}, Description:{Description}, OutOfScript:{OutOfScript}, Hits:{Hits}, GasTotal:{GasTotal}, GasMin:{GasMin}, GasMax:{GasMax}, GasAvg:{GasAvg}")]
    public class CoverageHit(int offset, Instruction instruction, bool outOfScript = false, MethodToken[]? methodTokens = null)
    {
        /// <summary>
        /// The covered instruction
        /// </summary>
        public Instruction Instruction { get; } = instruction;

        /// <summary>
        /// The instruction offset
        /// </summary>
        public int Offset { get; } = offset;

        /// <summary>
        /// The instruction description
        /// </summary>
        public string Description => DescriptionFromInstruction(Instruction, MethodTokens);

        /// <summary>
        /// Method tokens
        /// </summary>
        public MethodToken[]? MethodTokens { get; } = methodTokens;

        /// <summary>
        /// The instruction is out of the script
        /// </summary>
        public bool OutOfScript { get; } = outOfScript;

        /// <summary>
        /// Hits
        /// </summary>
        public int Hits { get; private set; }

        /// <summary>
        /// Minimum used fee (In the unit of datoshi, 1 datoshi = 1e-8 GAS)
        /// </summary>
        public long FeeMin { get; private set; }

        /// <summary>
        /// Minimum used fee (In the unit of datoshi, 1 datoshi = 1e-8 GAS)
        /// </summary>
        public long FeeMax { get; private set; }

        /// <summary>
        /// Total used fee (In the unit of datoshi, 1 datoshi = 1e-8 GAS)
        /// </summary>
        public long FeeTotal { get; private set; }

        /// <summary>
        /// Average used fee
        /// </summary>
        public long FeeAvg => Hits == 0 ? 0 : FeeTotal / Hits;

        /// <summary>
        /// Hits
        /// </summary>
        /// <param name="fee">Fee (In the unit of datoshi, 1 datoshi = 1e-8 GAS)</param>
        public void Hit(long fee)
        {
            Hits++;

            if (Hits == 1)
            {
                FeeMin = fee;
                FeeMax = fee;
            }
            else
            {
                FeeMin = Math.Min(FeeMin, fee);
                FeeMax = Math.Max(FeeMax, fee);
            }

            FeeTotal += fee;
        }

        /// <summary>
        /// Hits
        /// </summary>
        /// <param name="value">Value</param>
        public void Hit(CoverageHit value)
        {
            if (value.Hits == 0) return;

            Hits += value.Hits;

            if (Hits == 1)
            {
                FeeMin = value.FeeMin;
                FeeMax = value.FeeMax;
            }
            else
            {
                FeeMin = Math.Min(FeeMin, value.FeeMin);
                FeeMax = Math.Max(FeeMax, value.FeeMax);
            }

            FeeTotal += value.FeeTotal;
        }

        /// <summary>
        /// Clone data
        /// </summary>
        /// <returns>CoverageData</returns>
        public CoverageHit Clone()
        {
            return new CoverageHit(Offset, Instruction, OutOfScript, MethodTokens)
            {
                FeeMax = FeeMax,
                FeeMin = FeeMin,
                FeeTotal = FeeTotal,
                Hits = Hits
            };
        }

        /// <summary>
        /// Return description from instruction
        /// </summary>
        /// <param name="instruction">Instruction</param>
        /// <returns>Description</returns>
        public static string DescriptionFromInstruction(Instruction instruction, params MethodToken[]? tokens)
        {
            if (instruction.Operand.Length > 0)
            {
                var ret = instruction.OpCode.ToString() + " 0x" + instruction.Operand.ToArray().ToHexString();

                switch (instruction.OpCode)
                {
                    case OpCode.CALLT:
                        {
                            var tokenId = instruction.TokenU16;

                            if (tokens != null && tokens.Length > tokenId)
                            {
                                var token = tokens[tokenId];

                                return ret + $" ({token.Hash},{token.Method},{token.ParametersCount},{token.CallFlags})";
                            }
                            break;
                        }
                    case OpCode.JMP:
                    case OpCode.JMPIF:
                    case OpCode.JMPIFNOT:
                    case OpCode.JMPEQ:
                    case OpCode.JMPNE:
                    case OpCode.JMPGT:
                    case OpCode.JMPGE:
                    case OpCode.JMPLT:
                    case OpCode.JMPLE: return ret + $" ({instruction.TokenI8})";
                    case OpCode.JMP_L:
                    case OpCode.JMPIF_L:
                    case OpCode.JMPIFNOT_L:
                    case OpCode.JMPEQ_L:
                    case OpCode.JMPNE_L:
                    case OpCode.JMPGT_L:
                    case OpCode.JMPGE_L:
                    case OpCode.JMPLT_L:
                    case OpCode.JMPLE_L: return ret + $" ({instruction.TokenI32})";
                    case OpCode.SYSCALL:
                        {
                            if (ApplicationEngine.Services.TryGetValue(instruction.TokenU32, out var syscall))
                            {
                                return ret + $" ('{syscall.Name}')";
                            }

                            return ret;
                        }
                }

                if (instruction.Operand.Span.TryGetString(out var str) && str is not null && HexStringInterpreter.HexRegex.IsMatch(str))
                {
                    return ret + $" '{str}'";
                }

                return ret;
            }

            return instruction.OpCode.ToString();
        }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Offset:{Offset}, Description:{Description}, OutOfScript:{OutOfScript}, Hits:{Hits}, FeeTotal:{FeeTotal}, FeeMin:{FeeMin}, FeeMax:{FeeMax}, FeeAvg:{FeeAvg}";
        }
    }
}
