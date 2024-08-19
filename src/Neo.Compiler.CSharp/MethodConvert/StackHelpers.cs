// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using System;
using System.Buffers.Binary;
using System.Numerics;

namespace Neo.Compiler;

partial class MethodConvert
{
    private void Push(bool value)
    {
        AddInstruction(value ? OpCode.PUSHT : OpCode.PUSHF);
    }

    private Instruction Push(BigInteger number)
    {
        if (number >= -1 && number <= 16) return AddInstruction(OpCode.PUSH0 + (byte)(int)number);
        Span<byte> buffer = stackalloc byte[32];
        if (!number.TryWriteBytes(buffer, out var bytesWritten, isUnsigned: false, isBigEndian: false))
            throw new ArgumentOutOfRangeException(nameof(number));
        var instruction = bytesWritten switch
        {
            1 => new Instruction
            {
                OpCode = OpCode.PUSHINT8,
                Operand = PadRight(buffer, bytesWritten, 1, number.Sign < 0).ToArray()
            },
            2 => new Instruction
            {
                OpCode = OpCode.PUSHINT16,
                Operand = PadRight(buffer, bytesWritten, 2, number.Sign < 0).ToArray()
            },
            4 => new Instruction
            {
                OpCode = OpCode.PUSHINT32,
                Operand = PadRight(buffer, bytesWritten, 4, number.Sign < 0).ToArray()
            },
            8 => new Instruction
            {
                OpCode = OpCode.PUSHINT64,
                Operand = PadRight(buffer, bytesWritten, 8, number.Sign < 0).ToArray()
            },
            16 => new Instruction
            {
                OpCode = OpCode.PUSHINT128,
                Operand = PadRight(buffer, bytesWritten, 16, number.Sign < 0).ToArray()
            },
            _ => new Instruction
            {
                OpCode = OpCode.PUSHINT256,
                Operand = PadRight(buffer, bytesWritten, 32, number.Sign < 0).ToArray()
            },
        };
        AddInstruction(instruction);
        return instruction;
    }

    private Instruction Push(string s)
    {
        return Push(Utility.StrictUTF8.GetBytes(s));
    }

    private Instruction Push(byte[] data)
    {
        OpCode opcode;
        byte[] buffer;
        switch (data.Length)
        {
            case <= byte.MaxValue:
                opcode = OpCode.PUSHDATA1;
                buffer = new byte[sizeof(byte) + data.Length];
                buffer[0] = (byte)data.Length;
                Buffer.BlockCopy(data, 0, buffer, sizeof(byte), data.Length);
                break;
            case <= ushort.MaxValue:
                opcode = OpCode.PUSHDATA2;
                buffer = new byte[sizeof(ushort) + data.Length];
                BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort)data.Length);
                Buffer.BlockCopy(data, 0, buffer, sizeof(ushort), data.Length);
                break;
            default:
                opcode = OpCode.PUSHDATA4;
                buffer = new byte[sizeof(uint) + data.Length];
                BinaryPrimitives.WriteUInt32LittleEndian(buffer, (uint)data.Length);
                Buffer.BlockCopy(data, 0, buffer, sizeof(uint), data.Length);
                break;
        }
        return AddInstruction(new Instruction
        {
            OpCode = opcode,
            Operand = buffer
        });
    }

    private void Push(object? obj)
    {
        switch (obj)
        {
            case bool data:
                Push(data);
                break;
            case byte[] data:
                Push(data);
                break;
            case string data:
                Push(data);
                break;
            case BigInteger data:
                Push(data);
                break;
            case char data:
                Push((ushort)data);
                break;
            case sbyte data:
                Push(data);
                break;
            case byte data:
                Push(data);
                break;
            case short data:
                Push(data);
                break;
            case ushort data:
                Push(data);
                break;
            case int data:
                Push(data);
                break;
            case uint data:
                Push(data);
                break;
            case long data:
                Push(data);
                break;
            case ulong data:
                Push(data);
                break;
            case Enum data:
                Push(BigInteger.Parse(data.ToString("d")));
                break;
            case null:
                AddInstruction(OpCode.PUSHNULL);
                break;
            case float or double or decimal:
                throw new CompilationException(DiagnosticId.FloatingPointNumber, "Floating-point numbers are not supported.");
            default:
                throw new NotSupportedException($"Unsupported constant value: {obj}");
        }
    }

    private Instruction PushDefault(ITypeSymbol type)
    {
        return AddInstruction(type.GetStackItemType() switch
        {
            VM.Types.StackItemType.Boolean or VM.Types.StackItemType.Integer => OpCode.PUSH0,
            _ => OpCode.PUSHNULL,
        });
    }

    #region LabelsAndTargets

    private JumpTarget AddLabel(ILabelSymbol symbol, bool checkTryStack)
    {
        if (!_labels.TryGetValue(symbol, out JumpTarget? target))
        {
            target = new JumpTarget();
            _labels.Add(symbol, target);
        }
        if (checkTryStack && _tryStack.TryPeek(out ExceptionHandling? result) && result.State != ExceptionHandlingState.Finally)
        {
            result.Labels.Add(symbol);
        }
        return target;
    }

    #endregion

    private void PushSwitchLabels((SwitchLabelSyntax, JumpTarget)[] labels)
    {
        _switchStack.Push(labels);
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.SwitchCount++;
    }

    private void PopSwitchLabels()
    {
        _switchStack.Pop();
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.SwitchCount--;
    }

    private void PushContinueTarget(JumpTarget target)
    {
        _continueTargets.Push(target);
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.ContinueTargetCount++;
    }

    private void PopContinueTarget()
    {
        _continueTargets.Pop();
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.ContinueTargetCount--;
    }

    private void PushBreakTarget(JumpTarget target)
    {
        _breakTargets.Push(target);
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.BreakTargetCount++;
    }

    private void PopBreakTarget()
    {
        _breakTargets.Pop();
        if (_tryStack.TryPeek(out ExceptionHandling? result))
            result.BreakTargetCount--;
    }

    private static ReadOnlySpan<byte> PadRight(Span<byte> buffer, int dataLength, int padLength, bool negative)
    {
        byte pad = negative ? (byte)0xff : (byte)0;
        for (int x = dataLength; x < padLength; x++)
            buffer[x] = pad;
        return buffer[..padLength];
    }
}
