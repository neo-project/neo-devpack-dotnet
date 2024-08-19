// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;

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
        ConvertLocalToStaticFields(mc);
        InvokeMethod(mc);
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
        ConvertLocalToStaticFields(mc);
        InvokeMethod(mc);
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
                    var localIndex = _localVariables[localSymbol];
                    AccessSlot(OpCode.LDLOC, localIndex);
                    break;
                case IParameterSymbol parameterSymbol:
                    var paraIndex = _parameters[parameterSymbol];
                    AccessSlot(OpCode.LDARG, paraIndex);
                    break;
            }
            AccessSlot(OpCode.STSFLD, staticFieldIndex);
        }
    }
}
