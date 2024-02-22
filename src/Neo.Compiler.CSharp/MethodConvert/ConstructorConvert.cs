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
using Neo.IO;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;
using scfx::Neo.SmartContract.Framework.Attributes;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Neo.Compiler;

partial class MethodConvert
{
    private void ProcessFields(SemanticModel model)
    {
        _initslot = true;
        IFieldSymbol[] fields = Symbol.ContainingType.GetFields();
        for (int i = 0; i < fields.Length; i++)
        {
            ProcessFieldInitializer(model, fields[i], () =>
            {
                AddInstruction(OpCode.LDARG0);
                Push(i);
            }, () =>
            {
                AddInstruction(OpCode.SETITEM);
            });
        }
    }

    private void ProcessConstructorInitializer(SemanticModel model)
    {
        INamedTypeSymbol type = Symbol.ContainingType;
        if (type.IsValueType) return;
        INamedTypeSymbol baseType = type.BaseType!;
        if (baseType.SpecialType == SpecialType.System_Object) return;
        ConstructorInitializerSyntax? initializer = ((ConstructorDeclarationSyntax?)SyntaxNode)?.Initializer;
        if (initializer is null)
        {
            IMethodSymbol baseConstructor = baseType.InstanceConstructors.First(p => p.Parameters.Length == 0);
            if (baseType.DeclaringSyntaxReferences.IsEmpty && baseConstructor.GetAttributes().All(p => p.AttributeClass!.ContainingAssembly.Name != "Neo.SmartContract.Framework"))
                return;
            Call(model, baseConstructor, null);
        }
        else
        {
            IMethodSymbol baseConstructor = (IMethodSymbol)model.GetSymbolInfo(initializer).Symbol!;
            using (InsertSequencePoint(initializer))
                Call(model, baseConstructor, null, initializer.ArgumentList.Arguments.ToArray());
        }
    }

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
