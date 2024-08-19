// Copyright (C) 2015-2024 The Neo Project.
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

internal partial class MethodConvert
{
    /// <summary>
    /// Converts the code for assignment expression into OpCodes.
    /// Include assignment operator (=), null-coalescing assignment operator (??=) and compound assignment(+=, -=, *=, /= ... )
    /// </summary>
    /// <param name="model">The semantic model providing context and information about assignment expression.</param>
    /// <param name="expression">The syntax representation of the assignment expression statement being converted.</param>
    /// <example>
    /// The following code covers three branches. If you want to see the example code for only one of the branches,
    /// you can look at the comments of the corresponding method.
    /// <code>
    /// public class Cat
    /// {
    ///     public string Name { get; set; }
    /// }
    /// </code>
    /// <code>
    /// Cat nullableCat = null;
    /// Cat nonNullableCat = new() { Name = "Mimi" };
    /// nullableCat ??= nonNullableCat;
    /// var logInfo = "Nullable cat: ";
    /// logInfo += nullableCat.Name;
    /// Runtime.Log(log);
    /// </code>
    /// </example>
    /// <seealso cref="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/assignment-operator"/>
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
