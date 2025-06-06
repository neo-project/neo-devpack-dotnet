// Copyright (C) 2015-2025 The Neo Project.
//
// MockExtensions.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Moq;
using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Neo.SmartContract.Testing.Extensions
{
    internal static class MockExtensions
    {
        private static readonly Type methodCallType = typeof(Mock).Assembly.GetType("Moq.MethodCall")!;
        private static readonly MethodInfo isAnyMethod = typeof(It).GetMethod(nameof(It.IsAny), BindingFlags.Public | BindingFlags.Static)!;

        public static bool IsMocked<T>(this Mock<T> mock, MethodInfo method)
                where T : SmartContract
        {
            var property = methodCallType.GetProperty("Method")!;

            foreach (var setup in mock.Setups)
            {
                if (setup.GetType() != methodCallType) continue;
                if (property.GetValue(setup) is not MethodInfo mSetup) continue;

                if (method.Equals(mSetup))
                {
                    return true;
                }

                // Sometimes method comparation with Equals doesn't work as expected with moq

                if (method.DeclaringType?.Equals(mSetup.DeclaringType) == true &&
                    method.Attributes.Equals(mSetup.Attributes) &&
                    method.Name.Equals(mSetup.Name) &&
                    method.ReturnType.Equals(mSetup.ReturnType) &&
                    method.MetadataToken.Equals(mSetup.MetadataToken))
                {
                    var par1 = method.GetParameters();
                    var par2 = mSetup.GetParameters();

                    if (par1.Length == par2.Length)
                    {
                        var eq = true;

                        for (int i = 0; i < par1.Length; i++)
                        {
                            if (par1[i].ParameterType != par2[i].ParameterType)
                            {
                                eq = false;
                                break;
                            }
                        }

                        if (eq) return true;
                    }
                }
            }

            return false;
        }

        private static MethodCallExpression BuildIsAnyExpressions(Type type)
        {
            return Expression.Call(isAnyMethod.MakeGenericMethod(type));
        }

        private static LambdaExpression BuildIsAnyExpressions<T>(Mock<T> mock, string name, Type[] args)
            where T : SmartContract
        {
            var mockType = mock.Object.GetType().BaseType!;
            var expArgs = args.Select(BuildIsAnyExpressions).ToArray();

            var instanceParam = Expression.Parameter(mockType, "x");

            var metodoInfo = mockType.GetMethod(name, args)!;
            var callExpression = Expression.Call(instanceParam, metodoInfo, expArgs);
            var parameterExpression = Expression.Parameter(mockType, "x");

            return Expression.Lambda(callExpression, parameterExpression);
        }

        internal static void MockFunction<T>(this Mock<T> mock, string name, Type[] args, Type returnType, TestEngine engine)
            where T : SmartContract
        {
            var exp = BuildIsAnyExpressions(mock, name, args);

            var setupMethod = mock.GetType()
               .GetMethods(BindingFlags.Instance | BindingFlags.Public)
               .First(u => u.Name == nameof(Mock<T>.Setup) &&
                    u.GetParameters().Length == 1 &&
                    u.GetParameters()[0].ParameterType.ToString().Contains("[System.Func`")
                    )
               .MakeGenericMethod(returnType);

            var setup = setupMethod.Invoke(mock, new[] { exp })!;

            var retMethod = setup.GetType()
               .GetMethod("Returns", [typeof(InvocationFunc)])!;

            _ = retMethod.Invoke(setup, [new InvocationFunc(invocation =>
                {
                    var display = invocation.Method.GetCustomAttribute<DisplayNameAttribute>();
                    var name = display is not null ? display.DisplayName : invocation.Method.Name;

                    return mock.Object.Invoke(name, [.. invocation.Arguments]).ConvertTo(returnType, engine.StringInterpreter)!;
                })
            ]);
        }

        internal static void MockAction<T>(this Mock<T> mock, string name, Type[] args)
            where T : SmartContract
        {
            Expression exp = BuildIsAnyExpressions(mock, name, args);

            var setupMethod = mock.GetType()
               .GetMethods(BindingFlags.Instance | BindingFlags.Public)
               .First(u => u.Name == nameof(Mock<T>.Setup) &&
                    u.GetParameters().Length == 1 &&
                    u.GetParameters()[0].ParameterType.ToString().Contains("[System.Action`")
                    );

            var setup = setupMethod.Invoke(mock, new[] { exp })!;

            var retMethod = setup.GetType()
               .GetMethod("Callback", [typeof(InvocationAction)])!;

            _ = retMethod.Invoke(setup, [new InvocationAction(invocation =>
                {
                    var display = invocation.Method.GetCustomAttribute<DisplayNameAttribute>();
                    var name = display is not null ? display.DisplayName : invocation.Method.Name;

                    mock.Object.Invoke(name, [.. invocation.Arguments]);
                })
            ]);
        }
    }
}
