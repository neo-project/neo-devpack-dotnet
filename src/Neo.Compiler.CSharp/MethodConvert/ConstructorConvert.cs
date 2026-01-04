// Copyright (C) 2015-2026 The Neo Project.
//
// ConstructorConvert.cs file belongs to the neo project and is free
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
using Neo.IO;
using Neo.VM;
using System;
using System.Linq;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private void ProcessConstructorInitializer(SemanticModel model)
    {
        INamedTypeSymbol type = Symbol.ContainingType;
        if (type.IsValueType) return;
        INamedTypeSymbol baseType = type.BaseType!;
        if (baseType.SpecialType == SpecialType.System_Object) return;
        if (SyntaxNode is RecordDeclarationSyntax recordSyntax)
        {
            var baseCtorSyntax = recordSyntax.BaseList?.Types
                .OfType<PrimaryConstructorBaseTypeSyntax>()
                .FirstOrDefault();
            if (baseCtorSyntax is not null)
            {
                var arguments = baseCtorSyntax.ArgumentList?.Arguments.ToArray() ?? Array.Empty<ArgumentSyntax>();
                IMethodSymbol? baseCtor = baseType.InstanceConstructors.FirstOrDefault(p => p.Parameters.Length == arguments.Length);
                if (baseCtor is not null)
                {
                    // Records invoke their base constructors through a primary-constructor clause (e.g. : base(arg)).
                    // Emit the same call sequence we use for explicit constructor initializers so the positional
                    // record layout is populated correctly.
                    using (InsertSequencePoint(baseCtorSyntax))
                        CallMethodWithInstanceExpression(model, baseCtor, null, arguments);
                    return;
                }
            }
        }

        ConstructorInitializerSyntax? initializer = ((ConstructorDeclarationSyntax?)SyntaxNode)?.Initializer;
        if (initializer is null)
        {
            if (baseType.ContainingNamespace.ToString() == "Neo.SmartContract.Framework" && baseType.Name == "SmartContract")
                return;
            IMethodSymbol baseConstructor = baseType.InstanceConstructors.First(p => p.Parameters.Length == 0);
            if (baseType.DeclaringSyntaxReferences.IsEmpty && baseConstructor.GetAttributes().All(p => p.AttributeClass!.ContainingAssembly.Name != "Neo.SmartContract.Framework"))
                return;
            CallMethodWithInstanceExpression(model, baseConstructor, null);
        }
        else
        {
            IMethodSymbol baseConstructor = (IMethodSymbol)model.GetSymbolInfo(initializer).Symbol!;
            using (InsertSequencePoint(initializer))
                CallMethodWithInstanceExpression(model, baseConstructor, null, initializer.ArgumentList.Arguments.ToArray());
        }
    }

    private void ProcessStaticFields(SemanticModel model)
    {
        foreach (INamedTypeSymbol @class in _context.StaticFieldSymbols.Select(p => p.ContainingType).Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default).ToArray())
        {
            foreach (IFieldSymbol field in @class.GetAllMembers().OfType<IFieldSymbol>())
            {
                if (field.IsConst || !field.IsStatic) continue;
                ProcessFieldInitializer(model, field, null, () =>
                {
                    byte index = _context.AddStaticField(field);
                    AccessSlot(OpCode.STSFLD, index);
                });
            }
        }
        foreach (var (fieldIndex, type) in _context.VTables)
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
                    InvokeMethod(model, method);
                }
            }
            Push(virtualMethods.Length);
            AddInstruction(OpCode.PACK);
            AccessSlot(OpCode.STSFLD, fieldIndex);
        }
    }
}
