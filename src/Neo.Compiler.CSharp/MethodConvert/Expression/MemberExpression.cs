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
using Neo.VM;
using System;

namespace Neo.Compiler;

partial class MethodConvert
{
    /// <summary>
    /// This method converts a member access expression to OpCodes.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about member access expression.</param>
    /// <param name="expression">The syntax representation of the member access expression statement being converted.</param>
    /// <exception cref="CompilationException">Unsupported symbols will result in a compilation exception, such as non-static methods.</exception>
    /// <remarks>
    /// The method determines the symbol associated with the member access expression from the semantic model.
    /// It then generates OpCodes based on the type of symbol.
    /// Supported symbols include fields, methods, and properties.
    /// For fields, it handles constant fields, static fields, and instance fields.
    /// For methods, it handles static methods.
    /// For properties, it handles accessing static properties and instance properties.
    /// </remarks>
    /// <example>
    /// This is a member access example. The following code branches to "case IPropertySymbol property".
    /// <code>
    /// Runtime.Log(Ledger.CurrentHash.ToString());
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#member-access-expression-">Member access expression</seealso>
    private void ConvertMemberAccessExpression(SemanticModel model, MemberAccessExpressionSyntax expression)
    {
        ISymbol symbol = model.GetSymbolInfo(expression).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                if (field.IsConst)
                {
                    Push(field.ConstantValue);
                }
                else if (field.IsStatic)
                {
                    byte index = _context.AddStaticField(field);
                    AccessSlot(OpCode.LDSFLD, index);
                }
                else
                {
                    int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                    ConvertExpression(model, expression.Expression);
                    Push(index);
                    AddInstruction(OpCode.PICKITEM);
                }
                break;
            case IMethodSymbol method:
                if (!method.IsStatic)
                    throw new CompilationException(expression, DiagnosticId.NonStaticDelegate, $"Unsupported delegate: {method}");
                MethodConvert convert = _context.ConvertMethod(model, method);
                Jump(OpCode.PUSHA, convert._startTarget);
                break;
            case IPropertySymbol property:
                ExpressionSyntax? instanceExpression = property.IsStatic ? null : expression.Expression;
                Call(model, property.GetMethod!, instanceExpression);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }

    /// <summary>
    /// Further conversion of the ?. statement in the <see cref="ConvertConditionalAccessExpression"/> method
    /// </summary>
    /// <param name="model">The semantic model providing context and information about member binding expression.</param>
    /// <param name="expression">The syntax representation of the member binding expression statement being converted.</param>
    /// <exception cref="CompilationException">Only attributes and fields are supported, otherwise an exception is thrown.</exception>
    /// <example>
    /// <code>
    /// public class Person
    /// {
    ///     public string Name;
    ///     public int Age { get; set; }
    /// }
    /// </code>
    /// <code>
    /// Person person = null;
    /// Runtime.Log(person?.Name);
    /// Runtime.Log(person?.Age.ToString());
    /// </code>
    /// <c>person?.Name</c> code executes the <c>case IFieldSymbol field</c> branch;
    /// <c>person?.Age</c> code executes the <c>case IPropertySymbol property</c> branch.
    /// </example>
    private void ConvertMemberBindingExpression(SemanticModel model, MemberBindingExpressionSyntax expression)
    {
        ISymbol symbol = model.GetSymbolInfo(expression).Symbol!;
        switch (symbol)
        {
            case IFieldSymbol field:
                int index = Array.IndexOf(field.ContainingType.GetFields(), field);
                Push(index);
                AddInstruction(OpCode.PICKITEM);
                break;
            case IPropertySymbol property:
                Call(model, property.GetMethod!);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported, $"Unsupported symbol: {symbol}");
        }
    }
}
