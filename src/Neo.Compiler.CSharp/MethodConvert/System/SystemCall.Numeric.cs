// Copyright (C) 2015-2026 The Neo Project.
//
// SystemCall.Numeric.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private sealed record NumericTypeDescriptor(
        Type ClrType,
        int BitSize,
        bool IsSigned,
        BigInteger MinValue,
        BigInteger MaxValue,
        bool SupportsCopySign);

    private static readonly NumericTypeDescriptor[] s_numericTypeDescriptors =
    {
        new(typeof(byte),   8,  false, BigInteger.Zero, new BigInteger(byte.MaxValue),   false),
        new(typeof(sbyte),  8,  true,  new BigInteger(sbyte.MinValue),  new BigInteger(sbyte.MaxValue),  true),
        new(typeof(short),  16, true,  new BigInteger(short.MinValue),  new BigInteger(short.MaxValue),  true),
        new(typeof(ushort), 16, false, BigInteger.Zero, new BigInteger(ushort.MaxValue), false),
        new(typeof(int),    32, true,  new BigInteger(int.MinValue),    new BigInteger(int.MaxValue),    true),
        new(typeof(uint),   32, false, BigInteger.Zero, new BigInteger(uint.MaxValue),  false),
        new(typeof(long),   64, true,  new BigInteger(long.MinValue),   new BigInteger(long.MaxValue),   true),
        new(typeof(ulong),  64, false, BigInteger.Zero, new BigInteger(ulong.MaxValue), false),
    };


    private static NumericTypeDescriptor GetDescriptor(Type type) => s_numericTypeDescriptors.First(d => d.ClrType == type);

    private static MethodInfo GetRequiredMethod(Type type, string name, params Type[] parameterTypes)
    {
        var method = type.GetMethod(name, parameterTypes);
        if (method is null)
        {
            string signature = string.Join(", ", parameterTypes.Select(t => t.FullName));
            throw new InvalidOperationException($"Expected method '{type.FullName}.{name}({signature})' to exist when registering numeric system calls.");
        }
        return method;
    }

    private static void HandleNumericParse(NumericTypeDescriptor descriptor, MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);

        var endTarget = new JumpTarget();
        methodConvert.Dup();
        if (descriptor.MinValue < 0) // signed integer
        {
            methodConvert.Size();
            methodConvert.Push(descriptor.BitSize / 8);
            methodConvert.JumpIfLessOrEqual(endTarget);
        }
        else
        {
            methodConvert.Within(descriptor.MinValue, descriptor.MaxValue);
            methodConvert.JumpIfTrue(endTarget);
        }

        methodConvert.Throw(); // throw the overflowed value.
        endTarget.Instruction = methodConvert.Nop();
    }

    private static void HandleNumericLeadingZeroCount(NumericTypeDescriptor descriptor, MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        if (descriptor.IsSigned)
        {
            methodConvert.EmitIfComparison(
                () =>
                {
                    methodConvert.Dup();
                    methodConvert.Push0();
                },
                OpCode.JMPGE,
                thenEmitter: () => EmitLeadingZeroCountLoop(methodConvert, descriptor.BitSize),
                elseEmitter: () =>
                {
                    methodConvert.Drop();
                    methodConvert.Push0();
                });
        }
        else
        {
            EmitLeadingZeroCountLoop(methodConvert, descriptor.BitSize);
        }
    }

    private static void PushNumericConstant(MethodConvert methodConvert, NumericTypeDescriptor descriptor, BigInteger value)
    {
        if (descriptor.BitSize <= 8)
        {
            if (descriptor.IsSigned)
                methodConvert.Push((sbyte)value);
            else
                methodConvert.Push((byte)value);
        }
        else if (descriptor.BitSize <= 16)
        {
            if (descriptor.IsSigned)
                methodConvert.Push((short)value);
            else
                methodConvert.Push((ushort)value);
        }
        else if (descriptor.BitSize <= 32)
        {
            if (descriptor.IsSigned)
                methodConvert.Push((int)value);
            else
                methodConvert.Push((uint)value);
        }
        else
        {
            if (descriptor.IsSigned)
                methodConvert.Push((long)value);
            else
                methodConvert.Push((ulong)value);
        }
    }

    private static void PushMinValue(MethodConvert methodConvert, NumericTypeDescriptor descriptor)
        => PushNumericConstant(methodConvert, descriptor, descriptor.MinValue);

    private static void PushMaxValue(MethodConvert methodConvert, NumericTypeDescriptor descriptor)
        => PushNumericConstant(methodConvert, descriptor, descriptor.MaxValue);

    private static void HandleNumericCreateChecked(NumericTypeDescriptor descriptor, MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        var endTarget = new JumpTarget();
        methodConvert.Dup();
        if (descriptor.IsSigned)
        {
            methodConvert.Size();
            methodConvert.Push(descriptor.BitSize / 8);
            methodConvert.JumpIfLessOrEqual(endTarget);
        }
        else
        {
            methodConvert.Within(descriptor.MinValue, descriptor.MaxValue);
            methodConvert.JumpIfTrue(endTarget);
        }
        methodConvert.Throw();
        endTarget.Instruction = methodConvert.Nop();
    }

    private static void HandleNumericCreateSaturating(NumericTypeDescriptor descriptor, MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        PushMinValue(methodConvert, descriptor);
        PushMaxValue(methodConvert, descriptor);                   // value min max
        methodConvert.Reverse3();                                  // max min value
        methodConvert.Max();                                       // max tartget1
        methodConvert.Min();                                       // target2
    }

    private static void HandleNumericCreateTruncating(NumericTypeDescriptor descriptor, MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        methodConvert.PrepareArgumentsForMethod(model, symbol, arguments!);

        methodConvert.Push((BigInteger.One << descriptor.BitSize) - 1);
        methodConvert.And();

        if (descriptor.IsSigned)
            EmitSignedRotateResult(methodConvert, descriptor.BitSize);
    }

    private static void HandleNumericRotateLeft(NumericTypeDescriptor descriptor, MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        using var tempScope = methodConvert.PreserveAnonymousVariables();

        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        var (valueSlot, offsetSlot) = StoreRotateOperands(methodConvert);
        if (descriptor.IsSigned)
            EmitRotateLeftSigned(methodConvert, descriptor.BitSize, valueSlot, offsetSlot);
        else
            EmitRotateLeftUnsigned(methodConvert, descriptor.BitSize, valueSlot, offsetSlot);
    }

    private static void HandleNumericRotateRight(NumericTypeDescriptor descriptor, MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        using var tempScope = methodConvert.PreserveAnonymousVariables();

        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        var (valueSlot, offsetSlot) = StoreRotateOperands(methodConvert);
        if (descriptor.IsSigned)
            EmitRotateRightSigned(methodConvert, descriptor.BitSize, valueSlot, offsetSlot);
        else
            EmitRotateRightUnsigned(methodConvert, descriptor.BitSize, valueSlot, offsetSlot);
    }

    private static void HandleNumericPopCount(NumericTypeDescriptor descriptor, MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.EmitPopCountWithMask(descriptor.BitSize);
    }

    private static void HandleNumericCopySign(NumericTypeDescriptor descriptor, MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleBigIntegerCopySign(methodConvert, model, symbol, instanceExpression, arguments);
        var withinTarget = new JumpTarget();
        methodConvert.Dup();
        PushMaxValue(methodConvert, descriptor);
        methodConvert.JumpIfLessOrEqual(withinTarget);
        methodConvert.Throw();
        withinTarget.Instruction = methodConvert.Nop();
    }

    private static void EmitLeadingZeroCountLoop(MethodConvert methodConvert, int bitSize)
    {
        methodConvert.Push(0);
        methodConvert.EmitWhileComparisonTrueExit(
            perIterationSetup: () => methodConvert.Swap(),
            comparisonSetup: () => methodConvert.Dup(),
            comparisonOp: OpCode.JMPIFNOT,
            bodyEmitter: scope =>
            {
                methodConvert.Push1();
                methodConvert.ShR();
                methodConvert.Swap();
                methodConvert.Inc();
            },
            exitEmitter: () => methodConvert.Drop());
        methodConvert.Push(bitSize);
        methodConvert.Swap();
        methodConvert.Sub();
    }

    private static void RegisterNumericHandlers(NumericTypeDescriptor descriptor)
    {
        var type = descriptor.ClrType;

        var parseMethod = type.GetMethod("Parse", new[] { typeof(string) });
        if (parseMethod is not null)
            RegisterNumericMethod(parseMethod, (mc, model, symbol, instanceExpression, arguments) => HandleNumericParse(descriptor, mc, model, symbol, instanceExpression, arguments));

        var leadingZeroCountMethod = GetRequiredMethod(type, "LeadingZeroCount", type);
        RegisterNumericMethod(leadingZeroCountMethod, (mc, model, symbol, instanceExpression, arguments) => HandleNumericLeadingZeroCount(descriptor, mc, model, symbol, instanceExpression, arguments));

        if (descriptor.SupportsCopySign)
        {
            var copySignMethod = type.GetMethod("CopySign", new[] { type, type });
            if (copySignMethod is not null)
                RegisterNumericMethod(copySignMethod, (mc, model, symbol, instanceExpression, arguments) => HandleNumericCopySign(descriptor, mc, model, symbol, instanceExpression, arguments));
        }

        var rotateLeftMethod = type.GetMethod("RotateLeft", new[] { type, typeof(int) });
        if (rotateLeftMethod is not null)
            RegisterNumericMethod(rotateLeftMethod, (mc, model, symbol, instanceExpression, arguments) => HandleNumericRotateLeft(descriptor, mc, model, symbol, instanceExpression, arguments));

        var rotateRightMethod = type.GetMethod("RotateRight", new[] { type, typeof(int) });
        if (rotateRightMethod is not null)
            RegisterNumericMethod(rotateRightMethod, (mc, model, symbol, instanceExpression, arguments) => HandleNumericRotateRight(descriptor, mc, model, symbol, instanceExpression, arguments));

        var popCountMethod = type.GetMethod("PopCount", new[] { type });
        if (popCountMethod is not null)
            RegisterNumericMethod(popCountMethod, (mc, model, symbol, instanceExpression, arguments) => HandleNumericPopCount(descriptor, mc, model, symbol, instanceExpression, arguments));

        RegisterCreateCheckedHandlers(type, (mc, model, symbol, instanceExpression, arguments) => HandleNumericCreateChecked(descriptor, mc, model, symbol, instanceExpression, arguments));
        RegisterCreateSaturatingHandlers(type, (mc, model, symbol, instanceExpression, arguments) => HandleNumericCreateSaturating(descriptor, mc, model, symbol, instanceExpression, arguments));
        RegisterCreateTruncatingHandlers(type, (mc, model, symbol, instanceExpression, arguments) => HandleNumericCreateTruncating(descriptor, mc, model, symbol, instanceExpression, arguments));
    }

    private static void RegisterNumericMethod(MethodInfo method, SystemCallHandler handler)
    {
        var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
        var key = GetMethodKey(method, parameterTypes);
        SystemCallHandlers[key] = handler;
    }

    private static (byte valueSlot, byte offsetSlot) StoreRotateOperands(MethodConvert methodConvert)
    {
        byte offsetSlot = methodConvert.AddAnonymousVariable();
        byte valueSlot = methodConvert.AddAnonymousVariable();
        methodConvert.AccessSlot(OpCode.STLOC, offsetSlot);
        methodConvert.AccessSlot(OpCode.STLOC, valueSlot);
        return (valueSlot, offsetSlot);
    }

    private static void LoadMaskedRotateValue(MethodConvert methodConvert, byte valueSlot, BigInteger mask)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, valueSlot);
        methodConvert.Push(mask);
        methodConvert.And();
    }

    private static void LoadMaskedRotateOffset(MethodConvert methodConvert, byte offsetSlot, int bitWidth)
    {
        methodConvert.AccessSlot(OpCode.LDLOC, offsetSlot);
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
    }

    private static void LoadInverseRotateOffset(MethodConvert methodConvert, byte offsetSlot, int bitWidth)
    {
        methodConvert.Push(bitWidth);
        LoadMaskedRotateOffset(methodConvert, offsetSlot, bitWidth);
        methodConvert.Sub();
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
    }

    private static void EmitSignedRotateResult(MethodConvert methodConvert, int bitWidth)
    {
        methodConvert.Dup();
        methodConvert.Push(BigInteger.One << (bitWidth - 1));
        JumpTarget endTarget = new();
        methodConvert.JumpIfLess(endTarget);
        methodConvert.Push(BigInteger.One << bitWidth);
        methodConvert.Sub();
        endTarget.Instruction = methodConvert.Nop();
    }

    private static void EmitRotateLeftUnsigned(MethodConvert methodConvert, int bitWidth, byte valueSlot, byte offsetSlot)
    {
        var mask = (BigInteger.One << bitWidth) - 1;
        LoadMaskedRotateValue(methodConvert, valueSlot, mask);
        LoadMaskedRotateOffset(methodConvert, offsetSlot, bitWidth);
        methodConvert.ShL();
        methodConvert.Push(mask);
        methodConvert.And();
        LoadMaskedRotateValue(methodConvert, valueSlot, mask);
        LoadInverseRotateOffset(methodConvert, offsetSlot, bitWidth);
        methodConvert.ShR();
        methodConvert.Or();
        methodConvert.Push(mask);
        methodConvert.And();
    }

    private static void EmitRotateLeftSigned(MethodConvert methodConvert, int bitWidth, byte valueSlot, byte offsetSlot)
    {
        EmitRotateLeftUnsigned(methodConvert, bitWidth, valueSlot, offsetSlot);
        EmitSignedRotateResult(methodConvert, bitWidth);
    }

    private static void EmitRotateRightUnsigned(MethodConvert methodConvert, int bitWidth, byte valueSlot, byte offsetSlot)
    {
        var mask = (BigInteger.One << bitWidth) - 1;
        LoadMaskedRotateValue(methodConvert, valueSlot, mask);
        LoadMaskedRotateOffset(methodConvert, offsetSlot, bitWidth);
        methodConvert.ShR();
        LoadMaskedRotateValue(methodConvert, valueSlot, mask);
        LoadInverseRotateOffset(methodConvert, offsetSlot, bitWidth);
        methodConvert.ShL();
        methodConvert.Or();
        methodConvert.Push(mask);
        methodConvert.And();
    }

    private static void EmitRotateRightSigned(MethodConvert methodConvert, int bitWidth, byte valueSlot, byte offsetSlot)
    {
        EmitRotateRightUnsigned(methodConvert, bitWidth, valueSlot, offsetSlot);
        EmitSignedRotateResult(methodConvert, bitWidth);
    }

}
