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
using Neo.Cryptography.ECC;
using Neo.IO;
using Neo.Wallets;

namespace Neo.Compiler;

partial class MethodConvert
{
    private void ConvertExpression(SemanticModel model, ExpressionSyntax syntax, SyntaxNode? syntaxNode = null)
    {
        using var sequence = InsertSequencePoint(syntax);

        Optional<object?> constant = model.GetConstantValue(syntax);
        if (constant.HasValue)
        {
            var value = constant.Value;

            ITypeSymbol? typeSymbol = null;
            if (syntaxNode is VariableDeclaratorSyntax variableDeclarator)
            {
                var declaration = variableDeclarator.Parent as VariableDeclarationSyntax;
                if (declaration != null)
                {
                    typeSymbol = ModelExtensions.GetTypeInfo(model, declaration.Type).Type;
                }
            }
            else if (syntaxNode is PropertyDeclarationSyntax propertyDeclaration)
            {
                typeSymbol = ModelExtensions.GetTypeInfo(model, propertyDeclaration.Type).Type;
            }

            if (typeSymbol != null)
            {
                string fullName = typeSymbol.ToDisplayString();

                switch (fullName)
                {
                    case "Neo.SmartContract.Framework.UInt160":
                        var strValue = (string)value;
                        value = (UInt160.TryParse(strValue, out var hash)
                            ? hash
                            : strValue.ToScriptHash(_context.Options.AddressVersion)).ToArray();
                        break;
                    case "Neo.SmartContract.Framework.UInt256":
                        strValue = (string)value;
                        value = strValue.HexToBytes(true);
                        if (((byte[])value).Length != 32)
                            throw new CompilationException(syntax, DiagnosticId.InvalidInitialValue, "Invalid UInt256 literal");
                        break;
                    case "Neo.SmartContract.Framework.ECPoint":
                        strValue = (string)value;
                        value = ECPoint.Parse(strValue, ECCurve.Secp256r1).EncodePoint(true);
                        break;
                    case "Neo.SmartContract.Framework.ByteArray":
                        strValue = (string)value;
                        value = strValue.HexToBytes(true);
                        break;
                }
            }

            Push(value);
            return;
        }

        switch (syntax)
        {
            //Convert an expression that creates an object of an anonymous type.
            //Example: new { Amount = 108, Message = "Hello" };
            case AnonymousObjectCreationExpressionSyntax expression:
                ConvertAnonymousObjectCreationExpression(model, expression);
                break;
            //Convert an expression that creates an array.
            //Example: new int[4] { 5, 6, 7, 8};
            case ArrayCreationExpressionSyntax expression:
                ConvertArrayCreationExpression(model, expression);
                break;
            //Convert an assignment expression. 
            //Example: new int[4] { 5, 6, 7, 8};
            case AssignmentExpressionSyntax expression:
                ConvertAssignmentExpression(model, expression);
                break;
            case BaseObjectCreationExpressionSyntax expression:
                ConvertObjectCreationExpression(model, expression);
                break;
            case BinaryExpressionSyntax expression:
                ConvertBinaryExpression(model, expression);
                break;
            //Converts a cast expression.
            //Example: (ECPoint)ByteString.Empty;
            case CastExpressionSyntax expression:
                ConvertCastExpression(model, expression);
                break;
            //Converts a checked expression.
            //Example: checked(temp * 2);
            case CheckedExpressionSyntax expression:
                ConvertCheckedExpression(model, expression);
                break;
            //Converts a null-conditional access expression.
            //Example: people?.Name; array?[i];
            case ConditionalAccessExpressionSyntax expression:
                ConvertConditionalAccessExpression(model, expression);
                break;
            //Converts a null-conditional access expression.
            //Example: var x = a > b ? a : b;
            case ConditionalExpressionSyntax expression:
                ConvertConditionalExpression(model, expression);
                break;
            //Converts an array element or indexer access ([]) expression.
            //Example: array[i];
            case ElementAccessExpressionSyntax expression:
                ConvertElementAccessExpression(model, expression);
                break;
            //Further conversion of the ?[] statement in the ConvertConditionalAccessExpression method.
            //Example: array?[i];
            case ElementBindingExpressionSyntax expression:
                ConvertElementBindingExpression(model, expression);
                break;
            //Converts an identifier name expression.
            //Example: int a = 1;
            case IdentifierNameSyntax expression:
                ConvertIdentifierNameExpression(model, expression);
                break;
            //Converts an implicit array creation expression.
            //Example: var authorNames = new[] {"Shilpa", "Soniya", "Shivi", "Ritika"};
            case ImplicitArrayCreationExpressionSyntax expression:
                ConvertImplicitArrayCreationExpression(model, expression);
                break;
            //Converts initialization of array fields
            //Example: static UInt160 Owner = UInt160.Zero;
            case InitializerExpressionSyntax expression:
                ConvertInitializerExpression(model, expression);
                break;
            //Converts an interpolated string expression;
            //Example: $"Hello, {name}"
            case InterpolatedStringExpressionSyntax expression:
                ConvertInterpolatedStringExpression(model, expression);
                break;
            //Converts Invocation, include method invocation, event invocation and delegate invocation
            //Example: Runtime.Log("hello");
            case InvocationExpressionSyntax expression:
                ConvertInvocationExpression(model, expression);
                break;
            //Converts an 'is' pattern expression;
            //Example: if (obj is string)
            case IsPatternExpressionSyntax expression:
                ConvertIsPatternExpression(model, expression);
                break;
            //Converts a member access expression.
            //Example: Ledger.CurrentHash
            case MemberAccessExpressionSyntax expression:
                ConvertMemberAccessExpression(model, expression);
                break;
            //Further conversion of the ?. statement in the ConvertConditionalAccessExpression method
            //Example: people?.Name;
            case MemberBindingExpressionSyntax expression:
                ConvertMemberBindingExpression(model, expression);
                break;
            //Converts a parenthesized expression.
            //Continuing the recursive processing of the expression with the parentheses removed.
            //Example: (a / b), (a + b), (new byte[33])
            case ParenthesizedExpressionSyntax expression:
                ConvertExpression(model, expression.Expression);
                break;
            //Converts postfix operator.
            //Example: i++, i--
            case PostfixUnaryExpressionSyntax expression:
                ConvertPostfixUnaryExpression(model, expression);
                break;
            //Converts prefix operator.
            //Example: ++i, --i, !flag
            case PrefixUnaryExpressionSyntax expression:
                ConvertPrefixUnaryExpression(model, expression);
                break;
            //Converts a switch  expression.
            //Example: day switch { 1 => "Monday", 2 => "Tuesday", 3 => "Wednesday", 4 => "Thursday", 5 => "Friday", 6 => "Saturday", 7 => "Sunday", _ => "Unknown" };
            case SwitchExpressionSyntax expression:
                ConvertSwitchExpression(model, expression);
                break;
            //Converts a base  expression.
            //Example: public A() : base() { }
            case BaseExpressionSyntax:
            //Converts "this" keyword
            //The "this" keyword in extended methods is also handled here.
            //Examples: private string name; public void MyMethod(){ Runtime.Log(this.name); }
            //Examples: UInt160.Zero.ToAddress();
            case ThisExpressionSyntax:
                AddInstruction(OpCode.LDARG0);
                break;
            //Converts a throw expression
            //Examples: string a = null; var b = a ?? throw new Exception();
            //Examples: var first = args.Length >= 1 ? args[0] : throw new Exception();
            case ThrowExpressionSyntax expression:
                Throw(model, expression.Expression);
                break;
            //Converts a tuple type expression.
            //Example: (string, int) t1 = ("chris", 3);
            case TupleExpressionSyntax expression:
                ConvertTupleExpression(model, expression);
                break;
            default:
                //Example: typeof(Transaction);
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
        ITypeSymbol? type = ModelExtensions.GetTypeInfo(model, expression).Type;
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
            case "Neo.SmartContract.Framework.ECPoint":
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
