// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private static void HandleNullableByteHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableByteValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableByteGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableSByteHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableSByteValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableSByteGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableShortHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableShortValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableShortGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableUShortHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableUShortValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableUShortGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableUIntHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableUIntValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableUIntGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableULongHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableULongValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableULongGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBoolHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableBoolValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBoolGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableCharHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableCharValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableCharGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBigIntegerHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableBigIntegerValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBigIntegerGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableIntHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableIntValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableIntGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableLongHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.AddInstruction(OpCode.NOT);
    }

    private static void HandleNullableLongValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.THROW);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableLongGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        var endTarget = new JumpTarget();
        methodConvert.Jump(OpCode.JMPIFNOT, endTarget);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(0);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {

        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        JumpTarget endTarget2 = new();
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF, endTarget);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "itoa", 1, true);
        methodConvert.Jump(OpCode.JMP_L, endTarget2);
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push("");
        endTarget2.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBoolToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        JumpTarget trueTarget = new(), nullTarget = new(), endTarget = new();
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget);
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);
        methodConvert.Push("False");
        methodConvert.Jump(OpCode.JMP_L, endTarget);
        trueTarget.Instruction = methodConvert.Push("True");
        methodConvert.Jump(OpCode.JMP_L, endTarget);
        nullTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push("");
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget1 = new(), nullTarget2 = new(), endTarget = new();

        methodConvert.AddInstruction(OpCode.DUP);// x y
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget1);

        // y is not null
        methodConvert.AddInstruction(OpCode.SWAP);// y x
        methodConvert.AddInstruction(OpCode.DUP); // y x x
        methodConvert.AddInstruction(OpCode.ISNULL); // y x
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget2);

        // y and x both are not null
        methodConvert.AddInstruction(OpCode.EQUAL);
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        // y is null, then return true if x is null, false otherwise
        nullTarget1.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);// x
        methodConvert.AddInstruction(OpCode.ISNULL); // true is null, otherwise false
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        nullTarget2.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP); // drop x
        methodConvert.AddInstruction(OpCode.DROP); // drop y
        methodConvert.Push(false); // y not null but x is null

        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBigIntegerEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget1 = new(), nullTarget2 = new(), endTarget = new();

        methodConvert.AddInstruction(OpCode.DUP);// x y
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget1);

        // y is not null
        methodConvert.AddInstruction(OpCode.SWAP);// y x
        methodConvert.AddInstruction(OpCode.DUP); // y x x
        methodConvert.AddInstruction(OpCode.ISNULL); // y x
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget2);

        // y and x both are not null
        methodConvert.AddInstruction(OpCode.NUMEQUAL);
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        // y is null, then return true if x is null, false otherwise
        nullTarget1.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);// x
        methodConvert.AddInstruction(OpCode.ISNULL); // true is null, otherwise false
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        nullTarget2.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP); // drop x
        methodConvert.AddInstruction(OpCode.DROP); // drop y
        methodConvert.Push(false); // y not null but x is null

        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBoolEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget1 = new(), nullTarget2 = new(), endTarget = new();

        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget1);

        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget2);

        methodConvert.AddInstruction(OpCode.EQUAL);
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        nullTarget1.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        nullTarget2.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(false);

        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBigIntegerEqualsWithNonNullable(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget = new(), endTarget = new();

        methodConvert.AddInstruction(OpCode.DUP);// x y
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget);

        // y is not null
        methodConvert.AddInstruction(OpCode.NUMEQUAL);
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        // y is null, then return false
        nullTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP); // drop x
        methodConvert.Push(false); // y is null

        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleNullableBoolEqualsWithNonNullable(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget = new(), endTarget = new();

        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF_L, nullTarget);

        methodConvert.AddInstruction(OpCode.EQUAL);
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        nullTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(false);

        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }
}
