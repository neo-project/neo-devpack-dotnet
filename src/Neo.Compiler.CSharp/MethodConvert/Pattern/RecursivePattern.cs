// Copyright (C) 2015-2026 The Neo Project.
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
        if (pattern.PropertyPatternClause is not { } propertyClause)
        {
            throw CompilationException.UnsupportedSyntax(pattern, "Recursive patterns must include a property pattern clause. Use syntax like 'Type { Property: value }' or '{ Property: value }'.");
        }

        bool hasSubpattern = false;
        foreach (var subpattern in propertyClause.Subpatterns)
        {
            if (subpattern is not { Pattern: ConstantPatternSyntax constantPattern })
            {
                throw CompilationException.UnsupportedSyntax(subpattern, $"Recursive patterns currently only support constant pattern matching. Found: {subpattern.Pattern?.GetType().Name ?? "null"}. Use syntax like '{{ PropertyName: constantValue }}'.");
            }

            if (hasSubpattern)
            {
                JumpTarget evaluateNextPattern = new();
                JumpTarget endPattern = new();
                Jump(OpCode.JMPIF_L, evaluateNextPattern);
                Push(false);
                Jump(OpCode.JMP_L, endPattern);
                evaluateNextPattern.Instruction = AddInstruction(OpCode.NOP);
                EmitRecursivePropertyConstantPatternCheck(model, subpattern, constantPattern, localIndex);
                endPattern.Instruction = AddInstruction(OpCode.NOP);
            }
            else
            {
                EmitRecursivePropertyConstantPatternCheck(model, subpattern, constantPattern, localIndex);
                hasSubpattern = true;
            }
        }

        if (!hasSubpattern)
        {
            Push(true);
        }
    }

    private void EmitRecursivePropertyConstantPatternCheck(SemanticModel model, SubpatternSyntax subpattern, ConstantPatternSyntax constantPattern, byte localIndex)
    {
        // Example:
        // if (newOwner is { IsValid: true, IsZero:false})
        // {
        // }
        if (subpattern.NameColon is null)
        {
            throw CompilationException.UnsupportedSyntax(subpattern, "Recursive property patterns must specify a property name. Use syntax like '{ PropertyName: constantValue }'.");
        }

        var propertySymbol = model.GetSymbolInfo(subpattern.NameColon.Name).Symbol!;
        if (propertySymbol is not IPropertySymbol property)
        {
            throw CompilationException.UnsupportedSyntax(subpattern, $"Recursive pattern can only match properties, not '{propertySymbol.GetType().Name}'. Ensure '{subpattern.NameColon.Name.ToString()}' is a property with a getter.");
        }

        if (property.GetMethod is null)
        {
            throw CompilationException.UnsupportedSyntax(subpattern, $"Property '{property.Name}' must have a getter to be used in a recursive pattern.");
        }

        AccessSlot(OpCode.LDLOC, localIndex);
        CallMethodWithConvention(model, property.GetMethod);
        object? constantValue = model.GetConstantValue(constantPattern.Expression).Value;
        Push(constantValue);
        AddInstruction(OpCode.EQUAL);
    }
}
