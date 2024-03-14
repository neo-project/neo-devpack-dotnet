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
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

partial class MethodConvert
{
    private void ConvertRecursivePattern(SemanticModel model, RecursivePatternSyntax pattern, byte localIndex)
    {
            ITypeSymbol type = model.GetTypeInfo(pattern.Type).Type!;
            AccessSlot(OpCode.LDLOC, localIndex);
            IsType(type.GetPatternType());

            ILocalSymbol? positionLocal = null;
            if (pattern.PositionalPatternClause is PositionalPatternClauseSyntax positionalClause)
            {
                positionLocal = (ILocalSymbol)model.GetDeclaredSymbol(positionalClause.Subpatterns.FirstOrDefault())!;
                byte positionIndex = AddLocalVariable(positionLocal);

                AccessSlot(OpCode.LDLOC, localIndex);
                Call(type.GetMembers("Deconstruct").OfType<IMethodSymbol>().First());
                AccessSlot(OpCode.STLOC, positionIndex);
            }

            if (pattern.PropertyPatternClause is PropertyPatternClauseSyntax propertyClause)
            {
                foreach (SubpatternSyntax subpattern in propertyClause.Subpatterns)
                {
                    ILocalSymbol local = (ILocalSymbol)model.GetDeclaredSymbol(subpattern)!;
                    byte index = AddLocalVariable(local);

                    AccessSlot(OpCode.LDLOC, localIndex);
                    Call(((IPropertySymbol)model.GetSymbolInfo(subpattern.NameColon!.Name).Symbol!).GetMethod!);
                    AccessSlot(OpCode.STLOC, index);
                }
            }

            if (pattern.Designation is ParenthesizedVariableDesignationSyntax designation)
            {
                ILocalSymbol local = (ILocalSymbol)model.GetDeclaredSymbol(designation.Variables.First())!;
                byte index = AddLocalVariable(local);

                AccessSlot(OpCode.LDLOC, localIndex);
                AccessSlot(OpCode.STLOC, index);
            }

            if (positionLocal is not null)
            {
                RemoveLocalVariable(positionLocal);
            }
        }
}
