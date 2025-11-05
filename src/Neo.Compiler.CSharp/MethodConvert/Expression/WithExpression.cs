// Copyright (C) 2015-2025 The Neo Project.
//
// WithExpression.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using System;
using System.Linq;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Convert record with expression: record with{...InitializerExpression}
    /// </summary>
    /// <param name="model"></param>
    /// <param name="expression"></param>
    private void ConvertWithExpressionSyntax(SemanticModel model, WithExpressionSyntax expression)
    {
        ITypeSymbol? type = model.GetTypeInfo(expression.Expression).Type;
        if (type is null)
            throw CompilationException.UnsupportedSyntax(expression, "Unable to resolve record type for with-expression.");

        // Load the original record instance/value onto the stack
        ConvertExpression(model, expression.Expression);

        if (!type.IsValueType)
        {
            var cloneMethod = type.GetMembers()
                .OfType<IMethodSymbol>()
                .FirstOrDefault(m => m.Name == "Clone" && m.Parameters.Length == 0 && !m.IsStatic)
                ?? type.BaseType?.GetMembers()
                    .OfType<IMethodSymbol>()
                    .FirstOrDefault(m => m.Name == "Clone" && m.Parameters.Length == 0 && !m.IsStatic);

            if (cloneMethod is not null)
            {
                CallInstanceMethod(model, cloneMethod, instanceOnStack: true, Array.Empty<ArgumentSyntax>());
            }
            else
            {
                // Fallback for sealed records that don't expose Clone. Behaves the
                // same as the previous implementation by duplicating the struct
                // representation.
                AddInstruction(new Instruction { OpCode = OpCode.UNPACK });
                AddInstruction(new Instruction { OpCode = OpCode.PACKSTRUCT });
            }
        }
        else
        {
            // Value-type records are copied by value.
            AddInstruction(new Instruction { OpCode = OpCode.UNPACK });
            AddInstruction(new Instruction { OpCode = OpCode.PACKSTRUCT });
        }

        // Apply the object initializer on the cloned value.
        ConvertObjectCreationExpressionInitializer(model, expression.Initializer);
    }
}
