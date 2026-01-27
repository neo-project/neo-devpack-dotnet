// Copyright (C) 2015-2026 The Neo Project.
//
// InvocationExpression.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Converts Invocation, include method invocation, event invocation and delegate invocation to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about invocation expression.</param>
    /// <param name="expression">The syntax representation of the invocation expression statement being converted.</param>
    private void ConvertInvocationExpression(SemanticModel model, InvocationExpressionSyntax expression)
    {
        ArgumentSyntax[] arguments = expression.ArgumentList.Arguments.ToArray();
        ISymbol symbol = model.GetSymbolInfo(expression.Expression).Symbol!;

        if (symbol is IMethodSymbol methodSymbol && TryOptimizeParseInvocation(model, methodSymbol, arguments))
            return;

        switch (symbol)
        {
            case IEventSymbol @event:
                ConvertEventInvocationExpression(model, @event, arguments);
                break;
            case IMethodSymbol method:
                ConvertMethodInvocationExpression(model, method, expression.Expression, arguments);
                break;
            default:
                ConvertDelegateInvocationExpression(model, expression.Expression, arguments);
                break;
        }
    }

    private bool TryOptimizeParseInvocation(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<ArgumentSyntax> arguments)
    {
        if (symbol.Name != "Parse" || symbol.Parameters.Length != 1 || arguments.Count != 1)
            return false;

        ArgumentSyntax argument = arguments[0];
        if (argument.NameColon is not null)
            return false;

        Optional<object?> constant = model.GetConstantValue(argument.Expression);
        if (!constant.HasValue || constant.Value is not string stringValue)
            return false;

        try
        {
            using var sequence = InsertSequencePoint(argument.Expression);
            switch (symbol.ContainingType.ToString())
            {
                case "Neo.SmartContract.Framework.UInt160":
                    Push(ConvertToUInt160(stringValue));
                    return true;
                case "Neo.SmartContract.Framework.UInt256":
                    Push(ConvertToUInt256(stringValue, argument.Expression));
                    return true;
                case "Neo.SmartContract.Framework.ECPoint":
                    Push(ConvertToECPoint(stringValue));
                    return true;
                default:
                    return false;
            }
        }
        catch (Exception ex) when (ex is FormatException or CompilationException)
        {
            throw new CompilationException(argument.Expression, DiagnosticId.InvalidInitialValue, $"Invalid {symbol.ContainingType.Name} literal: {stringValue}");
        }
    }

    /// <summary>
    /// Convert the event invocation expression to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about event invocation expression.</param>
    /// <param name="symbol">Symbol of the event</param>
    /// <param name="arguments">Arguments of the event</param>
    /// <example><see href="https://github.com/neo-project/neo-devpack-dotnet/blob/master/examples/Example.SmartContract.Event/Event.cs"/></example>
    private void ConvertEventInvocationExpression(SemanticModel model, IEventSymbol symbol, ArgumentSyntax[] arguments)
    {
        foreach (ArgumentSyntax argument in arguments.Reverse())  // PACK works in a reversed way
            ConvertExpression(model, argument.Expression);
        Push(arguments.Length);
        AddInstruction(OpCode.PACK);
        Push(symbol.GetDisplayName());
        CallInteropMethod(ApplicationEngine.System_Runtime_Notify);
    }

    /// <summary>
    /// Convert the method invocation expression to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about method invocation expression.</param>
    /// <param name="symbol">Symbol of the method</param>
    /// <param name="expression">The syntax representation of the method invocation expression statement being converted.</param>
    /// <param name="arguments">Arguments of the method</param>
    /// <example>
    /// <c>Runtime.Log("hello World!");</c>
    /// </example>
    private void ConvertMethodInvocationExpression(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax expression, ArgumentSyntax[] arguments)
    {
        switch (expression)
        {
            case IdentifierNameSyntax:
                CallMethodWithInstanceExpression(model, symbol, null, arguments);
                break;
            case MemberAccessExpressionSyntax syntax:
                // Handle Event?.Invoke() pattern - check if this is calling Invoke on an event
                if (symbol.Name == "Invoke" && syntax.Expression is IdentifierNameSyntax identifier)
                {
                    var receiverSymbol = model.GetSymbolInfo(identifier).Symbol;
                    if (receiverSymbol is IEventSymbol eventSymbol)
                    {
                        ConvertEventInvocationExpression(model, eventSymbol, arguments);
                        return;
                    }
                }
                CallMethodWithInstanceExpression(model, symbol, symbol.IsStatic ? null : syntax.Expression, arguments);
                break;
            case MemberBindingExpressionSyntax:
                // Handle Event?.Invoke() pattern in conditional access
                if (symbol.Name == "Invoke" && symbol.ContainingType.TypeKind == TypeKind.Delegate)
                {
                    // Find the event symbol from the parent conditional access expression
                    var parent = expression.Parent?.Parent;
                    if (parent is InvocationExpressionSyntax invocation &&
                        invocation.Parent is ConditionalAccessExpressionSyntax conditional)
                    {
                        var receiverSymbol = model.GetSymbolInfo(conditional.Expression).Symbol;
                        if (receiverSymbol is IEventSymbol eventSymbol)
                        {
                            // Drop the duplicated receiver from the stack (from ConditionalAccessExpression)
                            AddInstruction(OpCode.DROP);
                            ConvertEventInvocationExpression(model, eventSymbol, arguments);
                            return;
                        }
                    }
                }
                CallInstanceMethod(model, symbol, true, arguments);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(expression, $"Unsupported invocation expression '{expression.GetType().Name}'. Use supported patterns: method calls, property access, or delegate invocations.");
        }
    }

    /// <summary>
    /// Convert the delegate invocation expression to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about delegate invocation expression.</param>
    /// <param name="expression">The syntax representation of the delegate invocation expression statement being converted.</param>
    /// <param name="arguments">Arguments of the delegate</param>
    /// <example>
    /// <code>
    /// public delegate int MyDelegate(int x, int y);
    ///
    /// static int CalculateSum(int x, int y)
    /// {
    ///     return x + y;
    /// }
    ///
    /// public void MyMethod()
    /// {
    ///     MyDelegate myDelegate = CalculateSum;
    ///     int result = myDelegate(5, 6);
    ///     Runtime.Log($"Sum: {result}");
    /// }
    /// </code>
    /// <c>myDelegate(5, 6)</c> This line will be converted by the following method.
    /// The  IdentifierNameSyntax is "myDelegate" the "type" is "MyDelegate".
    /// </example>
    private void ConvertDelegateInvocationExpression(SemanticModel model, ExpressionSyntax expression, ArgumentSyntax[] arguments)
    {
        INamedTypeSymbol type = (INamedTypeSymbol)model.GetTypeInfo(expression).Type!;
        PrepareArgumentsForMethod(model, type.DelegateInvokeMethod!, arguments);
        ConvertExpression(model, expression);
        AddInstruction(OpCode.CALLA);
    }
}
