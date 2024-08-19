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
                        throw new CompilationException(subpattern, DiagnosticId.SyntaxNotSupported, $"Unsupported property or field: {subpattern.NameColon.Name}");
                    }
                    object? constantValue = model.GetConstantValue(constantPattern.Expression).Value;
                    Push(constantValue);
                    AddInstruction(OpCode.EQUAL);
                }
                else
                {
                    throw new CompilationException(subpattern, DiagnosticId.SyntaxNotSupported, $"Unsupported subpattern: {subpattern}");
                }
            }
        }
        else
        {
            throw new CompilationException(pattern, DiagnosticId.SyntaxNotSupported, $"Unsupported pattern: {pattern}");
        }
    }
}
