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
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

namespace Neo.Compiler;

partial class MethodConvert
{
    private void ConvertSource(SemanticModel model)
    {
        if (SyntaxNode is null) return;
        for (byte i = 0; i < Symbol.Parameters.Length; i++)
        {
            IParameterSymbol parameter = Symbol.Parameters[i].OriginalDefinition;
            byte index = i;
            if (!Symbol.IsStatic) index++;
            _parameters.Add(parameter, index);
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
                    //   public void Test() => a+=1; // this will push a int value to the stack
                    //   public void Test() { a+=1; } // this will not push value to the stack
                    if (syntax is MethodDeclarationSyntax methodSyntax
                        && methodSyntax.ReturnType.ToString() == "void"
                        && IsExpressionReturningValue(model, methodSyntax))
                        AddInstruction(OpCode.DROP);
                }
                else
                    ConvertStatement(model, syntax.Body);
                break;
            default:
                throw new CompilationException(SyntaxNode, DiagnosticId.SyntaxNotSupported, $"Unsupported method body:{SyntaxNode}");
        }
        _initslot = !_inline;
    }

    private static bool IsExpressionReturningValue(SemanticModel semanticModel, MethodDeclarationSyntax methodDeclaration)
    {
        // Check if it's a method declaration with an expression body
        if (methodDeclaration is { ExpressionBody: not null } methodSyntax)
        {
            // Retrieve the expression from the expression body
            var expression = methodSyntax.ExpressionBody.Expression;

            // Use the semantic model to get the type information of the expression
            var typeInfo = semanticModel.GetTypeInfo(expression);

            // Check if the expression's type is not void, meaning the expression has a return value
            return typeInfo.ConvertedType?.SpecialType != SpecialType.System_Void;
        }

        // For other types of BaseMethodDeclarationSyntax or cases without an expression body, default to no return value
        return false;
    }
}
