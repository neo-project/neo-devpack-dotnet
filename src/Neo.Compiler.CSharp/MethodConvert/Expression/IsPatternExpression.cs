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
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// This method converts an 'is' pattern expression to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about is pattern expression.</param>
    /// <param name="expression">The syntax representation of the is pattern expression statement being converted.</param>
    /// <example>
    /// In this example, the 'is' pattern expression is used to check if obj is an instance of the string type.
    /// <code>
    /// object obj = "Hello";
    /// if (obj is string str)
    /// {
    ///     Runtime.Log($"The object is a string: {str}");
    /// }
    /// else
    /// {
    ///     Runtime.Log("The object is not a string.");
    /// }
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/type-testing-and-cast#is-operator">is operator</seealso>
    private void ConvertIsPatternExpression(SemanticModel model, IsPatternExpressionSyntax expression)
    {
        byte anonymousIndex = AddAnonymousVariable();
        ConvertExpression(model, expression.Expression);
        AccessSlot(OpCode.STLOC, anonymousIndex);
        ConvertPattern(model, expression.Pattern, anonymousIndex);
        RemoveAnonymousVariable(anonymousIndex);
    }
}
