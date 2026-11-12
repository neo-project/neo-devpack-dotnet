// Copyright (C) 2015-2026 The Neo Project.
//
// ConstantFold.cs file belongs to the neo project and is free
// software: you can redistribute it and/or modify it under the terms of the
// MIT software license, see the accompanying file LICENSE in the main
// directory of the repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms, with or without
// modifications, are permitted.

using Neo.Compiler.ControlFlow;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using VmInstruction = Neo.VM.Instruction;

namespace Neo.Optimizer
{
    /// <summary>
    /// Folds arithmetic, bitwise, comparison and byte-string operations whose operands are
    /// both constant pushes into a single constant push.
    /// </summary>
    /// <remarks>
    /// The pass runs after <see cref="Peephole.InitStaticToConst"/> so combinations of
    /// statically initialized fields (e.g. <c>static readonly int A = 7, B = 5; A + B</c>)
    /// appear as adjacent constant pushes, and before
    /// <see cref="JumpCompresser.FoldConstantConditionalJump"/> so comparisons folded to
    /// PUSHT/PUSHF are consumed by constant-conditional jump folding.
    ///
    /// The VM evaluates these opcodes on arbitrary-precision integers, so folding computes
    /// the same BigInteger result at compile time. Any width normalization the compiler
    /// emitted after the operation (range checks, masks) still applies to the folded
    /// constant, which preserves the declared-width semantics.
    ///
    /// Folding is skipped when a jump or exception-handler entry point targets the operand
    /// or the operation itself, because such a path would observe only part of the pattern.
    /// </remarks>
    public static class ConstantFold
    {
        private static readonly HashSet<OpCode> NumericPushOpCodes = new()
        {
            OpCode.PUSHM1,
            OpCode.PUSH0, OpCode.PUSH1, OpCode.PUSH2, OpCode.PUSH3, OpCode.PUSH4,
            OpCode.PUSH5, OpCode.PUSH6, OpCode.PUSH7, OpCode.PUSH8, OpCode.PUSH9,
            OpCode.PUSH10, OpCode.PUSH11, OpCode.PUSH12, OpCode.PUSH13, OpCode.PUSH14,
            OpCode.PUSH15, OpCode.PUSH16,
            OpCode.PUSHINT8, OpCode.PUSHINT16, OpCode.PUSHINT32, OpCode.PUSHINT64,
            OpCode.PUSHINT128, OpCode.PUSHINT256,
        };

        private static readonly HashSet<OpCode> BooleanPushOpCodes = new()
        {
            OpCode.PUSHT, OpCode.PUSHF,
        };

        private static readonly HashSet<OpCode> DataPushOpCodes = new()
        {
            OpCode.PUSHDATA1, OpCode.PUSHDATA2, OpCode.PUSHDATA4,
        };

        private const int MaxFoldedShiftCount = 256;

        [Strategy(Priority = 1 << 4)]
        public static (NefFile, ContractManifest, JObject?) FoldConstants(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            ContractInBasicBlocks contractInBasicBlocks = new(nef, manifest, debugInfo);
            InstructionCoverage oldContractCoverage = contractInBasicBlocks.coverage;
            Dictionary<int, VmInstruction> oldAddressToInstruction = oldContractCoverage.addressToInstructions;
            (Dictionary<VmInstruction, VmInstruction> jumpSourceToTargets,
                Dictionary<VmInstruction, (VmInstruction, VmInstruction)> trySourceToTargets,
                Dictionary<VmInstruction, HashSet<VmInstruction>> jumpTargetToSources) =
                (oldContractCoverage.jumpInstructionSourceToTargets,
                oldContractCoverage.tryInstructionSourceToTargets,
                oldContractCoverage.jumpTargetToSources);
            System.Collections.Specialized.OrderedDictionary simplifiedInstructionsToAddress = new();
            Dictionary<int, int> oldSequencePointAddressToNew = new();
            int currentAddress = 0;
            foreach ((int oldStartAddr, List<VmInstruction> basicBlock) in contractInBasicBlocks.sortedListInstructions)
            {
                int oldAddr = oldStartAddr;
                for (int index = 0; index < basicBlock.Count; index++)
                {
                    VmInstruction current = basicBlock[index];

                    VmInstruction? folded = null;
                    int consumed = 1;
                    if (index + 2 < basicBlock.Count)
                    {
                        VmInstruction second = basicBlock[index + 1];
                        VmInstruction third = basicBlock[index + 2];
                        if (IsSafeToRemove(second, third, jumpTargetToSources, trySourceToTargets))
                        {
                            folded = TryFoldBinary(current, second, third);
                            if (folded is not null) consumed = 3;
                        }
                    }
                    if (folded is null && index + 1 < basicBlock.Count)
                    {
                        VmInstruction second = basicBlock[index + 1];
                        if (IsSafeToRemove(second, jumpTargetToSources, trySourceToTargets))
                        {
                            folded = TryFoldUnary(current, second);
                            if (folded is not null) consumed = 2;
                        }
                    }

                    if (folded is not null)
                    {
                        simplifiedInstructionsToAddress.Add(folded, currentAddress);
                        for (int k = 0; k < consumed; k++)
                        {
                            oldSequencePointAddressToNew.Add(oldAddr, currentAddress);
                            oldAddr += basicBlock[index + k].Size;
                        }
                        currentAddress += folded.Size;
                        // Anything that jumped to the first consumed instruction now lands on the folded push.
                        OptimizedScriptBuilder.RetargetJump(current, folded,
                            jumpSourceToTargets, trySourceToTargets, jumpTargetToSources);
                        index += consumed - 1;
                        continue;
                    }

                    simplifiedInstructionsToAddress.Add(current, currentAddress);
                    currentAddress += current.Size;
                    oldAddr += current.Size;
                }
            }
            return AssetBuilder.BuildOptimizedAssets(nef, manifest, debugInfo,
                simplifiedInstructionsToAddress,
                jumpSourceToTargets, trySourceToTargets,
                oldAddressToInstruction, oldSequencePointAddressToNew: oldSequencePointAddressToNew);
        }

