// Copyright (C) 2015-2025 The Neo Project.
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    internal static readonly Regex s_pattern = new(@"^(Neo\.SmartContract\.Framework\.SmartContract|SmartContract\.Framework\.SmartContract|Framework\.SmartContract|SmartContract|Neo\.SmartContract\.Framework\.Nep17Token|Neo\.SmartContract\.Framework\.TokenContract|Neo.SmartContract.Framework.Nep11Token<.*>)$");

    private void ConvertSource(SemanticModel model)
    {
        if (SyntaxNode is null) return;
        for (byte i = 0; i < Symbol.Parameters.Length; i++)
        {
            IParameterSymbol parameter = Symbol.Parameters[i].OriginalDefinition;
            byte index = i;
            if (NeedInstanceConstructor(Symbol)) index++;
            _parameters.Add(parameter, index);

            if (parameter.RefKind == RefKind.Out)
            {
                _context.GetOrAddCapturedStaticField(parameter);
            }
        }
        switch (SyntaxNode)
        {
            case AccessorDeclarationSyntax syntax:
                if (syntax.Body is not null)
                    ConvertStatement(model, syntax.Body);
                else if (syntax.ExpressionBody is not null)
                    ConvertExpression(model, syntax.ExpressionBody.Expression);
                else
                    ConvertNoBody(syntax);
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
        if (Symbol.MethodKind == MethodKind.Constructor && Symbol.ContainingType.IsRecord)
        {
            _initSlot = true;
            IFieldSymbol[] fields = Symbol.ContainingType.GetFields();
            for (byte i = 1; i <= fields.Length; i++)
            {
                AddInstruction(OpCode.LDARG0);
                Push(i - 1);
                AccessSlot(OpCode.LDARG, i);
                AddInstruction(OpCode.SETITEM);
            }
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
            var typeInfo = semanticModel.GetTypeInfo(expression);

            // Check if the expression's type is not void, meaning the expression has a return value
            return typeInfo.ConvertedType?.SpecialType != SpecialType.System_Void;
        }

        // For other types of BaseMethodDeclarationSyntax or cases without an expression body, default to no return value
        return false;
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
