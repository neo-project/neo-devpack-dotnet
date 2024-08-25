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
    private static void HandleULongParse(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        JumpTarget endTarget = new();
        sb.Atoi(methodConvert);
        sb.IsULongCheck();
    }

    // HandleULongLeadingZeroCount
    private static void HandleULongLeadingZeroCount(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol,
        ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleLeadingZeroCount<ulong>(methodConvert, model, symbol, instanceExpression, arguments, sizeof(ulong) * 8, false);
    }

    // HandleULongCreateChecked
    private static void HandleULongCreateChecked(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        var sb = methodConvert.InstructionsBuilder;
        JumpTarget endTarget = new();
        if (arguments is not null)
            methodConvert.PrepareArgumentsForMethod(model, symbol, arguments);
        sb.IsULongCheck();
    }

    // HandleULongCreateSaturating
    private static void HandleULongCreateSaturating(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleCreateSaturating(methodConvert, model, symbol, instanceExpression, arguments, ulong.MinValue, ulong.MaxValue);
    }

    /// <summary>
    /// Handles the ULong.RotateLeft operation by converting it to the appropriate VM instructions.
    /// </summary>
    /// <param name="methodConvert">The MethodConvert instance to add instructions to.</param>
    /// <param name="model">The semantic model of the code being converted.</param>
    /// <param name="symbol">The method symbol representing the RotateLeft operation.</param>
    /// <param name="instanceExpression">The instance expression, if any (null for static methods).</param>
    /// <param name="arguments">The list of arguments passed to the method.</param>
    private static void HandleULongRotateLeft(MethodConvert methodConvert, SemanticModel model,
        IMethodSymbol symbol, ExpressionSyntax? instanceExpression,
        IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleUnsignedRotateLeft<ulong>(methodConvert, model, symbol, instanceExpression, arguments, sizeof(ulong) * 8);
    }

    // HandleULongRotateRight
    /// <summary>
    /// Handles the ULong.RotateRight operation by converting it to the appropriate VM instructions.
    /// </summary>
    /// <param name="methodConvert">The MethodConvert instance to add instructions to.</param>
    /// <param name="model">The semantic model of the code being converted.</param>
    /// <param name="symbol">The method symbol representing the RotateRight operation.</param>
    /// <param name="instanceExpression">The instance expression, if any (null for static methods).</param>
    /// <param name="arguments">The list of arguments passed to the method.</param>
    /// <remark>This method implements the rotation using bitwise operations.</remark>
    private static void HandleULongRotateRight(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        HandleUnsignedRotateRight<ulong>(methodConvert, model, symbol, instanceExpression, arguments, sizeof(ulong) * 8);
    }
}
