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
    private static void HandleSByteParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        sb.Atoi(methodConvert);
        sb.IsSByteCheck();
    }

    // HandleSByteLeadingZeroCount
    private static void HandleSByteLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleLeadingZeroCount<sbyte>(methodConvert, model, symbol, instanceExpression, arguments, sizeof(sbyte) * 8, true);
    }

    // HandleSByteCopySign
    private static void HandleSByteCopySign(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleCopySign<sbyte>(methodConvert, model, symbol, instanceExpression, arguments, sb => sb.IsSByteCheck());
    }

    // HandleSByteCreateChecked
    private static void HandleSByteCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        JumpTarget endTarget = new();
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsSByteCheck();
    }

    // HandleSByteCreateSaturating
    private static void HandleSByteCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleCreateSaturating(methodConvert, model, symbol, instanceExpression, arguments, sbyte.MinValue, sbyte.MaxValue);
    }

    // HandleSByteRotateLeft
    private static void HandleSByteRotateLeft(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleSignedRotateLeft<sbyte>(methodConvert, model, symbol, instanceExpression, arguments, sizeof(sbyte) * 8);
    }

    // HandleSByteRotateRight
    private static void HandleSByteRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleSignedRotateRight<sbyte>(methodConvert, model, symbol, instanceExpression, arguments, sizeof(sbyte) * 8);
    }
}