        private static bool IsSafeToRemove(
            VmInstruction second,
            VmInstruction third,
            Dictionary<VmInstruction, HashSet<VmInstruction>> jumpTargetToSources,
            Dictionary<VmInstruction, (VmInstruction, VmInstruction)> trySourceToTargets) =>
            IsSafeToRemove(second, jumpTargetToSources, trySourceToTargets)
            && IsSafeToRemove(third, jumpTargetToSources, trySourceToTargets);

        private static bool IsSafeToRemove(
            VmInstruction instruction,
            Dictionary<VmInstruction, HashSet<VmInstruction>> jumpTargetToSources,
            Dictionary<VmInstruction, (VmInstruction, VmInstruction)> trySourceToTargets)
        {
            if (jumpTargetToSources.TryGetValue(instruction, out HashSet<VmInstruction>? sources) && sources.Count > 0)
                return false;
            foreach ((_, (VmInstruction t1, VmInstruction t2)) in trySourceToTargets)
                if (t1 == instruction || t2 == instruction)
                    return false;
            return true;
        }

        private static VmInstruction? TryFoldBinary(VmInstruction left, VmInstruction right, VmInstruction op)
        {
            if (TryGetPushedInteger(left, out BigInteger l) && TryGetPushedInteger(right, out BigInteger r))
            {
                return op.OpCode switch
                {
                    OpCode.ADD => PushInteger(l + r),
                    OpCode.SUB => PushInteger(l - r),
                    OpCode.MUL => PushInteger(l * r),
                    OpCode.DIV => r.IsZero ? null : PushInteger(BigInteger.Divide(l, r)),
                    OpCode.MOD => r.IsZero ? null : PushInteger(l - r * BigInteger.Divide(l, r)),
                    OpCode.AND => PushInteger(l & r),
                    OpCode.OR => PushInteger(l | r),
                    OpCode.XOR => PushInteger(l ^ r),
                    OpCode.SHL => r.Sign >= 0 && r <= MaxFoldedShiftCount ? PushInteger(l << (int)r) : null,
                    OpCode.SHR => r.Sign >= 0 && r <= MaxFoldedShiftCount ? PushInteger(l >> (int)r) : null,
                    OpCode.NUMEQUAL => PushBoolean(l == r),
                    OpCode.NUMNOTEQUAL => PushBoolean(l != r),
                    OpCode.LT => PushBoolean(l < r),
                    OpCode.LE => PushBoolean(l <= r),
                    OpCode.GT => PushBoolean(l > r),
                    OpCode.GE => PushBoolean(l >= r),
                    OpCode.MIN => PushInteger(BigInteger.Min(l, r)),
                    OpCode.MAX => PushInteger(BigInteger.Max(l, r)),
                    _ => null,
                };
            }

            if (TryGetPushedBytes(left, out byte[] lb) && TryGetPushedBytes(right, out byte[] rb))
            {
                return op.OpCode switch
                {
                    OpCode.CAT => PushBytes([.. lb, .. rb]),
                    OpCode.EQUAL => PushBoolean(lb.SequenceEqual(rb)),
                    OpCode.NOTEQUAL => PushBoolean(!lb.SequenceEqual(rb)),
                    _ => null,
                };
            }

            if (TryGetPushedBoolean(left, out bool b1) && TryGetPushedBoolean(right, out bool b2))
            {
                return op.OpCode switch
                {
                    OpCode.BOOLAND => PushBoolean(b1 && b2),
                    OpCode.BOOLOR => PushBoolean(b1 || b2),
                    _ => null,
                };
            }

            return null;
        }

        private static VmInstruction? TryFoldUnary(VmInstruction first, VmInstruction op)
        {
            if (TryGetPushedInteger(first, out BigInteger value))
            {
                return op.OpCode switch
                {
                    // VM NOT converts any value to its boolean negation.
                    OpCode.NOT => PushBoolean(value.IsZero),
                    OpCode.ABS => PushInteger(BigInteger.Abs(value)),
                    _ => null,
                };
            }

            if (TryGetPushedBoolean(first, out bool b))
            {
                return op.OpCode == OpCode.NOT ? PushBoolean(!b) : null;
            }

            if (TryGetPushedBytes(first, out byte[] data))
            {
                return op.OpCode == OpCode.SIZE ? PushInteger(data.Length) : null;
            }

            return null;
        }

