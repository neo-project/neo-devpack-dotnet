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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// The checked and unchecked statements specify the overflow-checking context for integral-type arithmetic operations and conversions.
    /// When integer arithmetic overflow occurs, the overflow-checking context defines what happens.
    /// In a checked context, a System.OverflowException is thrown;
    /// if overflow happens in a constant expression, a compile-time error occurs.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about checked and unchecked statement.</param>
    /// <param name="expression">The syntax representation of the checked and unchecked statement being converted.</param>
    /// <example>
    /// Use the checked keyword to qualify the result of the temp*2 calculation and use a try catch to handle the overflow if it occurs.
    /// <code>
    /// try
    /// {
    ///     int temp = int.MaxValue;
    ///     int a = checked(temp * 2);
    /// }
    /// catch (OverflowException)
    /// {
    ///     Runtime.Log("Overflow");
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    /// This code is not called when the checked keyword modifies a block of statements, for example.
    /// <code>
    /// checked
    /// {
    ///     int a = temp * 2;
    /// }
    /// </code>
    /// For a checked statement, see <see cref="ConvertCheckedStatement(SemanticModel, CheckedStatementSyntax)"/>
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/zh-cn/dotnet/csharp/language-reference/operators/arithmetic-operators#integer-arithmetic-overflow">Integer arithmetic overflow</seealso>
    private void ConvertCheckedExpression(SemanticModel model, CheckedExpressionSyntax expression)
    {
        _checkedStack.Push(expression.Keyword.IsKind(SyntaxKind.CheckedKeyword));
        ConvertExpression(model, expression.Expression);
        _checkedStack.Pop();
    }
}
