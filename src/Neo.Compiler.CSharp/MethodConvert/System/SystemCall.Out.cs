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

partial class MethodConvert
{
    private static void HandleByteTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, byte.MinValue, byte.MaxValue);
    }

    private static void HandleSByteTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, sbyte.MinValue, sbyte.MaxValue);
    }

    private static void HandleShortTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, short.MinValue, short.MaxValue);
    }

    private static void HandleUShortTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, ushort.MinValue, ushort.MaxValue);
    }

    private static void HandleIntTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, int.MinValue, int.MaxValue);
    }

    private static void HandleUIntTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, uint.MinValue, uint.MaxValue);
    }

    private static void HandleLongTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, long.MinValue, long.MaxValue);
    }

    private static void HandleULongTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleNumericTryParseWithOut(methodConvert, model, symbol, arguments, ulong.MinValue, ulong.MaxValue);
    }

    private static void HandleNumericTryParseWithOut(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, IReadOnlyList<SyntaxNode>? arguments, BigInteger minValue, BigInteger maxValue)
    {
        if (arguments is null) return;
        methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (!methodConvert._context.TryGetCapturedStaticField(symbol.Parameters[1], out var index)) throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Out parameter must be captured in a static field.");

        // Drop the out parameter since it's not needed
        // We use the static field to store the result
        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.DROP);

        // Convert string to integer
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);

        // Check if the parsing was successful (not null)
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);

        JumpTarget failTarget = new();
        methodConvert.Jump(OpCode.JMPIF_L, failTarget);

        // If successful, check if the parsed value is within the valid range
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push(minValue);
        methodConvert.Push(maxValue + 1);
        methodConvert.AddInstruction(OpCode.WITHIN);
        methodConvert.Jump(OpCode.JMPIFNOT_L, failTarget);

        // If within range, store the value and push true
        methodConvert.AccessSlot(OpCode.STSFLD, index);
        methodConvert.Push(true);
        JumpTarget endTarget = new();
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        // Fail target: push false
        failTarget.Instruction = methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(false);

        // End target
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }

    private static void HandleBigIntegerTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is null) return;
        methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (!methodConvert._context.TryGetCapturedStaticField(symbol.Parameters[1], out var index)) throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Out parameter must be captured in a static field.");


        JumpTarget endTarget = new();

        // Convert string to BigInteger
        methodConvert.CallContractMethod(NativeContract.StdLib.Hash, "atoi", 1, true);

        // Check if the parsing was successful
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AddInstruction(OpCode.ISNULL);
        methodConvert.Jump(OpCode.JMPIF_L, endTarget);

        // If successful, store the value and push true
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.AccessSlot(OpCode.STSFLD, index);
        methodConvert.Push(true);
        methodConvert.Jump(OpCode.JMP_L, endTarget);

        // End target: clean up stack and push false if parsing failed
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
        methodConvert.AddInstruction(OpCode.DROP);
        methodConvert.Push(false);
    }

    private static void HandleBoolTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        if (arguments is null) return;
        methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (!methodConvert._context.TryGetCapturedStaticField(symbol.Parameters[1], out var index)) throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Out parameter must be captured in a static field.");

        JumpTarget trueTarget = new();
        JumpTarget falseTarget = new();
        JumpTarget endTarget = new();

        methodConvert.AddInstruction(OpCode.SWAP);
        methodConvert.AddInstruction(OpCode.DROP);

        // Check for true values
        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("true"); // x x "true"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("TRUE"); // x x "TRUE"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("True"); // x x "True"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("t"); // x x "t"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("T"); // x x "T"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget);

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("1"); // x x "1"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("yes"); // x x "yes"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("YES"); // x x "YES"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("y"); // x x "y"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("Y"); // x x "Y"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, trueTarget); // x

        // Check for false values
        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push("false"); // x x "false"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("FALSE"); // x x "FALSE"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("False"); // x x "False"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("f"); // x x "f"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push("F"); // x x "F"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("0"); // x x "0"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP);
        methodConvert.Push("no"); // x x "no"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("NO"); // x x "NO"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("n"); // x x "n"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        methodConvert.AddInstruction(OpCode.DUP); // x x
        methodConvert.Push("N"); // x x "N"
        methodConvert.AddInstruction(OpCode.EQUAL); // x
        methodConvert.Jump(OpCode.JMPIF_L, falseTarget); // x

        // If parsing failed, clean up stack and push false
        methodConvert.AddInstruction(OpCode.DROP); //
        methodConvert.Push(false); // false
        methodConvert.AccessSlot(OpCode.STSFLD, index); // false
        methodConvert.Push(false); // false
        methodConvert.Jump(OpCode.JMP_L, endTarget); // false

        // True case
        trueTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP); // x
        methodConvert.AddInstruction(OpCode.DROP); //
        methodConvert.Push(true); //  true
        methodConvert.AccessSlot(OpCode.STSFLD, index); // true
        methodConvert.Push(true); // true
        methodConvert.Jump(OpCode.JMP_L, endTarget); // true

        // False case
        falseTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP); // x
        methodConvert.AddInstruction(OpCode.DROP); //
        methodConvert.Push(false); // false
        methodConvert.AccessSlot(OpCode.STSFLD, index); // false
        methodConvert.Push(true); // true

        // End target
        endTarget.Instruction = methodConvert.AddInstruction(OpCode.NOP);
    }
}
