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
using scfx::Neo.SmartContract.Framework;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using OpCode = Neo.VM.OpCode;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    #region Instructions

    private Instruction AddInstruction(Instruction instruction)
    {
        _instructions.Add(instruction);
        return instruction;
    }

    private Instruction AddInstruction(OpCode opcode)
    {
        return AddInstruction(new Instruction
        {
            OpCode = opcode
        });
    }

    private SequencePointInserter InsertSequencePoint(SyntaxNodeOrToken? syntax, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string? callerPath = null, [CallerMemberName] string? caller = null)
    {
        return new SequencePointInserter(_instructions, syntax, LocationInformation.BuildCompilerLocation(lineNumber, callerPath, caller));
    }

    private SequencePointInserter InsertSequencePoint(SyntaxReference? syntax, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string? callerPath = null, [CallerMemberName] string? caller = null)
    {
        return new SequencePointInserter(_instructions, syntax, LocationInformation.BuildCompilerLocation(lineNumber, callerPath, caller));
    }

    private SequencePointInserter InsertSequencePoint(Location? location, [CallerLineNumber] int lineNumber = 0, [CallerFilePath] string? callerPath = null, [CallerMemberName] string? caller = null)
    {
        return new SequencePointInserter(_instructions, location, LocationInformation.BuildCompilerLocation(lineNumber, callerPath, caller));
    }

    #endregion

    private Instruction Jump(OpCode opcode, JumpTarget target)
    {
        return AddInstruction(new Instruction
        {
            OpCode = opcode,
            Target = target
        });
    }

    private void Push(bool value)
    {
        AddInstruction(value ? OpCode.PUSHT : OpCode.PUSHF);
    }

    private Instruction Ret() => AddInstruction(OpCode.RET);

    private Instruction Push(BigInteger number)
    {
        if (number >= -1 && number <= 16) return AddInstruction(number == -1 ? OpCode.PUSHM1 : OpCode.PUSH0 + (byte)(int)number);
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
            <= 4 => new Instruction
            {
                OpCode = OpCode.PUSHINT32,
                Operand = PadRight(buffer, bytesWritten, 4, number.Sign < 0).ToArray()
            },
            <= 8 => new Instruction
            {
                OpCode = OpCode.PUSHINT64,
                Operand = PadRight(buffer, bytesWritten, 8, number.Sign < 0).ToArray()
            },
            <= 16 => new Instruction
            {
                OpCode = OpCode.PUSHINT128,
                Operand = PadRight(buffer, bytesWritten, 16, number.Sign < 0).ToArray()
            },
            <= 32 => new Instruction
            {
                OpCode = OpCode.PUSHINT256,
                Operand = PadRight(buffer, bytesWritten, 32, number.Sign < 0).ToArray()
            },
            _ => throw new ArgumentOutOfRangeException($"Number too large: {bytesWritten}")
        };
        AddInstruction(instruction);
        return instruction;
    }

    /// <summary>
    /// If all char in the input is no greater than byte.MaxValue == 255, get the byte for each char
    /// If any char in the input is greater than byte.MaxValue == 255, encode in UTF8
    /// This ensures "a\xff\x80\x79\x00" accurately translated to 0x61 0xff 0x80 0x79 0x00
    /// </summary>
    /// <param name="s">String value to push</param>
    /// <returns>Instruction</returns>
    private Instruction Push(string s)
    {
        try
        {// Handle byte-like "\xff\x80\x79\x00..."
         // fails on non-ascii char (> byte.MaxValue == 255) like "悪い文字"
         // fails on long \x, e.g. \x123, \x1234
         // does not fail on ascii chars
            MemoryStream pushed = new();
            BinaryWriter writer = new(pushed);
            foreach (char c in s)
                writer.Write(System.Convert.ToByte(c));
            return Push(pushed.ToArray());
        }
        catch { }
        return Push(Utility.StrictUTF8.GetBytes(s));
        // \xff (and each byte >= \x80) will be decoded to 2 bytes (0xc3 0xbf for \xff) by UTF8 decoder
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
        {// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/default-values
            VM.Types.StackItemType.Boolean => OpCode.PUSHF,
            VM.Types.StackItemType.Integer => OpCode.PUSH0,
            _ => OpCode.PUSHNULL,
        });
    }

    // Helper method to reverse stack items
    private void ReverseStackItems(int count)
    {
        switch (count)
        {
            case 2:
                AddInstruction(OpCode.SWAP);
                break;
            case 3:
                AddInstruction(OpCode.REVERSE3);
                break;
            case 4:
                AddInstruction(OpCode.REVERSE4);
                break;
            default:
                Push(count);
                AddInstruction(OpCode.REVERSEN);
                break;
        }
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

    /// <summary>
    /// Convert a throw expression or throw statement to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about the Throw.</param>
    /// <param name="exception">The content of exception</param>
    /// <exception cref="CompilationException">Only a single parameter is supported for exceptions.</exception>
    /// <example>
    /// throw statement:
    /// <code>
    /// if (shapeAmount <= 0)
    /// {
    ///     throw new Exception("Amount of shapes must be positive.");
    /// }
    ///</code>
    /// throw expression:
    /// <code>
    /// string a = null;
    /// var b = a ?? throw new Exception();
    /// </code>
    /// <code>
    /// var first = args.Length >= 1 ? args[0] : throw new Exception();
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/exception-handling-statements#the-throw-expression">The throw expression</seealso>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/statements/exception-handling-statements#the-try-catch-statement">Exception-handling statements - throw</seealso>
    private void Throw(SemanticModel model, ExpressionSyntax? exception)
    {
        if (exception is not null)
        {
            var type = model.GetTypeInfo(exception).Type!;
            if (type.IsSubclassOf(nameof(UncatchableException), includeThisClass: true))
            {
                AddInstruction(OpCode.ABORT);
                return;
            }
        }
        switch (exception)
        {
            case ObjectCreationExpressionSyntax expression:
                switch (expression.ArgumentList?.Arguments.Count)
                {
                    case null:
                    case 0:
                        Push("exception");
                        break;
                    case 1:
                        ConvertExpression(model, expression.ArgumentList.Arguments[0].Expression);
                        break;
                    default:
                        throw new CompilationException(expression, DiagnosticId.MultiplyThrows, "Only a single parameter is supported for exceptions.");
                }
                break;
            case null:
                AccessSlot(OpCode.LDLOC, _exceptionStack.Peek());
                break;
            default:
                ConvertExpression(model, exception);
                break;
        }
        AddInstruction(OpCode.THROW);
    }

    private Instruction IsType(VM.Types.StackItemType type)
    {
        return AddInstruction(new Instruction
        {
            OpCode = OpCode.ISTYPE,
            Operand = [(byte)type]
        });
    }

    private Instruction ChangeType(VM.Types.StackItemType type)
    {
        return AddInstruction(new Instruction
        {
            OpCode = OpCode.CONVERT,
            Operand = [(byte)type]
        });
    }
}
