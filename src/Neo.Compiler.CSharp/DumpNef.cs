using Neo.SmartContract;
using Neo.SmartContract.Native;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using Neo.VM;

namespace Neo.Optimizer
{
    public static class DumpNef
    {
        public static string GetInstructionAddressPadding(this Script script)
        {
            var digitCount = EnumerateInstructions(script).Last().address switch
            {
                var x when x < 10 => 1,
                var x when x < 100 => 2,
                var x when x < 1000 => 3,
                var x when x < 10000 => 4,
                var x when x <= ushort.MaxValue => 5,
                _ => throw new Exception($"Max script length is {ushort.MaxValue} bytes"),
            };
            return new string('0', digitCount);
        }

        public static string WriteInstruction(int address, Instruction instruction, string padString, MethodToken[] tokens)
        {
            string result = "";
            try
            {
                result += $"{address.ToString(padString)}";
                result += $" {instruction.OpCode}";
                if (!instruction.Operand.IsEmpty)
                    result += $" {instruction.GetOperandString()}";

                var comment = instruction.GetComment(address, tokens);
                if (comment.Length > 0)
                    result += $" # {comment}";
            }
            finally { }
            return result;
        }

        public static IEnumerable<(int address, Instruction instruction)> EnumerateInstructions(this Script script, bool print = false)
        {
            var address = 0;
            var opcode = OpCode.PUSH0;
            Instruction instruction;
            while (address < script.Length)
            {
                instruction = script.GetInstruction(address);
                opcode = instruction.OpCode;
                if (print)
                    Console.WriteLine(WriteInstruction(address, instruction, "0000", new MethodToken[] { }));
                yield return (address, instruction);
                address += instruction.Size;
            }

            if (opcode != OpCode.RET)
            {
                yield return (address, Instruction.RET);
            }
        }

        public static string GetOperandString(this Instruction instruction)
        {
            return string.Create<ReadOnlyMemory<byte>>(instruction.Operand.Length * 3 - 1,
                instruction.Operand, (span, memory) =>
                {
                    var first = memory.Span[0];
                    span[0] = GetHexValue(first / 16);
                    span[1] = GetHexValue(first % 16);

                    var index = 1;
                    for (var i = 2; i < span.Length; i += 3)
                    {
                        var b = memory.Span[index++];
                        span[i] = '-';
                        span[i + 1] = GetHexValue(b / 16);
                        span[i + 2] = GetHexValue(b % 16);
                    }
                });

            static char GetHexValue(int i) => (i < 10) ? (char)(i + '0') : (char)(i - 10 + 'A');
        }

        static readonly Lazy<IReadOnlyDictionary<uint, string>> sysCallNames = new(
            () => ApplicationEngine.Services.ToImmutableDictionary(kvp => kvp.Value.Hash, kvp => kvp.Value.Name));

        public static string GetComment(this Instruction instruction, int ip, MethodToken[]? tokens = null)
        {
            tokens ??= Array.Empty<MethodToken>();

            switch (instruction.OpCode)
            {
                case OpCode.PUSHINT8:
                case OpCode.PUSHINT16:
                case OpCode.PUSHINT32:
                case OpCode.PUSHINT64:
                case OpCode.PUSHINT128:
                case OpCode.PUSHINT256:
                    return $"{new BigInteger(instruction.Operand.Span)}";
                case OpCode.PUSHA:
                    return $"{checked(ip + instruction.TokenI32)}";
                case OpCode.PUSHDATA1:
                case OpCode.PUSHDATA2:
                case OpCode.PUSHDATA4:
                    {
                        var text = System.Text.Encoding.UTF8.GetString(instruction.Operand.Span)
                            .Replace("\r", "\"\\r\"").Replace("\n", "\"\\n\"");
                        if (instruction.Operand.Length == 20)
                        {
                            return $"as script hash: {new UInt160(instruction.Operand.Span)}, as text: \"{text}\"";
                        }
                        return $"as text: \"{text}\"";
                    }
                case OpCode.JMP:
                case OpCode.JMPIF:
                case OpCode.JMPIFNOT:
                case OpCode.JMPEQ:
                case OpCode.JMPNE:
                case OpCode.JMPGT:
                case OpCode.JMPGE:
                case OpCode.JMPLT:
                case OpCode.JMPLE:
                case OpCode.CALL:
                    return OffsetComment(instruction.TokenI8);
                case OpCode.JMP_L:
                case OpCode.JMPIF_L:
                case OpCode.JMPIFNOT_L:
                case OpCode.JMPEQ_L:
                case OpCode.JMPNE_L:
                case OpCode.JMPGT_L:
                case OpCode.JMPGE_L:
                case OpCode.JMPLT_L:
                case OpCode.JMPLE_L:
                case OpCode.CALL_L:
                    return OffsetComment(instruction.TokenI32);
                case OpCode.CALLT:
                    {
                        int index = instruction.TokenU16;
                        if (index >= tokens.Length)
                            return $"Unknown token {instruction.TokenU16}";
                        var token = tokens[index];
                        var contract = NativeContract.Contracts.SingleOrDefault(c => c.Hash == token.Hash);
                        var tokenName = contract is null ? $"{token.Hash}" : contract.Name;
                        return $"{tokenName}.{token.Method} token call";
                    }
                case OpCode.TRY:
                    return TryComment(instruction.TokenI8, instruction.TokenI8_1);
                case OpCode.TRY_L:
                    return TryComment(instruction.TokenI32, instruction.TokenI32_1);
                case OpCode.ENDTRY:
                    return OffsetComment(instruction.TokenI8);
                case OpCode.ENDTRY_L:
                    return OffsetComment(instruction.TokenI32);
                case OpCode.SYSCALL:
                    return sysCallNames.Value.TryGetValue(instruction.TokenU32, out var name)
                        ? $"{name} SysCall"
                        : $"Unknown SysCall {instruction.TokenU32}";
                case OpCode.INITSSLOT:
                    return $"{instruction.TokenU8} static variables";
                case OpCode.INITSLOT:
                    return $"{instruction.TokenU8} local variables, {instruction.TokenU8_1} arguments";
                case OpCode.LDSFLD:
                case OpCode.STSFLD:
                case OpCode.LDLOC:
                case OpCode.STLOC:
                case OpCode.LDARG:
                case OpCode.STARG:
                    return $"Slot index {instruction.TokenU8}";
                case OpCode.NEWARRAY_T:
                case OpCode.ISTYPE:
                case OpCode.CONVERT:
                    return $"{(VM.Types.StackItemType)instruction.TokenU8} type";
                default:
                    return string.Empty;
            }

            string OffsetComment(int offset) => $"pos: {checked(ip + offset)} (offset: {offset})";
            string TryComment(int catchOffset, int finallyOffset)
            {
                var builder = new System.Text.StringBuilder();
                builder.Append(catchOffset == 0 ? "no catch block, " : $"catch {OffsetComment(catchOffset)}, ");
                builder.Append(finallyOffset == 0 ? "no finally block" : $"finally {OffsetComment(finallyOffset)}");
                return builder.ToString();
            }
        }
    }
}
