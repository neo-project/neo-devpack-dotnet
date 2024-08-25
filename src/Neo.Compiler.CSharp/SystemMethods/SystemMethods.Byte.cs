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
    private static void HandleByteParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        sb.Atoi(methodConvert);
        sb.IsByteCheck();
    }

    //HandleByteLeadingZeroCount
    private static void HandleByteLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleLeadingZeroCount<byte>(methodConvert, model, symbol, instanceExpression, arguments, sizeof(byte) * 8, false);
    }

    // HandleByteCreateChecked
    private static void HandleByteCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsByteCheck();
    }

    // HandleByteCreateSaturating
    private static void HandleByteCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleCreateSaturating(methodConvert, model, symbol, instanceExpression, arguments, byte.MinValue, byte.MaxValue);
    }

    // HandleByteRotateLeft
    private static void HandleByteRotateLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleUnsignedRotateLeft<byte>(methodConvert, model, symbol, instanceExpression, arguments, sizeof(byte) * 8);
    }

    // HandleByteRotateRight
    private static void HandleByteRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleUnsignedRotateRight<byte>(methodConvert, model, symbol, instanceExpression, arguments, sizeof(byte) * 8);
    }
}
