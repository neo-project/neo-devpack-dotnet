// Copyright (C) 2015-2026 The Neo Project.
//
// LambdaExpression.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Convert a simple lambda expression to a method call
    /// </summary>
    /// <param name="model">The semantic model of the method</param>
    /// <param name="expression">The lambda expression to convert</param>
    /// <example>
    /// <code>
    /// public void MyMethod()
    /// {
    ///     var lambda = x => x + 1;
    ///     lambda(1);
    /// }
    /// </code>
    /// </example>
    private void ConvertSimpleLambdaExpression(SemanticModel model, SimpleLambdaExpressionSyntax expression)
    {
        var symbol = (IMethodSymbol)model.GetSymbolInfo(expression).Symbol!;
        var mc = _context.ConvertMethod(model, symbol);
        ValidateCapturedLambda(model, expression);
        ConvertLocalToStaticFields(mc);
        InvokeMethod(mc, model.GetTypeInfo(expression).ConvertedType);
    }

    /// <summary>
    /// Convert a parenthesized lambda expression to a method call
    /// </summary>
    /// <param name="model">The semantic model of the method</param>
    /// <param name="expression">The lambda expression to convert</param>
    /// <example>
    /// <code>
    /// public void MyMethod()
    /// {
    ///     var lambda = (int x, int y) => x + y;
    ///     var result = lambda(1, 2);
    ///     Console.WriteLine(result);
    /// }
    /// </code>
    /// </example>
    private void ConvertParenthesizedLambdaExpression(SemanticModel model, ParenthesizedLambdaExpressionSyntax expression)
    {
        var symbol = (IMethodSymbol)model.GetSymbolInfo(expression).Symbol!;
        var mc = _context.ConvertMethod(model, symbol);
        ValidateCapturedLambda(model, expression);
        ConvertLocalToStaticFields(mc);
        InvokeMethod(mc, model.GetTypeInfo(expression).ConvertedType);
    }

    private void ValidateCapturedLambda(SemanticModel model, LambdaExpressionSyntax expression)
    {
        if (!model.AnalyzeDataFlow(expression.Body).Captured.Any(symbol =>
            symbol is ILocalSymbol or IParameterSymbol &&
            SymbolEqualityComparer.Default.Equals(symbol.ContainingSymbol, Symbol)))
            return;

        if (CanCallMethod(model, Symbol, Symbol, new HashSet<IMethodSymbol>(SymbolEqualityComparer.Default)))
            throw new CompilationException(expression, DiagnosticId.CrossActivationCapture,
                "A lambda capturing local variables is not supported in a recursive method. Pass the values explicitly instead.");

        // A delegate is currently just a code pointer. It cannot carry the values
        // from the activation that created it when it leaves that activation.
        SyntaxNode value = expression;
        while (value.Parent is ParenthesizedExpressionSyntax or CastExpressionSyntax)
            value = value.Parent;

        ISymbol? local = value.Parent switch
        {
            EqualsValueClauseSyntax { Parent: VariableDeclaratorSyntax variable } => model.GetDeclaredSymbol(variable),
            AssignmentExpressionSyntax assignment when assignment.Right == value => model.GetSymbolInfo(assignment.Left).Symbol,
            _ => null
        };
        if (local is ILocalSymbol && SyntaxNode is not null)
        {
            foreach (IdentifierNameSyntax use in SyntaxNode.DescendantNodes().OfType<IdentifierNameSyntax>())
            {
                if (!SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(use).Symbol, local))
                    continue;
                SyntaxNode current = use;
                while (current.Parent is ParenthesizedExpressionSyntax or PostfixUnaryExpressionSyntax)
                    current = current.Parent;
                if (current.Parent is InvocationExpressionSyntax invocation && invocation.Expression == current)
                    continue;
                if (current.Parent is AssignmentExpressionSyntax assignment && assignment.Left == current)
                    continue;
                throw new CompilationException(expression, DiagnosticId.CrossActivationCapture,
                    "A captured lambda can only be invoked locally. Pass captured values as explicit parameters before returning or storing the delegate.");
            }
            return;
        }

        if (value.Parent is InvocationExpressionSyntax invocationExpression && invocationExpression.Expression == value)
            return;

        // A local collection can retain the delegate. Reject it when the
        // collection itself is returned, while preserving local loop uses.
        if (value.Parent is ArgumentSyntax
            {
                Parent: ArgumentListSyntax
                {
                    Parent: InvocationExpressionSyntax
                    {
                        Expression: MemberAccessExpressionSyntax
                        { Expression: IdentifierNameSyntax receiver }
                    }
                }
            } &&
            model.GetSymbolInfo(receiver).Symbol is ILocalSymbol collection && SyntaxNode is not null &&
            SyntaxNode.DescendantNodes().OfType<ReturnStatementSyntax>().Any(ret =>
                ret.Expression is IdentifierNameSyntax returned &&
                SymbolEqualityComparer.Default.Equals(model.GetSymbolInfo(returned).Symbol, collection)))
            throw new CompilationException(expression, DiagnosticId.CrossActivationCapture,
                "A collection containing a captured lambda cannot leave its creating method. Pass captured values explicitly.");

        // Calls such as Any(values, x => x > threshold) consume the callback
        // within the current activation and are already part of the supported
        // contract surface. A recursive call path is rejected above.
        if (value.Parent is ArgumentSyntax)
            return;

        throw new CompilationException(expression, DiagnosticId.CrossActivationCapture,
            "A captured lambda cannot escape its creating method. Pass captured values as explicit parameters.");
    }

    private static bool CanCallMethod(SemanticModel model, IMethodSymbol current, IMethodSymbol target,
        HashSet<IMethodSymbol> visited)
    {
        if (!visited.Add(current))
            return false;
        foreach (SyntaxReference reference in current.DeclaringSyntaxReferences)
        {
            SyntaxNode syntax = reference.GetSyntax();
            SemanticModel methodModel = model.Compilation.GetSemanticModel(syntax.SyntaxTree);
            foreach (InvocationExpressionSyntax call in syntax.DescendantNodes(descendIntoChildren: node =>
                node == syntax || node is not LambdaExpressionSyntax).OfType<InvocationExpressionSyntax>())
            {
                if (methodModel.GetSymbolInfo(call).Symbol is not IMethodSymbol callee)
                    continue;
                if (SymbolEqualityComparer.Default.Equals(callee.OriginalDefinition, target.OriginalDefinition))
                    return true;
                if (SymbolEqualityComparer.Default.Equals(callee.ContainingType, target.ContainingType) &&
                    CanCallMethod(model, callee, target, visited))
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Convert captured local variables/parameters to static fields
    /// Assign values of captured local variables/parameters to related static fields
    /// </summary>
    /// <param name="mc">The method convert context</param>
    private void ConvertLocalToStaticFields(MethodConvert mc)
    {
        if (mc.CapturedLocalSymbols.Count <= 0) return;
        foreach (var local in mc.CapturedLocalSymbols)
        {
            // copy captured local variable/parameter value to related static fields
            var staticFieldIndex = _context.GetOrAddCapturedStaticField(local);
            switch (local)
            {
                case ILocalSymbol localSymbol:
                    if (_byRefCapturedSymbols.Contains(localSymbol))
                    {
                        LdLocSlot(localSymbol);
                    }
                    else
                    {
                        var localIndex = _localVariables[localSymbol];
                        AccessSlot(OpCode.LDLOC, localIndex);
                    }
                    break;
                case IParameterSymbol parameterSymbol:
                    if (_byRefCapturedSymbols.Contains(parameterSymbol))
                    {
                        LdArgSlot(parameterSymbol);
                    }
                    else
                    {
                        var paraIndex = _parameters[parameterSymbol];
                        AccessSlot(OpCode.LDARG, paraIndex);
                    }
                    break;
            }
            AccessSlot(OpCode.STSFLD, staticFieldIndex);
        }
    }
}
