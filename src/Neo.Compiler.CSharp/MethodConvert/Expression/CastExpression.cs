// Copyright (C) 2015-2026 The Neo Project.
//
// CastExpression.cs file belongs to the neo project and is free
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

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// This method converts a cast expression to OpCodes.
    /// A cast expression of the form (T)E performs an explicit conversion of the result of expression E to type T.
    /// If no explicit conversion exists from the type of E to type T, a compile-time error occurs.
    /// </summary>
    /// <param name="model">The semantic model providing context and information about cast expression.</param>
    /// <param name="expression">The syntax representation of the cast expression statement being converted.</param>
    /// <remarks>
    /// This method determines the source type and the target type of the cast expression.
    /// If the cast can be resolved to a method symbol, it calls the corresponding method.
    /// Otherwise, it generates OpCodes based on the types involved in the cast operation.
    /// </remarks>
    /// <example>
    /// This code is cast a ByteString type to an ECPoint type,
    /// where the source type is ByteString and the target type is ECPoint.
    /// <code>
    /// ByteString bytes = ByteString.Empty;
    /// ECPoint point = (ECPoint)bytes;
    /// Runtime.Log(point.ToString());
    /// </code>
    /// </example>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/type-testing-and-cast#cast-expression">Cast expression</seealso>
    private void ConvertCastExpression(SemanticModel model, CastExpressionSyntax expression)
    {
        ITypeSymbol sType = model.GetTypeInfo(expression.Expression).Type!;
        ITypeSymbol tType = model.GetTypeInfo(expression.Type).Type!;
        if (IsByteArrayToByteStringCast(sType, tType) && TryConvertConstantByteArrayToByteString(model, expression.Expression))
            return;

        IMethodSymbol? method = model.GetSymbolInfo(expression).Symbol as IMethodSymbol
            ?? (sType is not null ? model.ClassifyConversion(expression.Expression, tType).MethodSymbol : null);
        if (method is not null)
        {
            ConvertMethodBackedCast(model, expression.Expression, sType, tType, method);
            return;
        }

        // Handle null literal expressions: (T)null
        if (expression.Expression is LiteralExpressionSyntax literal)
        {
            if (literal.Kind() == SyntaxKind.NullLiteralExpression)
            {
                AddInstruction(OpCode.PUSHNULL);
                return;
            }
        }

        ConvertExpression(model, expression.Expression);

        JumpTarget? nullableEnd = null;
        if (IsNullableValueType(sType) && tType.IsValueType)
        {
            AddInstruction(OpCode.DUP);
            AddInstruction(OpCode.ISNULL);
            if (IsNullableValueType(tType))
            {
                nullableEnd = new JumpTarget();
                Jump(OpCode.JMPIF_L, nullableEnd);
            }
            else
            {
                JumpTarget nonNull = new();
                Jump(OpCode.JMPIFNOT_L, nonNull);
                Throw();
                nonNull.Instruction = AddInstruction(OpCode.NOP);
            }
        }

        sType = GetNonNullableValueType(sType!);
        tType = GetNonNullableValueType(tType);
        if (sType is INamedTypeSymbol { TypeKind: TypeKind.Enum, EnumUnderlyingType: not null } sEnum)
            sType = sEnum.EnumUnderlyingType;
        if (tType is INamedTypeSymbol { TypeKind: TypeKind.Enum, EnumUnderlyingType: not null } tEnum)
            tType = tEnum.EnumUnderlyingType;
        switch ((sType.Name, tType.Name))
        {
            case ("ByteString", "ECPoint"):
                {
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    Jump(OpCode.JMPIF_L, endTarget);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.SIZE);
                    Push(33);
                    Jump(OpCode.JMPEQ_L, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                break;
            case ("ByteString", "UInt160"):
                {
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    Jump(OpCode.JMPIF_L, endTarget);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.SIZE);
                    Push(20);
                    Jump(OpCode.JMPEQ_L, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                break;
            case ("ByteString", "UInt256"):
                {
                    JumpTarget endTarget = new();
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.ISNULL);
                    Jump(OpCode.JMPIF_L, endTarget);
                    AddInstruction(OpCode.DUP);
                    AddInstruction(OpCode.SIZE);
                    Push(32);
                    Jump(OpCode.JMPEQ_L, endTarget);
                    AddInstruction(OpCode.THROW);
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                }
                break;
            case ("SByte", "Byte"):
            case ("SByte", "UInt16"):
            case ("SByte", "UInt32"):
            case ("SByte", "UInt64"):
            case ("Int16", "SByte"):
            case ("Int16", "Byte"):
            case ("Int16", "UInt16"):
            case ("Int16", "UInt32"):
            case ("Int16", "UInt64"):
            case ("Int32", "SByte"):
            case ("Int32", "Int16"):
            case ("Int32", "Byte"):
            case ("Int32", "UInt16"):
            case ("Int32", "UInt32"):
            case ("Int32", "UInt64"):
            case ("Int64", "SByte"):
            case ("Int64", "Int16"):
            case ("Int64", "Int32"):
            case ("Int64", "Byte"):
            case ("Int64", "UInt16"):
            case ("Int64", "UInt32"):
            case ("Int64", "UInt64"):
            case ("Byte", "SByte"):
            case ("UInt16", "SByte"):
            case ("UInt16", "Int16"):
            case ("UInt16", "Byte"):
            case ("UInt32", "SByte"):
            case ("UInt32", "Int16"):
            case ("UInt32", "Int32"):
            case ("UInt32", "Byte"):
            case ("UInt32", "UInt16"):
            case ("UInt64", "SByte"):
            case ("UInt64", "Int16"):
            case ("UInt64", "Int32"):
            case ("UInt64", "Int64"):
            case ("UInt64", "Byte"):
            case ("UInt64", "UInt16"):
            case ("UInt64", "UInt32"):
            case ("Char", "SByte"):
            case ("Char", "Byte"):
            case ("Char", "Int16"):
            case ("Char", "UInt16"):
            case ("Char", "Int32"):
            case ("Char", "UInt32"):
            case ("Char", "Int64"):
            case ("Char", "UInt64"):
            case ("SByte", "Char"):
            case ("Byte", "Char"):
            case ("Int16", "Char"):
            case ("UInt16", "Char"):
            case ("Int32", "Char"):
            case ("UInt32", "Char"):
            case ("Int64", "Char"):
            case ("UInt64", "Char"):
                {
                    EnsureIntegerInRange(tType);
                }
                break;
        }

        if (nullableEnd is not null)
            nullableEnd.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertMethodBackedCast(SemanticModel model, ExpressionSyntax operand,
        ITypeSymbol? sourceType, ITypeSymbol targetType, IMethodSymbol method)
    {
        if (method.Parameters.Length != 1)
        {
            CallMethodWithInstanceExpression(model, method, null, operand);
            return;
        }

        ITypeSymbol parameterType = method.Parameters[0].Type;
        ITypeSymbol returnType = method.ReturnType;
        bool unwrapSource = sourceType is not null && IsNullableValueType(sourceType)
            && parameterType.IsValueType && !IsNullableValueType(parameterType);
        bool checkParameterRange = RequiresCastIntegerRangeCheck(sourceType, parameterType);
        bool unwrapResult = IsNullableValueType(returnType)
            && targetType.IsValueType && !IsNullableValueType(targetType);
        bool checkResultRange = RequiresCastIntegerRangeCheck(returnType, targetType);

        if (!unwrapSource && !checkParameterRange && !unwrapResult && !checkResultRange)
        {
            CallMethodWithInstanceExpression(model, method, null, operand);
            return;
        }

        ConvertExpression(model, operand);
        JumpTarget? nullableEnd = unwrapSource ? GuardNullableCastValue(targetType) : null;

        if (checkParameterRange)
            EnsureCastIntegerInRange(sourceType, parameterType);

        CallMethodWithPreEvaluatedOperand(model, method, operand);

        if (unwrapResult)
            GuardNullableCastValue(targetType);
        if (checkResultRange)
            EnsureCastIntegerInRange(returnType, targetType);

        if (nullableEnd is not null)
            nullableEnd.Instruction = AddInstruction(OpCode.NOP);
    }

    private JumpTarget? GuardNullableCastValue(ITypeSymbol targetType)
    {
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ISNULL);
        if (IsNullableValueType(targetType) || targetType.IsReferenceType)
        {
            JumpTarget nullableEnd = new();
            Jump(OpCode.JMPIF_L, nullableEnd);
            return nullableEnd;
        }

        JumpTarget nonNull = new();
        Jump(OpCode.JMPIFNOT_L, nonNull);
        Throw();
        nonNull.Instruction = AddInstruction(OpCode.NOP);
        return null;
    }

    private void EnsureCastIntegerInRange(ITypeSymbol? sourceType, ITypeSymbol targetType)
    {
        ITypeSymbol nonNullTargetType = GetNonNullableValueType(targetType);
        if (sourceType is null || !IsNullableValueType(sourceType) || !IsNullableValueType(targetType))
        {
            EnsureIntegerInRange(nonNullTargetType);
            return;
        }

        JumpTarget nullValue = new();
        AddInstruction(OpCode.DUP);
        AddInstruction(OpCode.ISNULL);
        Jump(OpCode.JMPIF_L, nullValue);
        EnsureIntegerInRange(nonNullTargetType);
        nullValue.Instruction = AddInstruction(OpCode.NOP);
    }

    private void CallMethodWithPreEvaluatedOperand(SemanticModel model, IMethodSymbol method,
        ExpressionSyntax operand)
    {
        byte operandSlot = AddAnonymousVariable();
        AccessSlot(OpCode.STLOC, operandSlot);
        var previousInstanceExpression = _preEvaluatedInstanceExpression;
        var previousInstanceSlot = _preEvaluatedInstanceSlot;
        _preEvaluatedInstanceExpression = operand;
        _preEvaluatedInstanceSlot = operandSlot;
        try
        {
            CallMethodWithInstanceExpression(model, method, null, operand);
        }
        finally
        {
            _preEvaluatedInstanceExpression = previousInstanceExpression;
            _preEvaluatedInstanceSlot = previousInstanceSlot;
            RemoveAnonymousVariable(operandSlot);
        }
    }

    private static bool RequiresCastIntegerRangeCheck(ITypeSymbol? sourceType, ITypeSymbol? targetType)
    {
        if (sourceType is null || targetType is null)
            return false;

        sourceType = GetNonNullableValueType(sourceType);
        targetType = GetNonNullableValueType(targetType);
        if (sourceType is INamedTypeSymbol { TypeKind: TypeKind.Enum, EnumUnderlyingType: not null } sourceEnum)
            sourceType = sourceEnum.EnumUnderlyingType;
        if (targetType is INamedTypeSymbol { TypeKind: TypeKind.Enum, EnumUnderlyingType: not null } targetEnum)
            targetType = targetEnum.EnumUnderlyingType;

        return (sourceType.Name, targetType.Name) switch
        {
            ("SByte", "Byte" or "UInt16" or "UInt32" or "UInt64" or "Char") => true,
            ("Int16", "SByte" or "Byte" or "UInt16" or "UInt32" or "UInt64" or "Char") => true,
            ("Int32", "SByte" or "Int16" or "Byte" or "UInt16" or "UInt32" or "UInt64" or "Char") => true,
            ("Int64", "SByte" or "Int16" or "Int32" or "Byte" or "UInt16" or "UInt32" or "UInt64" or "Char") => true,
            ("Byte", "SByte" or "Char") => true,
            ("UInt16", "SByte" or "Int16" or "Byte" or "Char") => true,
            ("UInt32", "SByte" or "Int16" or "Int32" or "Byte" or "UInt16" or "Char") => true,
            ("UInt64", "SByte" or "Int16" or "Int32" or "Int64" or "Byte" or "UInt16" or "UInt32" or "Char") => true,
            ("Char", "SByte" or "Byte" or "Int16" or "UInt16" or "Int32" or "UInt32" or "Int64" or "UInt64") => true,
            _ => false
        };
    }

    private bool TryConvertConstantByteArrayToByteString(SemanticModel model, ExpressionSyntax expression)
    {
        while (expression is ParenthesizedExpressionSyntax parenthesizedExpression)
            expression = parenthesizedExpression.Expression;

        InitializerExpressionSyntax? initializer = expression switch
        {
            ArrayCreationExpressionSyntax { Initializer: { } arrayInitializer } => arrayInitializer,
            ImplicitArrayCreationExpressionSyntax { Initializer: { } implicitInitializer } => implicitInitializer,
            _ => null
        };

        if (initializer is null)
            return false;

        byte[] data = new byte[initializer.Expressions.Count];
        for (int i = 0; i < initializer.Expressions.Count; i++)
        {
            Optional<object?> value = model.GetConstantValue(initializer.Expressions[i]);
            if (!value.HasValue || value.Value is null)
                return false;

            data[i] = (byte)System.Convert.ChangeType(value.Value, typeof(byte))!;
        }

        Push(data);
        return true;
    }

    private static bool IsByteArrayToByteStringCast(ITypeSymbol sourceType, ITypeSymbol targetType)
    {
        return sourceType is IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_Byte }
            && targetType.ToString() == "Neo.SmartContract.Framework.ByteString";
    }
}
