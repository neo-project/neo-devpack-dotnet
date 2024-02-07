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
using Neo.SmartContract;
using Neo.VM;
using System.Linq;

namespace Neo.Compiler;

partial class MethodConvert
{

    /// <summary>
    /// Converts method, delegate or event invocation expressions to executable code.
    /// </summary>
    /// <param name="model">The semantic model</param>
    /// <param name="expression">The invocation expression syntax</param>
    /// <remarks>
    /// Handles expressions like:
    ///
    /// DoWork(x, y);
    /// handler(args);
    /// somethingHappened += OnEvent;
    ///
    /// Determines if it is a method call, delegate call or event access based on the symbol.
    /// Then emits appropriate instructions to execute the invocation.
    /// </remarks>
    private void ConvertInvocationExpression(SemanticModel model, InvocationExpressionSyntax expression)
    {
        ArgumentSyntax[] arguments = expression.ArgumentList.Arguments.ToArray();
        ISymbol symbol = model.GetSymbolInfo(expression.Expression).Symbol!;
        switch (symbol)
        {
            case IEventSymbol @event:
                // Example event access:
                // somethingHappened += OnEvent;
                ConvertEventInvocationExpression(model, @event, arguments);
                break;
            case IMethodSymbol method:
                // Example method call:
                // DoWork(x, y);
                ConvertMethodInvocationExpression(model, method, expression.Expression, arguments);
                break;
            default:
                // Example delegate call:
                // handler(args);
                ConvertDelegateInvocationExpression(model, expression.Expression, arguments);
                break;
        }
    }

    private void ConvertEventInvocationExpression(SemanticModel model, IEventSymbol symbol, ArgumentSyntax[] arguments)
    {
        AddInstruction(OpCode.NEWARRAY0);
        foreach (ArgumentSyntax argument in arguments)
        {
            AddInstruction(OpCode.DUP);
            ConvertExpression(model, argument.Expression);
            AddInstruction(OpCode.APPEND);
        }
        Push(symbol.GetDisplayName());
        Call(ApplicationEngine.System_Runtime_Notify);
    }

    private void ConvertMethodInvocationExpression(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax expression, ArgumentSyntax[] arguments)
    {
        switch (expression)
        {
            case IdentifierNameSyntax:
                Call(model, symbol, null, arguments);
                break;
            case MemberAccessExpressionSyntax syntax:
                if (symbol.IsStatic)
                    Call(model, symbol, null, arguments);
                else
                    Call(model, symbol, syntax.Expression, arguments);
                break;
            case MemberBindingExpressionSyntax:
                Call(model, symbol, true, arguments);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported expression: {expression}");
        }
    }

    private void ConvertDelegateInvocationExpression(SemanticModel model, ExpressionSyntax expression, ArgumentSyntax[] arguments)
    {
        INamedTypeSymbol type = (INamedTypeSymbol)model.GetTypeInfo(expression).Type!;
        PrepareArgumentsForMethod(model, type.DelegateInvokeMethod!, arguments);
        ConvertExpression(model, expression);
        AddInstruction(OpCode.CALLA);
    }
}
