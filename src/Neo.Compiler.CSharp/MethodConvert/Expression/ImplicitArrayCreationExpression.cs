// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Neo.Compiler;

partial class MethodConvert
{

    /// <summary>
    /// Converts implicit array creation expressions to executable code.
    /// </summary>
    /// <param name="model">The semantic model</param>
    /// <param name="expression">The implicit array creation expression syntax</param>
    /// <remarks>
    /// Handles syntax like:
    ///
    /// var data = new[] { 1, 2, 3 }; // handles whole new[] {1, 2, 3}
    ///
    /// Where array instance is created and initialized in a single expression.
    /// Determines array type and converts the element initializer expressions.
    /// </remarks>
    private void ConvertImplicitArrayCreationExpression(SemanticModel model, ImplicitArrayCreationExpressionSyntax expression)
    {
        IArrayTypeSymbol type = (IArrayTypeSymbol)model.GetTypeInfo(expression).ConvertedType!;
        ConvertInitializerExpression(model, type, expression.Initializer);
    }
}
