// Copyright (C) 2015-2026 The Neo Project.
//
// SourceConvert.cs file belongs to the neo project and is free
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
using Neo.VM;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    internal static readonly Regex s_pattern = new(@"^(Neo\.SmartContract\.Framework\.SmartContract|SmartContract\.Framework\.SmartContract|Framework\.SmartContract|SmartContract|Neo\.SmartContract\.Framework\.Nep17Token|Neo\.SmartContract\.Framework\.TokenContract|Neo.SmartContract.Framework.Nep11Token<.*>)$");

    private static bool IsByRef(RefKind refKind)
        => refKind == RefKind.Ref || refKind == RefKind.Out;

    private void RegisterMethodParameters()
    {
        if (_parameters.Count > 0)
            return;

        for (byte i = 0; i < Symbol.Parameters.Length; i++)
        {
            IParameterSymbol parameter = Symbol.Parameters[i];
            IParameterSymbol original = parameter.OriginalDefinition;
            byte index = i;
            if (NeedInstanceConstructor(Symbol)) index++;

            _parameters.TryAdd(parameter, index);
            if (!SymbolEqualityComparer.Default.Equals(parameter, original))
            {
                _parameters.TryAdd(original, index);
            }

            if (IsByRef(parameter.RefKind))
            {
                _context.GetOrAddCapturedStaticField(parameter);
            }
        }
    }

    private void ConvertSource(SemanticModel model)
    {
        if (SyntaxNode is null) return;
        switch (SyntaxNode)
        {
            case AccessorDeclarationSyntax syntax:
                if (syntax.Body is not null)
                {
                    ConvertStatement(model, syntax.Body);
                }
                else if (syntax.ExpressionBody is not null)
                {
                    ConvertExpression(model, syntax.ExpressionBody.Expression);
                    if (Symbol.ReturnsVoid &&
                        IsExpressionReturningValue(model, syntax.ExpressionBody.Expression))
                    {
                        AddInstruction(OpCode.DROP);
                    }
                }
                else
                {
                    ConvertNoBody(syntax);
                }
                break;
            case ArrowExpressionClauseSyntax syntax:
                ConvertExpression(model, syntax.Expression);
                break;
            case BaseMethodDeclarationSyntax syntax:
                if (syntax.Body is null)
                {
                    ConvertExpression(model, syntax.ExpressionBody!.Expression);
                    // If the method has no return value,
                    // but the expression body has a return value, example: a+=1;
                    // drop the return value
                    // Problem:
                    //   public void Test() => a+=1; // this will push an int value to the stack
                    //   public void Test() { a+=1; } // this will not push value to the stack
                    if (syntax is MethodDeclarationSyntax methodSyntax
                        && methodSyntax.ReturnType.ToString() == "void"
                        && IsExpressionReturningValue(model, methodSyntax))
                        AddInstruction(OpCode.DROP);
                }
                else
                    ConvertStatement(model, syntax.Body);
                break;

            case SimpleLambdaExpressionSyntax syntax:
                if (syntax.Block is null)
                {
                    ConvertExpression(model, syntax.ExpressionBody!);
                }
                else
                {
                    ConvertStatement(model, syntax.Block);
                }
                break;
            case ParenthesizedLambdaExpressionSyntax syntax:
                if (syntax.Block is null)
                {
                    ConvertExpression(model, syntax.ExpressionBody!);
                }
                else
                {
                    ConvertStatement(model, syntax.Block);
                }
                break;
            case RecordDeclarationSyntax record:
                ConvertDefaultRecordConstruct(record);
                break;
            case ParameterSyntax parameter:
                ConvertRecordPropertyInitMethod(parameter);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(SyntaxNode, $"Method body type '{SyntaxNode.GetType().Name}' is not supported. Only standard method bodies, expression bodies, accessor bodies, arrow expressions, record constructors, and parameter property initializers are supported.");
        }
        // Set _initSlot to true for non-inline methods
        // This ensures that regular methods will have the INITSLOT instruction added
        _initSlot = !_inline;
    }

    private void ConvertDefaultRecordConstruct(RecordDeclarationSyntax recordDeclarationSyntax)
    {
        if (Symbol.MethodKind != MethodKind.Constructor || !Symbol.ContainingType.IsRecord)
            return;

        // Only the primary (positional) parameters should be copied into the struct backing array.
        // Secondary members (init-only properties, fields) keep their own initialization logic and must
        // not shift the positional layout. We cache their names so we can ignore the rest of the members.
        var positionalParameters = recordDeclarationSyntax.ParameterList?.Parameters
            .Select(p => p.Identifier.ValueText)
            .ToHashSet(StringComparer.Ordinal);
        if (positionalParameters is null || positionalParameters.Count == 0)
            return;

        _initSlot = true;
        INamedTypeSymbol type = Symbol.ContainingType;
        IFieldSymbol[] fields = type.GetFields();
        if (fields.Length == 0)
            return;

        // Struct layout is positional, so capture the field index to avoid repeated linear scans.
        var fieldIndices = new Dictionary<IFieldSymbol, int>(SymbolEqualityComparer.Default);
        for (int i = 0; i < fields.Length; i++)
        {
            fieldIndices[fields[i]] = i;
        }

        // Roslyn synthesizes a property for each primary parameter; in partial/extended records there may
        // be user-defined members with the same name. Group by name so we can resolve the associated field
        // even when multiple candidates exist (e.g. with explicit property implementations).
        var propertiesByName = type.GetMembers()
            .OfType<IPropertySymbol>()
            .GroupBy(p => p.Name, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.ToArray(), StringComparer.Ordinal);

        foreach (var parameter in Symbol.Parameters)
        {
            if (!positionalParameters.Contains(parameter.Name))
                continue;

            IFieldSymbol? backingField = null;

            // Prefer the synthesized property backing field when available; otherwise fall back to the
            // compiler-generated '<name>k__BackingField'. Record extensions may expose the property as
            // part of a partial class, so we tolerate multiple candidates.
            if (propertiesByName.TryGetValue(parameter.Name, out IPropertySymbol[]? candidates))
            {
                backingField = candidates
                    .Select(prop => fields.FirstOrDefault(f => SymbolEqualityComparer.Default.Equals(f.AssociatedSymbol, prop)))
                    .FirstOrDefault(f => f is not null);
            }

            backingField ??= fields.FirstOrDefault(f => f.Name == $"<{parameter.Name}>k__BackingField");

            if (backingField is null || !fieldIndices.TryGetValue(backingField, out int fieldIndex))
                continue;

            AddInstruction(OpCode.LDARG0);
            Push(fieldIndex);
            AccessSlot(OpCode.LDARG, (byte)(parameter.Ordinal + 1));
            AddInstruction(OpCode.SETITEM);
        }
    }

    private void ConvertRecordPropertyInitMethod(ParameterSyntax parameter)
    {
        IPropertySymbol property = (IPropertySymbol)Symbol.AssociatedSymbol!;
        ConvertFieldBackedProperty(property);
    }

    private static bool IsExpressionReturningValue(SemanticModel semanticModel, MethodDeclarationSyntax methodDeclaration)
    {
        // Check if it's a method declaration with an expression body
        if (methodDeclaration is { ExpressionBody: not null })
        {
            // Retrieve the expression from the expression body
            var expression = methodDeclaration.ExpressionBody.Expression;

            // Use the semantic model to get the type information of the expression
            var correctModel = semanticModel.GetModelForNode(expression);
            var typeInfo = correctModel.GetTypeInfo(expression);

            // Check if the expression's type is not void, meaning the expression has a return value
            return typeInfo.ConvertedType?.SpecialType != SpecialType.System_Void;
        }

        // For other types of BaseMethodDeclarationSyntax or cases without an expression body, default to no return value
        return false;
    }

    private static bool IsExpressionReturningValue(SemanticModel semanticModel, ExpressionSyntax expression)
    {
        var correctModel = semanticModel.GetModelForNode(expression);
        var typeInfo = correctModel.GetTypeInfo(expression);
        return typeInfo.ConvertedType?.SpecialType != SpecialType.System_Void;
    }

    internal static ConcurrentDictionary<IMethodSymbol, bool> _cacheNeedInstanceConstructor = new ConcurrentDictionary<IMethodSymbol, bool>(SymbolEqualityComparer.Default);

    /// <summary>
    /// non-static methods needs constructors to be executed
    /// But non-static method in smart contract classes without explicit constructor
    /// does not constructors
    /// Cases we need instance constructors:
    /// 1. non-static smart contract with explicit instance constructor in itself
    /// 2. non-static ordinary method, with explicit instance constructor in itself or its base classes
    /// </summary>
    /// <param name="symbol">A method in class</param>
    /// <returns></returns>
    internal static bool NeedInstanceConstructor(IMethodSymbol symbol)
    {
        if (_cacheNeedInstanceConstructor.TryGetValue(symbol, out bool result))
            return result;
        static bool NeedInstanceConstructorInner(IMethodSymbol symbol)
        {
            if (symbol.IsStatic || symbol.MethodKind == MethodKind.AnonymousFunction)
                return false;
            INamedTypeSymbol? containingClass = symbol.ContainingType;
            if (containingClass == null)
                return false;
            // non-static methods in class
            if ((symbol.MethodKind == MethodKind.Constructor || symbol.MethodKind == MethodKind.SharedConstructor)
                && !CompilationEngine.IsDerivedFromSmartContract(containingClass))
                // is constructor, and is not smart contract
                // typically seen in framework methods
                return true;
            if (containingClass!.Constructors
                .FirstOrDefault(p => p.Parameters.Length == 0 && !p.IsStatic)?
                .DeclaringSyntaxReferences.Length > 0)
                // has explicit constructor
                return true;
            // No explicit non-static constructor in class
            // is smart contract, or is normal non-static method (whether contract or not)
            if (!s_pattern.IsMatch(containingClass?.BaseType?.ToString() ?? string.Empty))
                // class itself is not directly inheriting smart contract; can have more base classes
                return true;
            // is non-static method, directly inheriting smart contract
            if (containingClass!.GetFields().Any((IFieldSymbol f) => !f.IsStatic))
                // has non-static fields
                return true;
            return false;
        }
        return _cacheNeedInstanceConstructor[symbol] = NeedInstanceConstructorInner(symbol);
    }
}
