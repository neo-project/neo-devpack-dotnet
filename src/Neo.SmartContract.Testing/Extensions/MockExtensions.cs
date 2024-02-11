using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Neo.SmartContract.Testing.Extensions
{
    internal static class MockExtensions
    {
        private static readonly MethodInfo isAnyMethod = typeof(It).GetMethod(nameof(It.IsAny), BindingFlags.Public | BindingFlags.Static)!;

        private static MethodCallExpression BuildIsAnyExpressions(Type type)
        {
            return Expression.Call(isAnyMethod.MakeGenericMethod(type));
        }

        private static Expression BuildIsAnyExpressions<T>(Mock<T> mock, string name, Type[] args)
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

        public static void MockMethodWithReturn<T>(this Mock<T> mock, string name, Type[] args, Type returnType)
            where T : SmartContract
        {
            Expression exp = BuildIsAnyExpressions(mock, name, args);

            var setupMethod = mock.GetType()
               .GetMethods(BindingFlags.Instance | BindingFlags.Public)
               .First(u => u.Name == nameof(Mock<T>.Setup) &&
                    u.GetParameters().Length == 1 &&
                    u.GetParameters()[0].ParameterType.ToString().Contains("[System.Func`")
                    )
               .MakeGenericMethod(returnType);

            var setup = setupMethod.Invoke(mock, new object[] { exp })!;

            var retMethod = setup.GetType()
               .GetMethod("Returns", new Type[] { typeof(InvocationFunc) })!;

            _ = retMethod.Invoke(setup, new object[] { new InvocationFunc(invocation =>
                {
                    return mock.Object.Invoke(invocation.Method.Name, invocation.Arguments.ToArray()).ConvertTo(returnType)!;
                })
            });
        }

        public static void MockMethod<T>(this Mock<T> mock, string name, Type[] args)
            where T : SmartContract
        {
            Expression exp = BuildIsAnyExpressions(mock, name, args);

            var setupMethod = mock.GetType()
               .GetMethods(BindingFlags.Instance | BindingFlags.Public)
               .First(u => u.Name == nameof(Mock<T>.Setup) &&
                    u.GetParameters().Length == 1 &&
                    u.GetParameters()[0].ParameterType.ToString().Contains("[System.Action`")
                    );

            var setup = setupMethod.Invoke(mock, new object[] { exp })!;

            var retMethod = setup.GetType()
               .GetMethod("Callback", new Type[] { typeof(InvocationAction) })!;

            _ = retMethod.Invoke(setup, new object[] { new InvocationAction(invocation =>
                {
                    mock.Object.Invoke(invocation.Method.Name, invocation.Arguments.ToArray());
                })
            });
        }
    }
}
