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
using Neo.VM;
using System.Linq;

namespace Neo.Compiler;

partial class MethodConvert
{
    private void ProcessStaticFields(SemanticModel model)
    {
        foreach (INamedTypeSymbol @class in context.StaticFieldSymbols.Select(p => p.ContainingType).Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default).ToArray())
        {
            foreach (IFieldSymbol field in @class.GetAllMembers().OfType<IFieldSymbol>())
            {
                if (field.IsConst || !field.IsStatic) continue;
                ProcessFieldInitializer(model, field, null, () =>
                {
                    byte index = context.AddStaticField(field);
                    AccessSlot(OpCode.STSFLD, index);
                });
            }
        }
        foreach (var (fieldIndex, type) in context.VTables)
        {
            IMethodSymbol[] virtualMethods = type.GetAllMembers().OfType<IMethodSymbol>().Where(p => p.IsVirtualMethod()).ToArray();
            for (int i = virtualMethods.Length - 1; i >= 0; i--)
            {
                IMethodSymbol method = virtualMethods[i];
                if (method.IsAbstract)
                {
                    Push((object?)null);
                }
                else
                {
                    MethodConvert convert = context.ConvertMethod(model, method);
                    Jump(OpCode.PUSHA, convert._startTarget);
                }
            }
            Push(virtualMethods.Length);
            AddInstruction(OpCode.PACK);
            AccessSlot(OpCode.STSFLD, fieldIndex);
        }
    }
}
