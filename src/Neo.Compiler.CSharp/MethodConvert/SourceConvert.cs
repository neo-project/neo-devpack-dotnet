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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    private void ConvertSource(SemanticModel model)
    {
        if (SyntaxNode is null) return;
        for (byte i = 0; i < Symbol.Parameters.Length; i++)
        {
            IParameterSymbol parameter = Symbol.Parameters[i].OriginalDefinition;
            byte index = i;
            if (IsInstanceMethod(Symbol)) index++;
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
                throw new CompilationException(SyntaxNode, DiagnosticId.SyntaxNotSupported, $"Unsupported method body:{SyntaxNode}");
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

    private bool IsInstanceMethod(IMethodSymbol symbol) => !symbol.IsStatic && symbol.MethodKind != MethodKind.AnonymousFunction;

}
