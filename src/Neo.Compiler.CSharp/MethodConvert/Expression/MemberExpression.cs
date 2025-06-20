// Copyright (C) 2015-2025 The Neo Project.
//
// MemberExpression.cs file belongs to the neo project and is free
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
using Neo.VM;
using System;

namespace Neo.Compiler;

internal partial class MethodConvert
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
                    // This branch is not covered, is there any c# code that matches the conditions?
                    // Const member field access is handled via ConvertMethodInvocationExpression
                    Push(field.ConstantValue);
                }
                else if (field.IsStatic)
                {
                    // Have to process the string.Empty specially since it has no AssociatedSymbol
                    // thus will return directly without this if check.
                    if (field.ContainingType.ToString() == "string" && field.Name == "Empty")
                    {
                        Push(string.Empty);
                        return;
                    }

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
                //This branch is not covered, is there any c# code that matches the conditions?
                if (!method.IsStatic)
                    throw new CompilationException(expression, DiagnosticId.NonStaticDelegate, $"Unsupported delegate: {method}");
                InvokeMethod(model, method);
                break;
            case IPropertySymbol property:
                ExpressionSyntax? instanceExpression = property.IsStatic ? null : expression.Expression;
                CallMethodWithInstanceExpression(model, property.GetMethod!, instanceExpression);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(expression, $"Unsupported member access '{symbol.Name}' of type '{symbol.GetType().Name}'. Use supported members: fields, properties, constants, or enum values.");
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
                CallMethodWithConvention(model, property.GetMethod!);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(expression, $"Unsupported member access '{symbol.Name}' of type '{symbol.GetType().Name}'. Use supported members: fields, properties, constants, or enum values.");
        }
    }
}
