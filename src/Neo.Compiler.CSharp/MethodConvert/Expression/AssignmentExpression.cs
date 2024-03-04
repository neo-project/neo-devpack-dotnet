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
    /// The assignment operator = assigns the value of its right-hand operand to a variable,
    /// a property, or an indexer element given by its left-hand operand.
    /// The result of an assignment expression is the value assigned to the left-hand operand.
    /// The type of the right-hand operand must be the same as the type of the left-hand operand or implicitly convertible to it.
    /// The null-coalescing assignment operator ??= assigns the value of its right-hand operand to its left-hand operand only if the left-hand operand evaluates to null.
    /// The ??= operator doesn't evaluate its right-hand operand if the left-hand operand evaluates to non-null.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about assignment expression.</param>
    /// <param name="expression">The syntax representation of the assignment expression statement being converted.</param>
    /// <example>
    /// The assignment operator = is right-associative, that is, an expression of the form
    /// <code>
    /// a = b = c
    /// </code>
    /// is evaluated as
    /// <code>
    /// a = (b = c)
    /// </code>
    /// </example>
    private void ConvertAssignmentExpression(SemanticModel model, AssignmentExpressionSyntax expression)
    {
        switch (expression.OperatorToken.ValueText)
        {
            case "=":
                ConvertSimpleAssignmentExpression(model, expression);
                break;
            case "??=":
                ConvertCoalesceAssignmentExpression(model, expression);
                break;
            default:
                ConvertComplexAssignmentExpression(model, expression);
                break;
        }
    }
}
