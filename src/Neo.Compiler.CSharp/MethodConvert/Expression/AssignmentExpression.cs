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
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Neo.Compiler;

partial class MethodConvert
{
    /// <summary>
    /// Converts an assignment expression into executable instructions based on the type of assignment.
    /// </summary>
    /// <param name="model">The semantic model for resolving types and symbols within the current context.</param>
    /// <param name="expression">The assignment expression syntax node to be converted.</param>
    /// <remarks>
    /// This method handles different types of assignment expressions by switching on the operator token:
    /// - For simple assignments using "=".
    /// - For null-coalescing assignments using "??=".
    /// - For all other types of assignments, such as compound assignments (e.g., +=, -=).
    /// </remarks>
    private void ConvertAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
    {
        switch (expression.OperatorToken.ValueText)
        {
            case "=":
                // Example: variable = value
                ConvertSimpleAssignmentExpression(model, expression);
                break;
            case "??=":
                // Example: variable ??= value
                ConvertCoalesceAssignmentExpression(model, expression);
                break;
            default:
                // Example: variable += value
                ConvertComplexAssignmentExpression(model, expression);
                break;
        }
    }
}
