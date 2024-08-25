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
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Neo.Compiler;

internal static partial class SystemMethods
{
    private static void HandleCharParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        sb.Atoi(methodConvert);
        sb.IsUShortCheck();
    }

    // Handler for equality methods (Equals)
    private static void HandleEquals(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.NumEqual();
    }

    // Handler for Array.Length and string.Length properties
    private static void HandleLength(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (instanceExpression is not null)
            methodConvert.ConvertExpression(model, instanceExpression);
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Size();
    }

    private static void HandleCharIsDigit(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsDigitChar();
    }

    private static void HandleCharIsLetter(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Dup();
        sb.IsUpperChar();
        sb.Swap();
        sb.IsLowerChar();
        sb.BoolOr();
    }

    private static void HandleCharIsWhiteSpace(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Dup();
        sb.Within((ushort)'\t', (ushort)'\r');
        sb.Swap();
        sb.Within((ushort)' ', (ushort)' ');
        sb.BoolOr();
    }

    private static void HandleCharIsLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsLowerChar();
    }

    private static void HandleCharIsUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsUpperChar();
    }

    private static void HandleCharIsPunctuation(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        var endTarget = new JumpTarget();
        sb.Dup();
        sb.Within((ushort)'!', (ushort)'/');
        sb.JmpIf(endTarget);
        sb.Dup();
        sb.Within((ushort)':', (ushort)'@');
        sb.JmpIf(endTarget);
        sb.Dup();
        sb.Within((ushort)'[', (ushort)'`');
        sb.JmpIf(endTarget);
        sb.Within((ushort)'{', (ushort)'~');
        sb.AddTarget(endTarget);
    }

    private static void HandleCharIsSymbol(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        var endTarget = new JumpTarget();
        sb.Dup();
        sb.Within((ushort)'$', (ushort)'+');
        sb.JmpIf(endTarget);
        sb.Dup();
        sb.Within((ushort)'<', (ushort)'=');
        sb.JmpIf(endTarget);
        sb.Dup();
        sb.Within((ushort)'>', (ushort)'@');
        sb.JmpIf(endTarget);
        sb.Dup();
        sb.Within((ushort)'[', (ushort)'`');
        sb.JmpIf(endTarget);
        sb.Within((ushort)'{', (ushort)'~');
        sb.AddTarget(endTarget);
    }

    private static void HandleCharIsControl(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Dup();
        sb.Within((ushort)'\0', (ushort)'\x1F');
        sb.Swap();
        sb.Within((ushort)'\x7F', (ushort)'\x9F');
        sb.BoolOr();
    }

    private static void HandleCharIsSurrogate(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Dup();
        sb.Within((ushort)0xD800, (ushort)0xDBFF);
        sb.Swap();
        sb.Within((ushort)0xDC00, (ushort)0xDFFF);
        sb.BoolOr();
    }

    private static void HandleCharIsHighSurrogate(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Within((ushort)0xD800, (ushort)0xDBFF);
    }

    private static void HandleCharIsLowSurrogate(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Within((ushort)0xDC00, (ushort)0xDFFF);
    }

    private static void HandleCharIsLetterOrDigit(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        sb.Dup();
        sb.IsDigitChar();
        sb.JmpIf(endTarget);
        sb.Dup();
        sb.IsUpperChar();
        sb.JmpIf(endTarget);
        sb.IsLowerChar();
        sb.AddTarget(endTarget);
    }

    private static void HandleCharIsBetween(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget validTarget = new();
        JumpTarget endTarget = new();
        sb.Dup();
        sb.Rot();
        sb.Ge();
        sb.Dup();
        sb.JmpIfNot(validTarget);
        sb.Reverse3();
        sb.Drop();
        sb.Drop();
        sb.Jmp(endTarget);
        sb.AddTarget(validTarget);
        sb.Drop();
        sb.Lt();
        sb.AddTarget(endTarget);
    }

    private static void HandleCharGetNumericValue(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        JumpTarget validTarget = new();
        sb.Dup();
        sb.IsDigitChar();
        sb.JmpIf(validTarget);
        sb.Drop();
        sb.PushM1();
        sb.Jmp(endTarget);
        sb.AddTarget(validTarget);
        sb.Push((ushort)'0');
        sb.Sub();
        sb.AddTarget(endTarget);
    }

    private static void HandleCharToLower(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Dup();
        sb.IsUpperChar();
        var endTarget = new JumpTarget();
        sb.JmpIfNot(endTarget);
        sb.Push((ushort)'A');
        sb.Sub();
        sb.Push((ushort)'a');
        sb.Add();
        sb.AddTarget(endTarget);
    }

    private static void HandleCharToUpper(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Dup();
        sb.IsLowerChar();
        var endTarget = new JumpTarget();
        sb.JmpIfNot(endTarget);
        sb.Push((ushort)'a');
        sb.Sub();
        sb.Push((ushort)'A');
        sb.Add();
        sb.AddTarget(endTarget);
    }

    private static void HandleCharToLowerInvariant(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        sb.Dup();
        sb.IsLowerChar();
    }

    private static void HandleCharToUpperInvariant(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        sb.Dup();
        sb.IsUpperChar();
    }

    private static void HandleCharIsAscii(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.Push(128);
        sb.Lt();
    }

    private static void HandleCharIsAsciiDigit(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsDigitChar();
    }

    private static void HandleCharIsAsciiLetter(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsUpperChar();
        var endTarget = new JumpTarget();
        sb.JmpIf(endTarget);
        sb.IsLowerChar();
        sb.AddTarget(endTarget);
    }
}
