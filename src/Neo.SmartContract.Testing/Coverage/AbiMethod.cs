using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Neo.SmartContract.Testing.Coverage
{
    [DebuggerDisplay("Name={Name}, PCount={PCount}")]
    public class AbiMethod
    {
        /// <summary>
        /// Method name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Parameters count
        /// </summary>
        public int PCount { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Method name</param>
        /// <param name="pCount">Parameters count</param>
        public AbiMethod(string name, int pCount)
        {
            Name = name;
            PCount = pCount;
        }

        /// <summary>
        /// Create from expression
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>AbiMethod</returns>
        public static AbiMethod[] CreateFromExpression(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                if (memberExpression.Member is PropertyInfo pInfo)
                {
                    if (pInfo.CanRead)
                    {
                        var display = pInfo.GetGetMethod()?.GetCustomAttribute<DisplayNameAttribute>();
                        var nameRead = display is not null ? display.DisplayName : memberExpression.Member.Name;

                        if (pInfo.CanWrite)
                        {
                            // If Property CanWrite, we return both methods

                            display = pInfo.GetSetMethod()?.GetCustomAttribute<DisplayNameAttribute>();
                            var nameWrite = display is not null ? display.DisplayName : memberExpression.Member.Name;

                            return new AbiMethod[]
                            {
                                new AbiMethod(nameRead, 0),
                                new AbiMethod(nameWrite, 1)
                            };
                        }

                        // Only read property

                        return new AbiMethod[] { new AbiMethod(nameRead, 0) };
                    }
                }
            }
            else if (expression is MethodCallExpression methodExpression)
            {
                if (methodExpression.Method is MethodInfo mInfo)
                {
                    var display = mInfo.GetCustomAttribute<DisplayNameAttribute>();
                    var name = display is not null ? display.DisplayName : mInfo.Name;

                    return new AbiMethod[] { new AbiMethod(name, mInfo.GetParameters().Length) };
                }
            }

            return Array.Empty<AbiMethod>();
        }
    }
}
