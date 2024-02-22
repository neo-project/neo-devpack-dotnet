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
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.SmartContract.Native;
using Neo.VM;
using System.Numerics;

namespace Neo.Compiler;

partial class MethodConvert
{
    private void ConvertExpression(SemanticModel model, ExpressionSyntax syntax)
    {
        Optional<object?> constant = model.GetConstantValue(syntax);
        if (constant.HasValue)
        {
            Push(constant.Value);
            return;
        }
        switch (syntax)
        {
            case AnonymousObjectCreationExpressionSyntax expression:
                ConvertAnonymousObjectCreationExpression(model, expression);
                break;
            case ArrayCreationExpressionSyntax expression:
                ConvertArrayCreationExpression(model, expression);
                break;
            case AssignmentExpressionSyntax expression:
                ConvertAssignmentExpression(model, expression);
                break;
            case BaseObjectCreationExpressionSyntax expression:
                ConvertObjectCreationExpression(model, expression);
                break;
            case BinaryExpressionSyntax expression:
                ConvertBinaryExpression(model, expression);
                break;
            case CastExpressionSyntax expression:
                ConvertCastExpression(model, expression);
                break;
            case CheckedExpressionSyntax expression:
                ConvertCheckedExpression(model, expression);
                break;
            case ConditionalAccessExpressionSyntax expression:
                ConvertConditionalAccessExpression(model, expression);
                break;
            case ConditionalExpressionSyntax expression:
                ConvertConditionalExpression(model, expression);
                break;
            case ElementAccessExpressionSyntax expression:
                ConvertElementAccessExpression(model, expression);
                break;
            case ElementBindingExpressionSyntax expression:
                ConvertElementBindingExpression(model, expression);
                break;
            case IdentifierNameSyntax expression:
                ConvertIdentifierNameExpression(model, expression);
                break;
            case ImplicitArrayCreationExpressionSyntax expression:
                ConvertImplicitArrayCreationExpression(model, expression);
                break;
            case InitializerExpressionSyntax expression:
                ConvertInitializerExpression(model, expression);
                break;
            case InterpolatedStringExpressionSyntax expression:
                ConvertInterpolatedStringExpression(model, expression);
                break;
            case InvocationExpressionSyntax expression:
                ConvertInvocationExpression(model, expression);
                break;
            case IsPatternExpressionSyntax expression:
                ConvertIsPatternExpression(model, expression);
                break;
            case MemberAccessExpressionSyntax expression:
                ConvertMemberAccessExpression(model, expression);
                break;
            case MemberBindingExpressionSyntax expression:
                ConvertMemberBindingExpression(model, expression);
                break;
            case ParenthesizedExpressionSyntax expression:
                ConvertExpression(model, expression.Expression);
                break;
            case PostfixUnaryExpressionSyntax expression:
                ConvertPostfixUnaryExpression(model, expression);
                break;
            case PrefixUnaryExpressionSyntax expression:
                ConvertPrefixUnaryExpression(model, expression);
                break;
            case SwitchExpressionSyntax expression:
                ConvertSwitchExpression(model, expression);
                break;
            case BaseExpressionSyntax:
            case ThisExpressionSyntax:
                AddInstruction(OpCode.LDARG0);
                break;
            case ThrowExpressionSyntax expression:
                Throw(model, expression.Expression);
                break;
            case TupleExpressionSyntax expression:
                ConvertTupleExpression(model, expression);
                break;
            default:
                throw new CompilationException(syntax, DiagnosticId.SyntaxNotSupported, $"Unsupported syntax: {syntax}");
        }
    }

    private void EnsureIntegerInRange(ITypeSymbol type)
    {
        if (type.Name == "BigInteger") return;
        var (minValue, maxValue, mask) = type.Name switch
        {
            "SByte" => ((BigInteger)sbyte.MinValue, (BigInteger)sbyte.MaxValue, (BigInteger)0xff),
            "Int16" => (short.MinValue, short.MaxValue, 0xffff),
            "Int32" => (int.MinValue, int.MaxValue, 0xffffffff),
            "Int64" => (long.MinValue, long.MaxValue, 0xffffffffffffffff),
            "Byte" => (byte.MinValue, byte.MaxValue, 0xff),
            "UInt16" => (ushort.MinValue, ushort.MaxValue, 0xffff),
            "UInt32" => (uint.MinValue, uint.MaxValue, 0xffffffff),
            "UInt64" => (ulong.MinValue, ulong.MaxValue, 0xffffffffffffffff),
            _ => throw new CompilationException(DiagnosticId.SyntaxNotSupported, $"Unsupported type: {type}")
        };
        JumpTarget checkUpperBoundTarget = new(), adjustTarget = new(), endTarget = new();
        AddInstruction(OpCode.DUP);
        Push(minValue);
        Jump(OpCode.JMPGE_L, checkUpperBoundTarget);
        if (_checkedStack.Peek())
            AddInstruction(OpCode.THROW);
        else
            Jump(OpCode.JMP_L, adjustTarget);
        checkUpperBoundTarget.Instruction = AddInstruction(OpCode.DUP);
        Push(maxValue);
        Jump(OpCode.JMPLE_L, endTarget);
        if (_checkedStack.Peek())
        {
            AddInstruction(OpCode.THROW);
        }
        else
        {
            adjustTarget.Instruction = Push(mask);
            AddInstruction(OpCode.AND);
            if (minValue < 0)
            {
                AddInstruction(OpCode.DUP);
                Push(maxValue);
                Jump(OpCode.JMPLE_L, endTarget);
                Push(mask + 1);
                AddInstruction(OpCode.SUB);
            }
        }
        endTarget.Instruction = AddInstruction(OpCode.NOP);
    }

    private void ConvertObjectToString(SemanticModel model, ExpressionSyntax expression)
    {
        ITypeSymbol? type = model.GetTypeInfo(expression).Type;
        switch (type?.ToString())
        {
            case "sbyte":
            case "byte":
            case "short":
            case "ushort":
            case "int":
            case "uint":
            case "long":
            case "ulong":
            case "System.Numerics.BigInteger":
                ConvertExpression(model, expression);
                Call(NativeContract.StdLib.Hash, "itoa", 1, true);
                break;
            case "string":
            case "Neo.Cryptography.ECC.ECPoint":
            case "Neo.SmartContract.Framework.ByteString":
            case "Neo.SmartContract.Framework.UInt160":
            case "Neo.SmartContract.Framework.UInt256":
                ConvertExpression(model, expression);
                break;
            default:
                throw new CompilationException(expression, DiagnosticId.InvalidToStringType, $"Unsupported interpolation: {expression}");
        }
    }
}
