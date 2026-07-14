// Copyright (C) 2015-2026 The Neo Project.
//
// ElementExpression.cs file belongs to the neo project and is free
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
using System.Linq;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// This method converts an array element or indexer access ([]) expression to OpCodes.
    /// An array element or indexer access ([]) expression accesses a single element in an array or a collection.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about element access expression.</param>
    /// <param name="expression">The syntax representation of the element access expression statement being converted.</param>
    /// <remarks>
    /// If the accessed element is a property, the method calls the property's getter.
    /// Arrays of any rank are supported: the compiler drills through each index and emits the necessary PICKITEM calls.
    /// </remarks>
    /// <example>
    /// The following example demonstrates how to access array elements:
    /// <code>
    /// var array = new byte[8];
    /// var sum = 0;
    /// for (var i = 0; i< 8; i++)
    /// {
    ///     sum += array[i];
    /// }
    /// Runtime.Log(sum.ToString());
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/member-access-operators#indexer-operator-">Indexer operator []</seealso>
    private void ConvertElementAccessExpression(SemanticModel model, ElementAccessExpressionSyntax expression)
    {
        if (model.GetSymbolInfo(expression).Symbol is IPropertySymbol property)
        {
            CallMethodWithInstanceExpression(model, property.GetMethod!, expression.Expression, expression.ArgumentList.Arguments.ToArray());
        }
        else
        {
            ITypeSymbol arrayExpressionType = model.GetTypeInfo(expression.Expression).Type!;
            if (arrayExpressionType is IArrayTypeSymbol { Rank: > 1 } arrayType)
            {
                EnsureMultiDimensionalArguments(expression.ArgumentList.Arguments, arrayType.Rank, expression.ArgumentList);
                ConvertExpression(model, expression.Expression);
                EmitArrayDimensionNavigation(model, expression.ArgumentList.Arguments, arrayType.Rank);
            }
            else
            {
                if (expression.ArgumentList.Arguments.Count != 1)
                    throw new CompilationException(expression.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {expression.ArgumentList.Arguments}");
                ITypeSymbol type = model.GetTypeInfo(expression).Type!;
                ConvertExpression(model, expression.Expression);
                ConvertIndexOrRange(model, type, expression.ArgumentList.Arguments[0].Expression);
            }
        }
    }

    /// <summary>
    /// Further conversion of the ?[] statement in the <see cref="ConvertConditionalAccessExpression"/> method
    /// </summary>
    /// <param name="model">The semantic model providing context and information about element binding expression.</param>
    /// <param name="expression">The syntax representation of the element binding expression statement being converted.</param>
    /// <example>
    /// <code>
    /// var a = Ledger.GetBlock(10000);
    /// var b = Ledger.GetBlock(10001);
    /// var array = new[] { a, b };
    /// var firstItem = array?[0];
    /// Runtime.Log(firstItem?.Timestamp.ToString());
    /// </code>
    /// </example>
    private void ConvertElementBindingExpression(SemanticModel model, ElementBindingExpressionSyntax expression)
    {
        IArrayTypeSymbol? arrayType = expression.Parent is ConditionalAccessExpressionSyntax parent
            ? model.GetTypeInfo(parent.Expression).Type as IArrayTypeSymbol
            : null;
        if (arrayType is not null && arrayType.Rank > 1)
        {
            EnsureMultiDimensionalArguments(expression.ArgumentList.Arguments, arrayType.Rank, expression.ArgumentList);
            EmitArrayDimensionNavigation(model, expression.ArgumentList.Arguments, arrayType.Rank);
        }
        else
        {
            if (expression.ArgumentList.Arguments.Count != 1)
                throw new CompilationException(expression.ArgumentList, DiagnosticId.MultidimensionalArray, $"Unsupported array rank: {expression.ArgumentList.Arguments}");
            ITypeSymbol type = model.GetTypeInfo(expression).Type!;
            ConvertIndexOrRange(model, type, expression.ArgumentList.Arguments[0].Expression);
        }
    }

    /// <summary>
    /// This method converts an index or range expression to OpCodes.
    /// An index or range expression specifies the index or range of elements to access in an array or a collection.
    /// </summary>
    /// <param name="model">The semantic model providing contextual information for the expression.</param>
    /// <param name="type">The type symbol of the array or collection being accessed.</param>
    /// <param name="indexOrRange">The expression representing the index or range.</param>
    /// <exception cref="CompilationException">Only byte[] and string type support range access,
    /// otherwise, an exception is thrown. For examples.
    /// <code>
    /// int[] oneThroughTen = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    /// var a = oneThroughTen[..];
    /// </code>
    /// </exception>
    /// <example>
    /// The following code is an example of an index or range selector in a collective:
    /// <code>
    /// byte[] oneThroughTen = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    /// var a = oneThroughTen[..];
    /// var b = oneThroughTen[..3];
    /// var c = oneThroughTen[2..];
    /// var d = oneThroughTen[3..5];
    /// var e = oneThroughTen[^2..];
    /// var f = oneThroughTen[..^3];
    /// var g = oneThroughTen[3..^4];
    /// var h = oneThroughTen[^4..^2];
    /// var i = oneThroughTen[0];
    /// </code>
    /// result:
    /// a: 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
    /// b: 1, 2, 3
    /// c: 3, 4, 5, 6, 7, 8, 9, 10
    /// d: 4, 5
    /// e: 9, 10
    /// f: 1, 2, 3, 4, 5, 6, 7
    /// g: 4, 5, 6
    /// h: 7, 8
    /// i: 1
    /// </example>
    /// <remarks>
    /// If the expression is a range, it calculates the start and end indices and extracts the relevant sub-array or sub-collection.
    /// If the expression is an index, it generates OpCodes to fetch the element at the specified index.
    /// </remarks>
    private void ConvertIndexOrRange(SemanticModel model, ITypeSymbol type, ExpressionSyntax indexOrRange)
    {
        if (indexOrRange is RangeExpressionSyntax range)
        {
            if (range.LeftOperand is null)
                Push(0);
            else
                ConvertRangeEndpointValue(model, range.LeftOperand);

            if (range.RightOperand is null)
            {
                AddInstruction(OpCode.OVER);
                AddInstruction(OpCode.SIZE);
            }
            else
            {
                ConvertRangeEndpointValue(model, range.RightOperand);
            }

            // Decode from-end values only after both endpoint expressions have run.
            ResolveRangeEndpointOffset();
            AddInstruction(OpCode.SWAP);
            ResolveRangeEndpointOffset();
            AddInstruction(OpCode.SWAP);

            AddInstruction(OpCode.OVER);
            AddInstruction(OpCode.SUB);
            switch (type.ToString())
            {
                case "byte[]":
                    AddInstruction(OpCode.SUBSTR);
                    break;
                case "string":
                    AddInstruction(OpCode.SUBSTR);
                    ChangeType(VM.Types.StackItemType.ByteString);
                    break;
                default:
                    throw new CompilationException(indexOrRange, DiagnosticId.ArrayRange, $"The type {type} does not support range access.");
            }
        }
        else
        {
            ConvertExpression(model, indexOrRange);
            AddInstruction(OpCode.PICKITEM);
        }
    }

    private void ConvertRangeEndpointValue(SemanticModel model, ExpressionSyntax endpoint)
    {
        ExpressionSyntax value = endpoint;
        while (value is ParenthesizedExpressionSyntax parenthesized)
            value = parenthesized.Expression;

        if (value is ThrowExpressionSyntax)
        {
            ConvertExpression(model, value);
            return;
        }

        TypeInfo typeInfo = model.GetTypeInfo(value);
        if (IsSystemIndexType(typeInfo.ConvertedType) &&
            !IsSystemIndexType(typeInfo.Type) &&
            !IsSupportedRangeIndexConversion(model.GetConversion(value)))
            ThrowUnsupportedRangeIndexExpression(value);

        switch (value)
        {
            case PrefixUnaryExpressionSyntax prefix when prefix.OperatorToken.IsKind(SyntaxKind.CaretToken):
                ConvertExpression(model, prefix.Operand);
                ValidateRangeEndpointValue();
                // Encode ^n as -(n + 1), reserving nonnegative values for from-start indices.
                AddInstruction(OpCode.INC);
                AddInstruction(OpCode.NEGATE);
                return;
            case CastExpressionSyntax cast when
                IsSystemIndexType(model.GetTypeInfo(cast.Type).Type) &&
                IsSystemIndexType(model.GetTypeInfo(cast.Expression).Type):
                ConvertRangeEndpointValue(model, cast.Expression);
                return;
            case PostfixUnaryExpressionSyntax postfix when
                postfix.OperatorToken.IsKind(SyntaxKind.ExclamationToken):
                ConvertRangeEndpointValue(model, postfix.Operand);
                return;
            case ConditionalExpressionSyntax conditional:
                ConvertRangeConditionalEndpoint(model, conditional);
                return;
            case CheckedExpressionSyntax checkedExpression:
                _checkedStack.Push(checkedExpression.Keyword.IsKind(SyntaxKind.CheckedKeyword));
                try
                {
                    ConvertRangeEndpointValue(model, checkedExpression.Expression);
                }
                finally
                {
                    _checkedStack.Pop();
                }
                return;
            case SwitchExpressionSyntax switchExpression:
                ConvertRangeSwitchEndpoint(model, switchExpression);
                return;
        }

        if (IsSystemIndexType(typeInfo.Type))
            ThrowUnsupportedRangeIndexExpression(value);

        ConvertExpression(model, value);
        ValidateRangeEndpointValue();
    }

    private static bool IsSystemIndexType(ITypeSymbol? type)
        => type is INamedTypeSymbol { Name: "Index" } && type.ContainingNamespace.ToDisplayString() == "System";

    private static bool IsSupportedRangeIndexConversion(Conversion conversion)
    {
        IMethodSymbol? method = conversion.MethodSymbol;
        return conversion.IsImplicit && conversion.IsUserDefined &&
            method is not null && method.Name == "op_Implicit" &&
            method.Parameters.Length == 1 &&
            method.Parameters[0].Type.SpecialType == SpecialType.System_Int32 &&
            IsSystemIndexType(method.ContainingType) &&
            IsSystemIndexType(method.ReturnType);
    }

    private static void ThrowUnsupportedRangeIndexExpression(ExpressionSyntax expression)
        => throw new CompilationException(expression, DiagnosticId.SyntaxNotSupported,
            "This System.Index expression cannot be lowered safely. Use an int endpoint or inline the ^ expression in the range.");

    private void ConvertRangeConditionalEndpoint(SemanticModel model, ConditionalExpressionSyntax expression)
    {
        JumpTarget falseTarget = new();
        JumpTarget endTarget = new();
        ConvertExpression(model, expression.Condition);
        Jump(OpCode.JMPIFNOT_L, falseTarget);
        ConvertRangeEndpointValue(model, expression.WhenTrue);
        Jump(OpCode.JMP_L, endTarget);
        falseTarget.Instruction = AddInstruction(OpCode.NOP);
        ConvertRangeEndpointValue(model, expression.WhenFalse);
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertRangeSwitchEndpoint(SemanticModel model, SwitchExpressionSyntax expression)
    {
        var arms = expression.Arms.Select(p => (p, new JumpTarget())).ToArray();
        JumpTarget breakTarget = new();
        byte anonymousIndex = AddAnonymousVariable();
        ConvertExpression(model, expression.GoverningExpression);
        AccessSlot(OpCode.STLOC, anonymousIndex);
        foreach (var (arm, nextTarget) in arms)
        {
            ConvertPattern(model, arm.Pattern, anonymousIndex);
            Jump(OpCode.JMPIFNOT_L, nextTarget);
            if (arm.WhenClause is not null)
            {
                ConvertExpression(model, arm.WhenClause.Condition);
                Jump(OpCode.JMPIFNOT_L, nextTarget);
            }
            ConvertRangeEndpointValue(model, arm.Expression);
            Jump(OpCode.JMP_L, breakTarget);
            nextTarget.Instruction = AddInstruction(OpCode.NOP);
        }
        Push("No switch arm matched.");
        AddInstruction(OpCode.THROW);
        breakTarget.Instruction = AddInstruction(OpCode.NOP);
        RemoveAnonymousVariable(anonymousIndex);
    }

    private void ValidateRangeEndpointValue()
    {
        JumpTarget nonNegativeTarget = new();
        AddInstruction(OpCode.DUP);
        Push(0);
        JumpIfGreaterOrEqual(nonNegativeTarget);
        Throw();
        nonNegativeTarget.Instruction = Nop();
    }

    private void ResolveRangeEndpointOffset()
    {
        JumpTarget absoluteTarget = new();
        AddInstruction(OpCode.DUP);
        Push(0);
        JumpIfGreaterOrEqual(absoluteTarget);
        Push(2);
        AddInstruction(OpCode.PICK);
        AddInstruction(OpCode.SIZE);
        AddInstruction(OpCode.SWAP);
        AddInstruction(OpCode.ADD);
        AddInstruction(OpCode.INC);
        absoluteTarget.Instruction = Nop();
    }

    private void EnsureMultiDimensionalArguments(SeparatedSyntaxList<ArgumentSyntax> arguments, int expectedRank, SyntaxNode errorNode)
    {
        if (arguments.Count != expectedRank)
            throw new CompilationException(errorNode, DiagnosticId.MultidimensionalArray, $"Expected {expectedRank} indices for multi-dimensional array access, but found {arguments.Count}.");

        foreach (ArgumentSyntax argument in arguments)
        {
            if (argument.Expression is RangeExpressionSyntax)
                throw new CompilationException(argument.Expression, DiagnosticId.MultidimensionalArray, "Range expressions are not supported for multi-dimensional arrays.");
        }
    }

    private void EmitArrayDimensionNavigation(SemanticModel model, SeparatedSyntaxList<ArgumentSyntax> arguments, int dimensionsToTraverse)
    {
        for (int i = 0; i < dimensionsToTraverse; i++)
        {
            ConvertExpression(model, arguments[i].Expression);
            AddInstruction(OpCode.PICKITEM);
        }
    }
}
