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
using System.Linq.Expressions;
using Neo.VM;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Array = System.Array;

namespace Neo.Compiler;

internal static partial class SystemMethods
{
    internal delegate void SystemCallHandler(MethodConvert methodConvert, SemanticModel model, IMethodSymbol symbol, ExpressionSyntax? instanceExpression, IReadOnlyList<SyntaxNode>? arguments);

    internal static readonly Dictionary<string, SystemCallHandler> SystemCallHandlers = new();

    static SystemMethods()
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
        var key = GetKeyFromExpression(expression, typeof(T));
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression, typeof(T1), typeof(T2));
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression, typeof(T1), typeof(T2), typeof(T3));
        SystemCallHandlers[key] = handler;
    }
    private static void RegisterHandler<T1, T2, T3, T4>(Expression<Func<T1, T2, T3, T4, bool>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression);
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T>(Expression<Action<T>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression, typeof(T));
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T1, T2>(Expression<Action<T1, T2>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression, typeof(T1), typeof(T2));
        SystemCallHandlers[key] = handler;
    }

    private static void RegisterHandler<T1, T2, T3>(Expression<Action<T1, T2, T3>> expression, SystemCallHandler handler)
    {
        var key = GetKeyFromExpression(expression, typeof(T1), typeof(T2), typeof(T3));
        SystemCallHandlers[key] = handler;
    }

    private static string GetKeyFromExpression(LambdaExpression expression, params Type[] argumentTypes)
    {
        return expression.Body switch
        {
            MethodCallExpression methodCall => GetMethodCallKey(methodCall, argumentTypes),
            MemberExpression { Member: PropertyInfo property } => $"{GetShortTypeName(property.DeclaringType)}.{property.Name}.get",
            MemberExpression { Member: FieldInfo field } => $"{GetShortTypeName(field.DeclaringType)}.{field.Name}",
            UnaryExpression { NodeType: ExpressionType.Convert } unaryExpression => GetUnaryExpressionKey(unaryExpression),
            IndexExpression indexExpression => GetIndexExpressionKey(indexExpression),
            _ => throw new ArgumentException("Expression must be a method call, property, field access, or special member.", nameof(expression)),
        };
    }

    private static string GetMethodCallKey(MethodCallExpression methodCall, Type[] argumentTypes)
    {
        var method = methodCall.Method;
        // Static method
        if (methodCall.Object == null) return GetMethodKey(method, argumentTypes);

        var methodName = method.Name;
        var paramNames = argumentTypes.Select(GetShortTypeName).ToArray();
        var parameters = paramNames.Length > 0 ? string.Join(", ", paramNames[1..]) : null;

        if (method.IsSpecialName && (methodName.StartsWith("get_Char") || methodName.StartsWith("set_Char")))
        {
            var accessorType = methodName.StartsWith("get_Char") ? "get" : "set";
            return $"{GetShortTypeName(method.DeclaringType)}.this[{parameters}].{accessorType}";
        }

        if (method.IsGenericMethod)
        {
            var containingType = GetShortTypeName(method.DeclaringType);
            var genericArguments = $"<{string.Join(", ", method.GetGenericArguments().Select(GetShortTypeName))}>";
            return $"{containingType}.{methodName}{genericArguments}({parameters})";
        }

        return $"{paramNames[0]}.{methodName}({parameters})";
    }

    private static string GetUnaryExpressionKey(UnaryExpression unaryExpression)
    {
        var operandType = GetShortTypeName(unaryExpression.Operand.Type);
        var targetType = GetShortTypeName(unaryExpression.Type);
        return unaryExpression.Method.Name == "op_Implicit"
            ? $"{targetType}.implicit operator {targetType}({operandType})"
            : $"{operandType}.explicit operator {targetType}({operandType})";
    }

    private static string GetIndexExpressionKey(IndexExpression indexExpression)
    {
        var indexParams = string.Join(", ", indexExpression.Arguments.Select(arg => GetShortTypeName(arg.Type)));
        return $"{GetShortTypeName(indexExpression.Object.Type)}.this[{indexParams}].get";
    }

    private static string GetMethodKey(MethodInfo method, Type[] argumentTypes)
    {
        var containingType = GetShortTypeName(method.DeclaringType);
        var parameters = string.Join(", ", argumentTypes.Select(GetShortTypeName));

        switch (method.IsSpecialName)
        {
            case true when method.Name.StartsWith("get_Char") || method.Name.StartsWith("set_Char"):
                {
                    var accessorType = method.Name.StartsWith("get_Char") ? "get" : "set";
                    return $"{GetShortTypeName(method.DeclaringType)}.this[{parameters}].{accessorType}";
                }
            case true when method.Name.StartsWith("op_"):
                {
                    var operatorName = GetOperatorName(method.Name);
                    if (operatorName is "implicit operator" or "explicit operator")
                    {
                        var returnType = GetShortTypeName(method.ReturnType);
                        return $"{containingType}.{operatorName} {returnType}({parameters})";
                    }
                    return $"{containingType}.{operatorName}({parameters})";
                }
            default:
                {
                    var genericArguments = method.IsGenericMethod
                        ? $"<{string.Join(", ", method.GetGenericArguments().Select(GetShortTypeName))}>"
                        : "";

                    return $"{containingType}.{method.Name}{genericArguments}({parameters})";
                }
        }
    }

    private static string GetShortTypeName(Type type)
    {
        if (type.IsArray)
        {
            return GetShortTypeName(type.GetElementType()) + "[]";
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return GetShortTypeName(type.GetGenericArguments()[0]) + "?";
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
}