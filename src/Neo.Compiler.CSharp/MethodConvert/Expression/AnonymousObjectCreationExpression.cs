// Copyright (C) 2015-2025 The Neo Project.
//
// AnonymousObjectCreationExpression.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
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
    /// Methods for converting the creation of an object of an anonymous type into a series of instructions.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about the anonymous object creation.</param>
    /// <param name="expression">The syntax representation of the  anonymous object creation statement being converted.</param>
    /// <remarks>
    /// Anonymous types provide a convenient way to encapsulate a set of read-only properties into a single object without having to explicitly define a type first.
    /// The type name is generated by the compiler and is not available at the source code level.
    /// The type of each property is inferred by the compiler.
    /// </remarks>
    /// <example>
    /// The following example shows an anonymous type that is initialized with two properties named Amount and Message.
    /// <c>var v = new { Amount = 108, Message = "Hello" };</c>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types">Anonymous types</seealso>
    private void ConvertAnonymousObjectCreationExpression(SemanticModel model, AnonymousObjectCreationExpressionSyntax expression)
    {
        foreach (AnonymousObjectMemberDeclaratorSyntax initializer in expression.Initializers.Reverse())  // PACK works in a reversed way
            ConvertExpression(model, initializer.Expression);
        Push(expression.Initializers.Count);
        AddInstruction(OpCode.PACK);
    }
}
