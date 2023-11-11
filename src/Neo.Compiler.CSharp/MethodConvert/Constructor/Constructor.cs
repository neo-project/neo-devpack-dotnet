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
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Neo.Compiler;

partial class MethodConvert
{
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
}
