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
    private static void HandleUShortParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        sb.Atoi(methodConvert);
        sb.IsUShortCheck();
    }

    // HandleUShortLeadingZeroCount
    private static void HandleUShortLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleLeadingZeroCount<ushort>(methodConvert, model, symbol, instanceExpression, arguments, sizeof(ushort) * 8, false);
    }

    // HandleUShortCreateChecked
    private static void HandleUShortCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        JumpTarget endTarget = new();
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsUShortCheck();
    }

    // HandleUShortCreateSaturating
    private static void HandleUShortCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleCreateSaturating(methodConvert, model, symbol, instanceExpression, arguments, ushort.MinValue, ushort.MaxValue);
    }

    // implement HandleUShortRotateLeft
    private static void HandleUShortRotateLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleUnsignedRotateLeft<ushort>(methodConvert, model, symbol, instanceExpression, arguments, sizeof(ushort) * 8);
    }

    // HandleUShortRotateRight
    private static void HandleUShortRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleUnsignedRotateRight<ushort>(methodConvert, model, symbol, instanceExpression, arguments, sizeof(ushort) * 8);
    }
}
