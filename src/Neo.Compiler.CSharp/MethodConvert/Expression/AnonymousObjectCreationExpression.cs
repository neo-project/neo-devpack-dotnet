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
using Neo.VM;

namespace Neo.Compiler;

partial class MethodConvert
{

    /// <summary>
    /// Converts C# anonymous type creation expressions to executable code.
    /// </summary>
    /// <param name="model">The semantic model</param>
    /// <param name="expression">The anonymous object creation expression syntax node</param>
    /// <remarks>
    /// This method handles conversion of expressions such as:
    ///
    /// new { X = 10, Y = 20 }
    ///
    /// Where an anonymous type is created on the fly and initialized with
    /// member values.
    ///
    /// The equivalent named type syntax would be:
    ///
    /// var temp = new MyType { X = 10, Y = 20 };
    ///
    /// This method converts the anonymous type syntax so that the
    /// object creation and member assignments are emitted correctly to executable code.
    /// </remarks>
    private void ConvertAnonymousObjectCreationExpression(SemanticModel model, AnonymousObjectCreationExpressionSyntax expression)
    {
        AddInstruction(OpCode.NEWARRAY0);
        foreach (AnonymousObjectMemberDeclaratorSyntax initializer in expression.Initializers)
        {
            AddInstruction(OpCode.DUP);
            ConvertExpression(model, initializer.Expression);
            AddInstruction(OpCode.APPEND);
        }
    }
}
