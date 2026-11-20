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
using System.Collections.Generic;
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
        HashSet<INamedTypeSymbol> initializedClasses = new(SymbolEqualityComparer.Default);
        HashSet<INamedTypeSymbol> initializingClasses = new(SymbolEqualityComparer.Default);
        Dictionary<ITypeSymbol, int> initializedVTables = new(SymbolEqualityComparer.Default);
        int staticFieldInitializationStart = _instructions.Count;
        bool initializedAny;

        // Converting initializers and vtable methods can discover more static slots.
        // Keep vtable initialization ahead of every static field initializer.
        do
        {
            initializedAny = false;

            int vTableInitializationStart = _instructions.Count;
            foreach (var (fieldIndex, type) in _context.VTables.ToArray())
            {
                int aliasCount = _context.ByRefAliasSignatures.Count;
                if (initializedVTables.TryGetValue(type, out int initializedAliasCount) && initializedAliasCount == aliasCount)
                    continue;
                initializedVTables[type] = aliasCount;
                initializedAny = true;
                IMethodSymbol[] virtualMethods = type.GetAllMembers().OfType<IMethodSymbol>().Where(p => p.IsVirtualMethod()).ToArray();
                for (int i = virtualMethods.Length - 1; i >= 0; i--)
                {
                    IMethodSymbol method = virtualMethods[i];
                    if (!HasByRefAliasVariants(method))
                    {
                        if (method.IsAbstract)
                            Push((object?)null);
                        else
                            InvokeMethod(model, method);
                        continue;
                    }

                    for (int aliasIndex = aliasCount - 1; aliasIndex >= 0; aliasIndex--)
                    {
                        if (method.IsAbstract)
                        {
                            Push((object?)null);
                        }
                        else
                        {
                            MethodConvert convert = _context.ConvertMethod(model, method);
                            MethodConvert alias = _context.AddByRefAliasMethod(convert, _context.ByRefAliasSignatures[aliasIndex]);
                            InvokeMethod(alias);
                        }
                    }
                    if (method.IsAbstract)
                        Push((object?)null);
                    else
                        InvokeMethod(model, method);
                    Push(aliasCount + 1);
                    AddInstruction(OpCode.PACK);
                }
                Push(virtualMethods.Length);
                AddInstruction(OpCode.PACK);
                AccessSlot(OpCode.STSFLD, fieldIndex);
            }

            if (vTableInitializationStart < _instructions.Count)
            {
                var vTableInitialization = _instructions[vTableInitializationStart..];
                _instructions.RemoveRange(vTableInitializationStart, vTableInitialization.Count);
                _instructions.InsertRange(staticFieldInitializationStart, vTableInitialization);
                staticFieldInitializationStart += vTableInitialization.Count;
            }

            foreach (INamedTypeSymbol @class in _context.StaticFieldSymbols.Select(p => p.ContainingType).Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default).ToArray())
            {
                InitializeClass(@class);
            }
        }
        while (initializedAny);

        void InitializeClass(INamedTypeSymbol @class)
        {
            if (initializedClasses.Contains(@class) || !initializingClasses.Add(@class)) return;

            // Follow calls and property getters as well as direct field reads. A helper can
            // read a field in a class that has not been initialized yet.
            foreach (IFieldSymbol field in @class.GetAllMembers().OfType<IFieldSymbol>())
            {
                if (field.IsConst || !field.IsStatic || field.DeclaringSyntaxReferences.IsEmpty) continue;
                if (field.DeclaringSyntaxReferences[0].GetSyntax() is not VariableDeclaratorSyntax { Initializer: { } initializer }) continue;

                HashSet<ISymbol> visitedMembers = new(SymbolEqualityComparer.Default);
                ScanDependencies(initializer.Value, field.ContainingAssembly, visitedMembers);
            }

            initializingClasses.Remove(@class);
            if (initializedClasses.Add(@class))
            {
                initializedAny = true;
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
        }

        void ScanDependencies(SyntaxNode syntax, IAssemblySymbol assembly, HashSet<ISymbol> visitedMembers)
        {
            if (assembly is not ISourceAssemblySymbol sourceAssembly) return;
            SemanticModel semanticModel = sourceAssembly.Compilation.GetSemanticModel(syntax.SyntaxTree);
            foreach (SyntaxNode node in syntax.DescendantNodesAndSelf())
            {
                switch (semanticModel.GetSymbolInfo(node).Symbol)
                {
                    case IFieldSymbol { IsStatic: true, IsConst: false } field:
                        InitializeClass(field.ContainingType);
                        break;
                    case IPropertySymbol { IsStatic: true } property:
                        InitializeClass(property.ContainingType);
                        ScanMember(property.GetMethod, visitedMembers);
                        break;
                    case IMethodSymbol { IsStatic: true } method:
                        InitializeClass(method.ContainingType);
                        ScanMember(method, visitedMembers);
                        break;
                }
            }
        }

        void ScanMember(IMethodSymbol? method, HashSet<ISymbol> visitedMembers)
        {
            if (method is null || !visitedMembers.Add(method)) return;
            foreach (SyntaxReference reference in method.DeclaringSyntaxReferences)
                ScanDependencies(reference.GetSyntax(), method.ContainingAssembly, visitedMembers);
        }
    }
}