        private static bool TryGetPushedInteger(VmInstruction instruction, out BigInteger value)
        {
            int opCode = (int)instruction.OpCode;
            if (opCode >= (int)OpCode.PUSH0 && opCode <= (int)OpCode.PUSH16)
            {
                value = opCode - (int)OpCode.PUSH0;
                return true;
            }
            if (instruction.OpCode == OpCode.PUSHM1)
            {
                value = BigInteger.MinusOne;
                return true;
            }
            if (NumericPushOpCodes.Contains(instruction.OpCode))
            {
                value = new BigInteger(instruction.Operand.Span);
                return true;
            }

            value = BigInteger.Zero;
            return false;
        }

        private static bool TryGetPushedBoolean(VmInstruction instruction, out bool value)
        {
            if (BooleanPushOpCodes.Contains(instruction.OpCode))
            {
                value = instruction.OpCode == OpCode.PUSHT;
                return true;
            }

            value = false;
            return false;
        }

        private static bool TryGetPushedBytes(VmInstruction instruction, out byte[] data)
        {
            if (DataPushOpCodes.Contains(instruction.OpCode))
            {
                data = instruction.Operand.ToArray();
                return true;
            }

            data = Array.Empty<byte>();
            return false;
        }

        private static VmInstruction PushBoolean(bool value) =>
            CreateInstruction(value ? OpCode.PUSHT : OpCode.PUSHF, ReadOnlySpan<byte>.Empty);

        /// <summary>
        /// Neo.VM.Instruction has no parameterless constructor; decoding a one-instruction
        /// script is the established construction pattern (see JumpCompresser).
        /// </summary>
        private static VmInstruction CreateInstruction(OpCode opcode, ReadOnlySpan<byte> operand)
        {
            byte[] buffer = new byte[1 + operand.Length];
            buffer[0] = (byte)opcode;
            operand.CopyTo(buffer.AsSpan(1));
            return new Script(buffer).GetInstruction(0);
        }

        private static VmInstruction? PushInteger(BigInteger number)
        {
            if (number >= -1 && number <= 16)
                return CreateInstruction(
                    number == -1 ? OpCode.PUSHM1 : (OpCode)((int)OpCode.PUSH0 + (int)number),
                    ReadOnlySpan<byte>.Empty);

            Span<byte> buffer = stackalloc byte[32];
            if (!number.TryWriteBytes(buffer, out int bytesWritten, isUnsigned: false, isBigEndian: false))
                return null; // out of the 256-bit operand range; leave the pattern unfolded
            return CreateInstruction(
                bytesWritten switch
                {
                    1 => OpCode.PUSHINT8,
                    2 => OpCode.PUSHINT16,
                    <= 4 => OpCode.PUSHINT32,
                    <= 8 => OpCode.PUSHINT64,
                    <= 16 => OpCode.PUSHINT128,
                    _ => OpCode.PUSHINT256,
                },
                PadRight(buffer, bytesWritten, instructionOperandSize(bytesWritten), number.Sign < 0));

            static int instructionOperandSize(int bytesWritten) => bytesWritten switch
            {
                1 => 1,
                2 => 2,
                <= 4 => 4,
                <= 8 => 8,
                <= 16 => 16,
                _ => 32,
            };
        }

        private static VmInstruction? PushBytes(byte[] data)
        {
            if (data.Length <= byte.MaxValue)
            {
                byte[] operand = new byte[sizeof(byte) + data.Length];
                operand[0] = (byte)data.Length;
                Buffer.BlockCopy(data, 0, operand, sizeof(byte), data.Length);
                return CreateInstruction(OpCode.PUSHDATA1, operand);
            }
            if (data.Length <= ushort.MaxValue)
            {
                byte[] operand = new byte[sizeof(ushort) + data.Length];
                BinaryPrimitives.WriteUInt16LittleEndian(operand, (ushort)data.Length);
                Buffer.BlockCopy(data, 0, operand, sizeof(ushort), data.Length);
                return CreateInstruction(OpCode.PUSHDATA2, operand);
            }
            if (data.Length <= int.MaxValue)
            {
                byte[] operand = new byte[sizeof(uint) + data.Length];
                BinaryPrimitives.WriteUInt32LittleEndian(operand, (uint)data.Length);
                Buffer.BlockCopy(data, 0, operand, sizeof(uint), data.Length);
                return CreateInstruction(OpCode.PUSHDATA4, operand);
            }
            return null;
        }

        /// <summary>
        /// Pads the little-endian two's-complement representation with sign bytes to the
        /// declared operand width, mirroring the compiler's constant encoding.
        /// </summary>
        private static ReadOnlySpan<byte> PadRight(ReadOnlySpan<byte> buffer, int bytesWritten, int size, bool sign)
        {
            Span<byte> result = new byte[size];
            buffer[..bytesWritten].CopyTo(result);
            byte padding = sign ? (byte)0xFF : (byte)0x00;
            result[bytesWritten..].Fill(padding);
            return result;
        }
    }
}
