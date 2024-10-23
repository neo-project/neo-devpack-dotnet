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
using System.Linq;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Converts the code for tuple type into OpCodes.
    /// Tuple types expressions are a new feature introduced in C# 7.0(Released March, 2017).
    /// </summary>
    /// <param name="model">The semantic model providing context and information about the tuple type.</param>
    /// <param name="expression">The syntax representation of the tuple type statement being converted.</param>
    /// <example>
    /// The tuples feature provides concise syntax to group multiple data elements in a lightweight data structure.
    /// The following example shows how you can declare a tuple variable, initialize it, and access its data members:
    /// <code>
    /// (string, int) t1 = ("chris", 3);
    /// Runtime.Log($"Tuple with elements {t1.Item1} and {t1.Item2}.");
    /// (string Name, int Count) t2 = ("chris", 3);
    /// Runtime.Log($"Sum of {t2.Name} elements is {t2.Count}.");
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-tuples">Tuple types</seealso>
    private void ConvertTupleExpression(SemanticModel model, TupleExpressionSyntax expression)
    {
        foreach (ArgumentSyntax argument in expression.Arguments.Reverse())  // PACKSTRUCT works in a reversed way
            ConvertExpression(model, argument.Expression);
        Push(expression.Arguments.Count);
        AddInstruction(OpCode.PACKSTRUCT);
    }
}
