// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.Compiler.CSharp is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

extern alias scfx;
using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Neo.SmartContract.Native;
using Neo.VM.Types;
using Array = System.Array;

namespace Neo.Compiler;

partial class MethodConvert
{
    private delegate bool SystemCallHandler(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments);

    private static readonly Dictionary<string, SystemCallHandler> SystemCallHandlers = new();

    static MethodConvert()
    {
        RegisterSystemCallHandlers();
    }

    private static void RegisterHandler<TResult>(Expression<Func<TResult>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression);
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T, TResult>(Expression<Func<T, TResult>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression);
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression);
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression);
        SystemCallHandlers[key] = handler;
    }

    private static string GetKeyFromExpression(LambdaExpression expression)
    {
        switch (expression.Body)
        {
            case MethodCallExpression methodCall:
                return GetMethodKey(methodCall.Method);

            case MemberExpression { Member: PropertyInfo property }:
                return $"{GetShortTypeName(property.DeclaringType)}.{property.Name}.get";

            case MemberExpression { Member: FieldInfo field }:
                return $"{GetShortTypeName(field.DeclaringType)}.{field.Name}";

            case UnaryExpression { NodeType: ExpressionType.Convert } unaryExpression:
                var operandType = GetShortTypeName(unaryExpression.Operand.Type);
                var targetType = GetShortTypeName(unaryExpression.Type);
                return unaryExpression.Method.Name == "op_Implicit" ? $"{targetType}.implicit operator {targetType}({operandType})" : $"{operandType}.explicit operator {targetType}({operandType})";

            case IndexExpression indexExpression:
                var indexParams = string.Join(", ", indexExpression.Arguments.Select(arg => GetShortTypeName(arg.Type)));
                return $"{GetShortTypeName(indexExpression.Object.Type)}.this[{indexParams}].get";

            default:
                throw new ArgumentException("Expression must be a method call, property, field access, or special member.", nameof(expression));
        }
    }

    private static string GetMethodKey(MethodInfo method)
    {
        var containingType = GetShortTypeName(method.DeclaringType);
        var parameters = string.Join(", ", method.GetParameters().Select(p => GetShortTypeName(p.ParameterType)));

        if (method.IsSpecialName && (method.Name.StartsWith("get_Char") || method.Name.StartsWith("set_Char")))
        {
            var accessorType = method.Name.StartsWith("get_Char") ? "get" : "set";
            return $"{GetShortTypeName(method.DeclaringType)}.this[{parameters}].{accessorType}";
        }

        if (method.IsSpecialName && method.Name.StartsWith("op_"))
        {
            var operatorName = GetOperatorName(method.Name);
            if (operatorName is "implicit operator" or "explicit operator")
            {
                var returnType = GetShortTypeName(method.ReturnType);
                return $"{containingType}.{operatorName} {returnType}({parameters})";
            }
            return $"{containingType}.{operatorName}({parameters})";
        }

        return $"{containingType}.{method.Name}({parameters})";
    }

    private static string GetShortTypeName(Type type)
    {
        if (type.IsArray)
        {
            return GetShortTypeName(type.GetElementType()) + "[]";
        }

        return type switch
        {
            _ when type == typeof(int) => "int",
            _ when type == typeof(long) => "long",
            _ when type == typeof(short) => "short",
            _ when type == typeof(byte) => "byte",
            _ when type == typeof(bool) => "bool",
            _ when type == typeof(string) => "string",
            _ when type == typeof(char) => "char",
            _ when type == typeof(void) => "void",
            _ when type == typeof(object) => "object",
            _ when type == typeof(sbyte) => "sbyte",
            _ when type == typeof(uint) => "uint",
            _ when type == typeof(ulong) => "ulong",
            _ when type == typeof(ushort) => "ushort",
            // _ when type == typeof(byte[]) => "byte[]",
            _ when type == typeof(BigInteger) => "System.Numerics.BigInteger",
            _ when type == typeof(Array) => "System.Array",
            _ when type == typeof(Math) => "System.Math",
            _ when type.IsGenericType => $"{type.Name.Split('`')[0]}<{string.Join(", ", type.GetGenericArguments().Select(GetShortTypeName))}>",
            _ => type.Name,
        };
    }

    private static string GetOperatorName(string methodName)
    {
        return methodName switch
        {
            "op_Implicit" => "implicit operator",
            "op_Explicit" => "explicit operator",
            _ => methodName.StartsWith("op_") ? methodName[3..] : methodName
        };
    }

    /// <summary>
    /// Attempts to process system constructors. Performs different processing operations based on the method symbol.
    /// </summary>
    /// <param name="model">The semantic model used to obtain detailed information about the symbol.</param>
    /// <param name="symbol">The method symbol to be processed.</param>
    /// <param name="arguments">A list of syntax nodes representing the arguments of the method.</param>
    /// <returns>True if system constructors are successfully processed; otherwise, false.</returns>
    private bool TryProcessSystemConstructors(SemanticModel model, IMethodSymbol symbol, IReadOnlyList<ArgumentSyntax> arguments)
    {
        switch (symbol.ToString())
        {
            //For the BigInteger(byte[]) constructor, prepares method arguments and changes the return type to integer.
            case "System.Numerics.BigInteger.BigInteger(byte[])":
                PrepareArgumentsForMethod(model, symbol, arguments);
                ChangeType(VM.Types.StackItemType.Integer);
                return true;
            //For other constructors, such as List<T>(), return processing failure.
            default:
                return false;
        }
    }

    /// <summary>
    /// Attempts to process system methods. Performs different processing operations based on the method symbol.
    /// </summary>
    /// <param name="model">The semantic model used to obtain detailed information about the symbol.</param>
    /// <param name="symbol">The method symbol to be processed.</param>
    /// <param name="instanceExpression">The instance expression representing the instance of method invocation, if any.</param>
    /// <param name="arguments">A list of syntax nodes representing the arguments of the method.</param>
    /// <returns>True if system methods are successfully processed; otherwise, false.</returns>
    private bool TryProcessSystemMethods(SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments)
    {
        //If the method belongs to a delegate and the method name is "Invoke",
        //calls the PrepareArgumentsForMethod method with CallingConvention.Cdecl convention and changes the return type to integer.
        //Example: Func<int, int, int>(privateSum).Invoke(a, b);
        //see ~/tests/Neo.Compiler.CSharp.TestContracts/Contract_Delegate.cs
        if (symbol.ContainingType.TypeKind == TypeKind.Delegate && symbol.Name == "Invoke")
        {
            if (arguments is not null)
                PrepareArgumentsForMethod(model, symbol, arguments, CallingConvention.Cdecl);
            ConvertExpression(model, instanceExpression!);
            AddInstruction(OpCode.CALLA);
            return true;
        }

        var key = symbol.ToString()!.Replace("?", "");
        if (key == "string.ToString()") key = "object.ToString()";
        if (SystemCallHandlers.TryGetValue(key, out var handler))
        {
            return handler(this, model, symbol, instanceExpression, arguments);
        }
        return false;
    }
}
