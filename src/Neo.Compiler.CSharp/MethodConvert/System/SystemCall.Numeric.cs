// Copyright (C) 2015-2025 The Neo Project.
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
    private static void HandleNumericParse(NumericTypeDescriptor descriptor, MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);
        methodConvert.EmitIf(
            () =>
            {
                methodConvert.Dup();
                methodConvert.Within(descriptor.MinValue, descriptor.MaxValue);
                methodConvert.Not();
            },
            () => { methodConvert.Throw(); });
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

    private static void HandleNumericCreateChecked(NumericTypeDescriptor descriptor, MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.EmitIf(
            () =>
            {
                methodConvert.Dup();
                methodConvert.Within(descriptor.MinValue, descriptor.MaxValue);
                methodConvert.Not();
            },
            () => { methodConvert.Throw(); });
    }

    private static void HandleNumericCreateSaturating(NumericTypeDescriptor descriptor, MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        EmitClampToRange(methodConvert, descriptor);
    }

    private static void HandleNumericRotateLeft(NumericTypeDescriptor descriptor, MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        if (descriptor.IsSigned)
            EmitRotateLeftSigned(methodConvert, descriptor.BitSize);
        else
            EmitRotateLeftUnsigned(methodConvert, descriptor.BitSize);
    }

    private static void HandleNumericRotateRight(NumericTypeDescriptor descriptor, MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.StdCall);
        if (descriptor.IsSigned)
            EmitRotateRightSigned(methodConvert, descriptor.BitSize);
        else
            EmitRotateRightUnsigned(methodConvert, descriptor.BitSize);
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
        EmitClampToRange(methodConvert, descriptor);
    }

    private static void EmitLeadingZeroCountLoop(MethodConvert methodConvert, int bitSize)
    {
        methodConvert.Push(0);
        methodConvert.EmitWhileComparisonTrueExit(
            perIterationSetup: () => methodConvert.Swap(),
            comparisonSetup: () =>
            {
                methodConvert.Dup();
                methodConvert.Push0();
            },
            comparisonOp: OpCode.JMPEQ,
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

    private static void EmitClampToRange(MethodConvert methodConvert, NumericTypeDescriptor descriptor)
    {
        JumpTarget clampToMin = new();
        JumpTarget clampToMax = new();
        JumpTarget endTarget = new();

        methodConvert.Dup();
        methodConvert.Push(descriptor.MinValue);
        methodConvert.JumpIfLess(clampToMin);

        methodConvert.Dup();
        methodConvert.Push(descriptor.MaxValue);
        methodConvert.JumpIfGreater(clampToMax);

        methodConvert.JumpAlways(endTarget);

        clampToMin.Instruction = methodConvert.Nop();
        methodConvert.Drop();
        methodConvert.Push(descriptor.MinValue);
        methodConvert.JumpAlways(endTarget);

        clampToMax.Instruction = methodConvert.Nop();
        methodConvert.Drop();
        methodConvert.Push(descriptor.MaxValue);

        endTarget.Instruction = methodConvert.Nop();
    }

    private static void RegisterNumericHandlers(NumericTypeDescriptor descriptor)
    {
        var type = descriptor.ClrType;

        var parseMethod = type.GetMethod("Parse", new[] { typeof(string) });
        if (parseMethod is not null)
            RegisterNumericMethod(parseMethod, (mc, model, symbol, instanceExpression, arguments) => HandleNumericParse(descriptor, mc, model, symbol, instanceExpression, arguments));

        var leadingZeroCountMethod = type.GetMethod("LeadingZeroCount", new[] { type });
        if (leadingZeroCountMethod is not null)
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
    }

    private static void RegisterNumericMethod(MethodInfo method, SystemCallHandler handler)
    {
        var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
        var key = GetMethodKey(method, parameterTypes);
        SystemCallHandlers[key] = handler;
    }

    private static void EmitRotateLeftUnsigned(MethodConvert methodConvert, int bitWidth)
    {
        var mask = (BigInteger.One << bitWidth) - 1;
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.Swap();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.Swap();
        methodConvert.ShL();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.LdArg0();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.LdArg1();
        methodConvert.Push(bitWidth);
        methodConvert.Swap();
        methodConvert.Sub();
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.ShR();
        methodConvert.Or();
        methodConvert.Push(mask);
        methodConvert.And();
    }

    private static void EmitRotateLeftSigned(MethodConvert methodConvert, int bitWidth)
    {
        var mask = (BigInteger.One << bitWidth) - 1;
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.Swap();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.Swap();
        methodConvert.ShL();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.LdArg0();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.LdArg1();
        methodConvert.Push(bitWidth);
        methodConvert.Swap();
        methodConvert.Sub();
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.ShR();
        methodConvert.Or();
        methodConvert.Dup();
        methodConvert.Push(BigInteger.One << (bitWidth - 1));
        JumpTarget endTarget = new();
        methodConvert.JumpIfLess(endTarget);
        methodConvert.Push(BigInteger.One << bitWidth);
        methodConvert.Sub();
        endTarget.Instruction = methodConvert.Nop();
    }

    private static void EmitRotateRightUnsigned(MethodConvert methodConvert, int bitWidth)
    {
        var mask = (BigInteger.One << bitWidth) - 1;
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.ShR();
        methodConvert.LdArg0();
        methodConvert.Push(bitWidth);
        methodConvert.LdArg1();
        methodConvert.Sub();
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.ShL();
        methodConvert.Or();
        methodConvert.Push(mask);
        methodConvert.And();
    }

    private static void EmitRotateRightSigned(MethodConvert methodConvert, int bitWidth)
    {
        var mask = (BigInteger.One << bitWidth) - 1;
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.Push(bitWidth);
        methodConvert.Mod();
        methodConvert.Push(bitWidth);
        methodConvert.Swap();
        methodConvert.Sub();
        methodConvert.Swap();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.Swap();
        methodConvert.ShL();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.LdArg0();
        methodConvert.Push(mask);
        methodConvert.And();
        methodConvert.LdArg1();
        methodConvert.Push(bitWidth);
        methodConvert.Mod();
        methodConvert.Push(bitWidth);
        methodConvert.Swap();
        methodConvert.Sub();
        methodConvert.Push(bitWidth);
        methodConvert.Swap();
        methodConvert.Sub();
        methodConvert.Push(bitWidth - 1);
        methodConvert.And();
        methodConvert.ShR();
        methodConvert.Or();
        methodConvert.Dup();
        methodConvert.Push(BigInteger.One << (bitWidth - 1));
        JumpTarget endTarget = new();
        methodConvert.JumpIfLess(endTarget);
        methodConvert.Push(BigInteger.One << bitWidth);
        methodConvert.Sub();
        endTarget.Instruction = methodConvert.Nop();
    }

}
