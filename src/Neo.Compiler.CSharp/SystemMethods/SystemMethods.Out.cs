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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Neo.Compiler;

internal static partial class SystemMethods
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
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is null) return;
        methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (!methodConvert.Context.TryGetCapturedStaticField(symbol.Parameters[1], out var index)) throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Out parameter must be captured in a static field.");

        // Drop the out parameter since it's not needed
        // We use the static field to store the result
        sb.Swap();
        sb.Drop();

        // Convert string to integer
        sb.Atoi(methodConvert);

        // Check if the parsing was successful (not null)
        sb.Dup();
        sb.IsNull();

        JumpTarget failTarget = new();
        sb.JmpIfL(failTarget);

        // If successful, check if the parsed value is within the valid range
        sb.Dup();
        sb.Within(minValue, maxValue);
        sb.JmpIfNotL(failTarget);

        // If within range, store the value and push true
        sb.StSFld(index);
        sb.Push(true);
        JumpTarget endTarget = new();
        sb.JmpL(endTarget);

        // Fail target: push false
        failTarget.Instruction = sb.Drop();
        sb.Push(false);

        // End target
        sb.AddTarget(endTarget);
    }

    private static void HandleBigIntegerTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is null) return;
        methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (!methodConvert.Context.TryGetCapturedStaticField(symbol.Parameters[1], out var index)) throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Out parameter must be captured in a static field.");
        JumpTarget endTarget = new();

        // Convert string to BigInteger
        sb.Atoi(methodConvert);

        // Check if the parsing was successful
        sb.Dup();
        sb.IsNull();
        sb.JmpIfL(endTarget);

        // If successful, store the value and push true
        sb.Dup();
        sb.StSFld(index);
        sb.Push(true);
        sb.JmpL(endTarget);

        // End target: clean up stack and push false if parsing failed
        sb.AddTarget(endTarget);
        sb.Drop();
        sb.Push(false);
    }

    private static void HandleBoolTryParseWithOut(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is null) return;
        methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        if (!methodConvert.Context.TryGetCapturedStaticField(symbol.Parameters[1], out var index)) throw new CompilationException(symbol, DiagnosticId.SyntaxNotSupported, "Out parameter must be captured in a static field.");

        JumpTarget trueTarget = new();
        JumpTarget falseTarget = new();
        JumpTarget endTarget = new();

        sb.Swap();
        sb.Drop();

        // Check for true values
        sb.Dup(); // x x
        sb.Equal("true"); // x
        sb.JmpIfL(trueTarget);

        sb.Dup(); // x x
        sb.Equal("TRUE"); // x
        sb.JmpIfL(trueTarget);

        sb.Dup(); // x x
        sb.Equal("True"); // x
        sb.JmpIfL(trueTarget);

        sb.Dup(); // x x
        sb.Equal("t"); // x
        sb.JmpIfL(trueTarget);

        sb.Dup(); // x x
        sb.Equal("T"); // x
        sb.JmpIfL(trueTarget);

        sb.Dup(); // x x
        sb.Equal("1"); // x
        sb.JmpIfL(trueTarget); // x

        sb.Dup(); // x x
        sb.Equal("yes"); // x
        sb.JmpIfL(trueTarget); // x

        sb.Dup(); // x x
        sb.Equal("YES"); // x
        sb.JmpIfL(trueTarget); // x

        sb.Dup(); // x x
        sb.Equal("y"); // x
        sb.JmpIfL(trueTarget); // x

        sb.Dup(); // x x
        sb.Equal("Y"); // x
        sb.JmpIfL(trueTarget); // x

        // Check for false values
        sb.Dup();
        sb.Equal("false"); // x
        sb.JmpIfL(falseTarget); // x

        sb.Dup(); // x x
        sb.Equal("FALSE"); // x
        sb.JmpIfL(falseTarget); // x

        sb.Dup(); // x x
        sb.Equal("False"); // x
        sb.JmpIfL(falseTarget); // x

        sb.Dup(); // x x
        sb.Equal("f"); // x
        sb.JmpIfL(falseTarget); // x

        sb.Dup(); // x x
        sb.Equal("F"); // x
        sb.JmpIfL(falseTarget); // x

        sb.Dup(); // x x
        sb.Equal("0"); // x
        sb.JmpIfL(falseTarget); // x

        sb.Dup();
        sb.Equal("no"); // x
        sb.JmpIfL(falseTarget); // x

        sb.Dup(); // x x
        sb.Equal("NO"); // x
        sb.JmpIfL(falseTarget); // x

        sb.Dup(); // x x
        sb.Equal("n"); // x
        sb.JmpIfL(falseTarget); // x

        sb.Dup(); // x x
        sb.Equal("N"); // x
        sb.JmpIfL(falseTarget); // x

        // If parsing failed, clean up stack and push false
        sb.Drop(); //
        sb.Push(false); // false
        sb.StSFld(index); // false
        sb.Push(false); // false
        sb.JmpL(endTarget); // false

        // True case
        trueTarget.Instruction = sb.Nop(); // x
        sb.Drop(); //
        sb.Push(true); //  true
        sb.StSFld(index); // true
        sb.Push(true); // true
        sb.JmpL(endTarget); // true

        // False case
        falseTarget.Instruction = sb.Nop(); // x
        sb.Drop(); //
        sb.Push(false); // false
        sb.StSFld(index); // false
        sb.Push(true); // true

        // End target
        sb.AddTarget(endTarget);
    }
}
