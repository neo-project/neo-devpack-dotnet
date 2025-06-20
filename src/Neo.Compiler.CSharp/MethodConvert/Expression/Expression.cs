// Copyright (C) 2015-2025 The Neo Project.
//
// Expression.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.Cryptography.ECC;
using Neo.Extensions;
using Neo.SmartContract.Native;
using Neo.VM;
using Neo.VM.Types;
using Neo.Wallets;
using System.Linq;
using System.Numerics;

namespace Neo.Compiler;

internal partial class MethodConvert
{
    /// <summary>
    /// Converts an expression to NeoVM instructions.
    /// </summary>
    /// <param name="model">The semantic model of the compilation.</param>
    /// <param name="syntax">The expression syntax to convert.</param>
    /// <param name="syntaxNode">Optional parent syntax node for context.</param>
    private void ConvertExpression(SemanticModel model, ExpressionSyntax syntax, SyntaxNode? syntaxNode = null)
    {
        // Insert a sequence point for debugging purposes
        using var sequence = InsertSequencePoint(syntax);

        // Try to convert the expression as a constant first
        if (TryConvertConstant(model, syntax, syntaxNode))
            return;

        // If it's not a constant, convert it as a non-constant expression
        ConvertNonConstantExpression(model, syntax);
    }

    private bool TryConvertConstant(SemanticModel model, ExpressionSyntax syntax, SyntaxNode? syntaxNode)
    {
        try
        {
            var constant = model.GetConstantValue(syntax);
            var value = constant.Value;
            if (value == null)
                return false;

            ITypeSymbol? typeSymbol = GetTypeSymbol(syntaxNode, model);

            if (typeSymbol != null)
            {
                value = ConvertComplexConstantTypes(typeSymbol, value, syntax);
            }

            Push(value);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private void ConvertNonConstantExpression(SemanticModel model, ExpressionSyntax syntax)
    {
        switch (syntax)
        {
            case AnonymousObjectCreationExpressionSyntax expression:
                // Example: new { Name = "John", Age = 30 }
                ConvertAnonymousObjectCreationExpression(model, expression);
                break;
            case ArrayCreationExpressionSyntax expression:
                // Example: new int[] { 1, 2, 3 }
                ConvertArrayCreationExpression(model, expression);
                break;
            case AssignmentExpressionSyntax expression:
                // Example: x = 5
                ConvertAssignmentExpression(model, expression);
                break;
            case BaseObjectCreationExpressionSyntax expression:
                // Example: new MyClass()
                ConvertObjectCreationExpression(model, expression);
                break;
            case BinaryExpressionSyntax expression:
                // Example: a + b
                ConvertBinaryExpression(model, expression);
                break;
            case CastExpressionSyntax expression:
                // Example: (int)myDouble
                ConvertCastExpression(model, expression);
                break;
            case CheckedExpressionSyntax expression:
                // Example: checked(x + y)
                ConvertCheckedExpression(model, expression);
                break;
            case ConditionalAccessExpressionSyntax expression:
                // Example: person?.Name
                ConvertConditionalAccessExpression(model, expression);
                break;
            case ConditionalExpressionSyntax expression:
                // Example: isTrue ? "Yes" : "No"
                ConvertConditionalExpression(model, expression);
                break;
            case ElementAccessExpressionSyntax expression:
                // Example: myArray[0]
                ConvertElementAccessExpression(model, expression);
                break;
            case ElementBindingExpressionSyntax expression:
                // Example: obj?[0]
                ConvertElementBindingExpression(model, expression);
                break;
            case IdentifierNameSyntax expression:
                // Example: myVariable
                ConvertIdentifierNameExpression(model, expression);
                break;
            case ImplicitArrayCreationExpressionSyntax expression:
                // Example: new[] { 1, 2, 3 }
                ConvertImplicitArrayCreationExpression(model, expression);
                break;
            case InitializerExpressionSyntax expression:
                // Example: { 1, 2, 3 }
                ConvertInitializerExpression(model, expression);
                break;
            case InterpolatedStringExpressionSyntax expression:
                // Example: $"Hello, {name}!"
                ConvertInterpolatedStringExpression(model, expression);
                break;
            case InvocationExpressionSyntax expression:
                // Example: MyMethod()
                ConvertInvocationExpression(model, expression);
                break;
            case IsPatternExpressionSyntax expression:
                // Example: obj is string s
                ConvertIsPatternExpression(model, expression);
                break;
            case MemberAccessExpressionSyntax expression:
                // Example: myObject.Property
                ConvertMemberAccessExpression(model, expression);
                break;
            case MemberBindingExpressionSyntax expression:
                // Example: ?.Property
                ConvertMemberBindingExpression(model, expression);
                break;
            case ParenthesizedExpressionSyntax expression:
                // Example: (x + y)
                ConvertExpression(model, expression.Expression);
                break;
            case PostfixUnaryExpressionSyntax expression:
                // Example: x++
                ConvertPostfixUnaryExpression(model, expression);
                break;
            case PrefixUnaryExpressionSyntax expression:
                // Example: ++x
                ConvertPrefixUnaryExpression(model, expression);
                break;
            case SwitchExpressionSyntax expression:
                // Example: x switch { 1 => "One", 2 => "Two", _ => "Other" }
                ConvertSwitchExpression(model, expression);
                break;
            case BaseExpressionSyntax:
            case ThisExpressionSyntax:
                // Example: base.Method() or this.Property
                AddInstruction(OpCode.LDARG0);
                break;
            case ThrowExpressionSyntax expression:
                // Example: throw new Exception("Error")
                Throw(model, expression.Expression);
                break;
            case TupleExpressionSyntax expression:
                // Example: (1, "Hello")
                ConvertTupleExpression(model, expression);
                break;
            case ParenthesizedLambdaExpressionSyntax expression:
                // Example: (x, y) => x + y
                ConvertParenthesizedLambdaExpression(model, expression);
                break;
            case SimpleLambdaExpressionSyntax expression:
                // Example: x => x * x
                ConvertSimpleLambdaExpression(model, expression);
                break;
            case CollectionExpressionSyntax expression:
                // Example: [1, 2, 3]
                ConvertCollectionExpression(model, expression);
                break;
            case WithExpressionSyntax expression:
                // Example: person with { Name = "John" }
                ConvertWithExpressionSyntax(model, expression);
                break;
            case LiteralExpressionSyntax expression:
                // Example: 42 or "Hello"
                ConvertLiteralExpression(model, expression);
                break;
            case TypeOfExpressionSyntax expression:
                // Example: typeof(int)
                // Note: Neo currently does not support the Type type of C#. The typeof operator here
                // will only return the string name of the class/type. This support is added
                // to ensure we can process enum parse methods.
                ConvertTypeOfExpression(model, expression);
                break;
            default:
                throw CompilationException.UnsupportedSyntax(syntax, $"Unsupported expression syntax '{syntax.GetType().Name}'. Consider using supported expressions: assignments, method calls, operators, or literal values.");
        }
    }

    private void ConvertTypeOfExpression(SemanticModel model, TypeOfExpressionSyntax expression)
    {
        var typeInfo = model.GetTypeInfo(expression.Type);
        if (typeInfo.Type == null)
            throw new CompilationException(expression, DiagnosticId.InvalidType, $"Invalid type in typeof expression: {expression.Type}");

        Push(typeInfo.Type.Name);
    }

    private static ITypeSymbol? GetTypeSymbol(SyntaxNode? syntaxNode, SemanticModel model)
    {
        return syntaxNode switch
        {
            VariableDeclaratorSyntax { Parent: VariableDeclarationSyntax declaration }
                => ModelExtensions.GetTypeInfo(model, declaration.Type).Type,
            PropertyDeclarationSyntax propertyDeclaration
                => ModelExtensions.GetTypeInfo(model, propertyDeclaration.Type).Type,
            _ => null
        };
    }

    private object ConvertComplexConstantTypes(ITypeSymbol typeSymbol, object value, ExpressionSyntax syntax)
    {
        string fullName = typeSymbol.ToDisplayString();
        return fullName switch
        {
            "Neo.SmartContract.Framework.UInt160" => ConvertToUInt160((string)value!),
            "Neo.SmartContract.Framework.UInt256" => ConvertToUInt256((string)value!, syntax),
            "Neo.SmartContract.Framework.ECPoint" => ConvertToECPoint((string)value!),
            "Neo.SmartContract.Framework.ByteArray" => ((string)value!).HexToBytes(true),
            _ => value
        };
    }

    private byte[] ConvertToUInt160(string strValue)
    {
        return (UInt160.TryParse(strValue, out var hash)
            ? hash
            : strValue.ToScriptHash(_context.Options.AddressVersion)).ToArray();
    }

    private static byte[] ConvertToUInt256(string strValue, ExpressionSyntax syntax)
    {
        var value = strValue.HexToBytes(true);
        if (value.Length != 32)
            throw new CompilationException(syntax, DiagnosticId.InvalidInitialValue, "Invalid UInt256 literal");
        return value;
    }

    private static byte[] ConvertToECPoint(string strValue)
    {
        return ECPoint.Parse(strValue, ECCurve.Secp256r1).EncodePoint(true);
    }


    /// <summary>
    /// Ensures that the value of the incoming integer type is within the specified range.
    /// If the type is BigInteger, no range check is performed.
    /// </summary>
    /// <param name="type">The integer type to be checked.</param>
    private void EnsureIntegerInRange(ITypeSymbol type)
    {
        if (type.Name == "BigInteger") return;
        while (type.NullableAnnotation == NullableAnnotation.Annotated)
        {
            // Supporting nullable integer like `byte?`
            type = ((INamedTypeSymbol)type).TypeArguments.First();
        }

        var (minValue, maxValue, mask) = type.Name switch
        {
            "SByte" => ((BigInteger)sbyte.MinValue, (BigInteger)sbyte.MaxValue, (BigInteger)0xff),
            "Int16" => (short.MinValue, short.MaxValue, 0xffff),
            "Char" => (ushort.MinValue, ushort.MaxValue, 0xffff),
            "Int32" => (int.MinValue, int.MaxValue, 0xffffffff),
            "Int64" => (long.MinValue, long.MaxValue, 0xffffffffffffffff),
            "Byte" => (byte.MinValue, byte.MaxValue, 0xff),
            "UInt16" => (ushort.MinValue, ushort.MaxValue, 0xffff),
            "UInt32" => (uint.MinValue, uint.MaxValue, 0xffffffff),
            "UInt64" => (ulong.MinValue, ulong.MaxValue, 0xffffffffffffffff),
            //"Boolean" => (0, 1, 0x01),
            _ => throw new CompilationException(DiagnosticId.SyntaxNotSupported, $"Unsupported type '{type}'. Consider using supported types: int, string, byte[], BigInteger, UInt160, UInt256, or custom structs/classes.")
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

    /// <summary>
    /// Converts an object to a string. Different conversion methods are used based on the type of the object.
    /// </summary>
    /// <param name="model">The semantic model used to obtain type information of the expression.</param>
    /// <param name="expression">The expression to be converted to a string.</param>
    /// <remarks>
    /// For integer types and BigInteger type, call the itoa method of NativeContract.StdLib.Hash for conversion.
    /// For string type and specific types in Neo.SmartContract.Framework, directly perform expression conversion.
    /// </remarks>
    /// <exception cref="CompilationException">For unsupported types, throw a compilation exception.</exception>
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
                CallContractMethod(NativeContract.StdLib.Hash, "itoa", 1, true);
                break;
            case "char":
                ConvertExpression(model, expression);
                ChangeType(StackItemType.ByteString);
                break;
            case "string":
            case "Neo.SmartContract.Framework.ECPoint":
            case "Neo.SmartContract.Framework.ByteString":
            case "Neo.SmartContract.Framework.UInt160":
            case "Neo.SmartContract.Framework.UInt256":
                ConvertExpression(model, expression);
                break;
            case "bool":
                {
                    ConvertExpression(model, expression);
                    JumpTarget falseTarget = new();
                    Jump(OpCode.JMPIFNOT_L, falseTarget);
                    Push("True");
                    JumpTarget endTarget = new();
                    Jump(OpCode.JMP_L, endTarget);
                    falseTarget.Instruction = Push("False");
                    endTarget.Instruction = AddInstruction(OpCode.NOP);
                    break;
                }
            case "byte[]":
                {
                    Push("System.Byte[]");
                    break;
                }
            default:
                throw new CompilationException(expression, DiagnosticId.InvalidToStringType, $"Unsupported interpolation: {expression}");
        }
    }
}
