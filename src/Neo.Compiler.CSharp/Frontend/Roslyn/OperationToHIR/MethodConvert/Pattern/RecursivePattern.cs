// Copyright (C) 2015-2025 The Neo Project.
//
// RecursivePattern.cs file belongs to the neo project and is free
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

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private void ConvertRecursivePattern(SemanticModel model, RecursivePatternSyntax pattern, byte localIndex)
    {
        if (pattern.PropertyPatternClause is { } propertyClause)
        {
            AccessSlot(OpCode.LDLOC, localIndex);
            foreach (var subpattern in propertyClause.Subpatterns)
            {
                if (subpattern is { Pattern: ConstantPatternSyntax constantPattern })
                {
                    // Example:
                    // if (newOwner is { IsValid: true, IsZero:false})
                    // {
                    // }
                    var propertySymbol = model.GetSymbolInfo(subpattern.NameColon!.Name).Symbol!;

                    if (propertySymbol is IPropertySymbol property)
                    {
                        CallMethodWithConvention(model, property.GetMethod!);
                    }
                    else
                    {
                        throw CompilationException.UnsupportedSyntax(subpattern, $"Recursive pattern can only match properties, not '{propertySymbol.GetType().Name}'. Ensure '{subpattern.NameColon.Name.ToString()}' is a property with a getter.");
                    }
                    object? constantValue = model.GetConstantValue(constantPattern.Expression).Value;
                    Push(constantValue);
                    AddInstruction(OpCode.EQUAL);
                }
                else
                {
                    throw CompilationException.UnsupportedSyntax(subpattern, $"Recursive patterns currently only support constant pattern matching. Found: {subpattern.Pattern?.GetType().Name ?? "null"}. Use syntax like '{{ PropertyName: constantValue }}'.");
                }
            }
        }
        else
        {
            throw CompilationException.UnsupportedSyntax(pattern, "Recursive patterns must include a property pattern clause. Use syntax like 'Type { Property: value }' or '{ Property: value }'.");
        }
    }
}
