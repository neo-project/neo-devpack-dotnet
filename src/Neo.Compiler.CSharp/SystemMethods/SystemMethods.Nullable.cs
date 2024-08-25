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

internal static partial class SystemMethods
{
    private static void HandleNullableByteHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.IsNull();
        sb.Not();
    }

    private static void HandleNullableByteValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Throw();
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableByteGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Drop();
        sb.Push0();
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableSByteHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.IsNull();
        sb.Not();
    }

    private static void HandleNullableSByteValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Throw();
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableSByteGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Drop();
        sb.Push0();
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableShortHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.IsNull();
        sb.Not();
    }

    private static void HandleNullableShortValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Throw();
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableShortGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Drop();
        sb.Push0();
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableUShortHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.IsNull();
        sb.Not();
    }

    private static void HandleNullableUShortValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Throw();
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableUShortGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Drop();
        sb.Push(0);
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableUIntHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.IsNull();
        sb.Not();
    }

    private static void HandleNullableUIntValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Throw();
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableUIntGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Drop();
        sb.Push(0);
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableULongHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.IsNull();
        sb.Not();
    }

    private static void HandleNullableULongValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Throw();
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableULongGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Drop();
        sb.Push(0);
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableBoolHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.IsNull();
        sb.Not();
    }

    private static void HandleNullableBoolValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Throw();
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableBoolGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Drop();
        sb.Push(0);
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableCharHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.IsNull();
        sb.Not();
    }

    private static void HandleNullableCharValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Throw();
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableCharGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Drop();
        sb.Push(0);
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableBigIntegerHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.IsNull();
        sb.Not();
    }

    private static void HandleNullableBigIntegerValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Throw();
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableBigIntegerGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Drop();
        sb.Push(0);
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableIntHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.IsNull();
        sb.Not();
    }

    private static void HandleNullableIntValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Throw();
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableIntGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Drop();
        sb.Push(0);
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableLongHasValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.IsNull();
        sb.Not();
    }

    private static void HandleNullableLongValue(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Throw();
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableLongGetValueOrDefault(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        var endTarget = new JumpTarget();
        sb.Jump(OpCode.JMPIFNOT, endTarget);
        sb.Drop();
        sb.Push(0);
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        JumpTarget endTarget2 = new();
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        sb.Dup();
        sb.IsNull();
        sb.Jump(OpCode.JMPIF, endTarget);
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "itoa", 1, true);
        sb.JmpL(endTarget2);
        endTarget.Instruction = sb.Nop();
        sb.Drop();
        sb.Push("");
        endTarget2.Instruction = sb.Nop();
    }

    private static void HandleNullableBoolToString(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        JumpTarget trueTarget = new(), nullTarget = new(), endTarget = new();
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Dup();
        sb.IsNull();
        sb.Jump(OpCode.JMPIF_L, nullTarget);
        sb.Jump(OpCode.JMPIF_L, trueTarget);
        sb.Push("False");
        sb.JmpL(endTarget);
        trueTarget.Instruction = sb.Push("True");
        sb.JmpL(endTarget);
        nullTarget.Instruction = sb.Nop();
        sb.Drop();
        sb.Push("");
        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget1 = new(), nullTarget2 = new(), endTarget = new();

        sb.Dup();// x y
        sb.IsNull();
        sb.JmpIfL(nullTarget1);

        // y is not null
        sb.Swap();// y x
        sb.Dup(); // y x x
        sb.IsNull(); // y x
        sb.JmpIfL(nullTarget2);

        // y and x both are not null
        sb.Equal();
        sb.JmpL(endTarget);

        // y is null, then return true if x is null, false otherwise
        nullTarget1.Instruction = sb.Nop();
        sb.Drop();// x
        sb.IsNull(); // true is null, otherwise false
        sb.JmpL(endTarget);

        nullTarget2.Instruction = sb.Nop();
        sb.Drop(); // drop x
        sb.Drop(); // drop y
        sb.Push(false); // y not null but x is null

        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableBigIntegerEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget1 = new(), nullTarget2 = new(), endTarget = new();

        sb.Dup();// x y
        sb.IsNull();
        sb.JmpIfL(nullTarget1);

        // y is not null
        sb.Swap();// y x
        sb.Dup(); // y x x
        sb.IsNull(); // y x
        sb.JmpIfL(nullTarget2);

        // y and x both are not null
        sb.NumEqual();
        sb.JmpL(endTarget);

        // y is null, then return true if x is null, false otherwise
        nullTarget1.Instruction = sb.Nop();
        sb.Drop();// x
        sb.IsNull(); // true is null, otherwise false
        sb.JmpL(endTarget);

        nullTarget2.Instruction = sb.Nop();
        sb.Drop(); // drop x
        sb.Drop(); // drop y
        sb.Push(false); // y not null but x is null

        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableBoolEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget1 = new(), nullTarget2 = new(), endTarget = new();

        sb.Dup();
        sb.IsNull();
        sb.JmpIfL(nullTarget1);

        sb.Dup();
        sb.IsNull();
        sb.JmpIfL(nullTarget2);

        sb.Equal();
        sb.JmpL(endTarget);

        nullTarget1.Instruction = sb.Nop();
        sb.Drop();
        sb.IsNull();
        sb.JmpL(endTarget);

        nullTarget2.Instruction = sb.Nop();
        sb.Drop();
        sb.Push(false);

        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableBigIntegerEqualsWithNonNullable(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget = new(), endTarget = new();

        sb.Dup();// x y
        sb.IsNull();
        sb.JmpIfL(nullTarget);

        // y is not null
        sb.NumEqual();
        sb.JmpL(endTarget);

        // y is null, then return false
        nullTarget.Instruction = sb.Nop();
        sb.Drop(); // drop x
        sb.Push(false); // y is null

        endTarget.Instruction = sb.Nop();
    }

    private static void HandleNullableBoolEqualsWithNonNullable(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);

        JumpTarget nullTarget = new(), endTarget = new();

        sb.Dup();
        sb.IsNull();
        sb.JmpIfL(nullTarget);

        sb.Equal();
        sb.JmpL(endTarget);

        nullTarget.Instruction = sb.Nop();
        sb.Drop();
        sb.Push(false);

        endTarget.Instruction = sb.Nop();
    }
}
