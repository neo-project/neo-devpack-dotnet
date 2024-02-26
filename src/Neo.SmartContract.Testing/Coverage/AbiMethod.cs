using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Neo.SmartContract.Testing.Coverage
{
    [DebuggerDisplay("{Name},{PCount}")]
    public class AbiMethod : IEquatable<AbiMethod>
    {
        private readonly string _toString;

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
        /// <param name="argsName">Arguments names</param>
        public AbiMethod(string name, string[] argsName)
        {
            Name = name;
            PCount = argsName.Length;
            _toString = name + $"({string.Join(",", argsName)})";
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
                                new AbiMethod(nameRead, Array.Empty<string>()),
                                new AbiMethod(nameWrite, new string[]{ "value" })
                            };
                        }

                        // Only read property

                        return new AbiMethod[] { new AbiMethod(nameRead, Array.Empty<string>()) };
                    }
                }
            }
            else if (expression is MethodCallExpression methodExpression)
            {
                if (methodExpression.Method is MethodInfo mInfo)
                {
                    var display = mInfo.GetCustomAttribute<DisplayNameAttribute>();
                    var name = display is not null ? display.DisplayName : mInfo.Name;

                    return new AbiMethod[] { new AbiMethod(name, mInfo.GetParameters().Select(u => u.Name ?? "arg").ToArray()) };
                }
            }

            return Array.Empty<AbiMethod>();
        }

        public override bool Equals(object obj)
        {
            if (obj is not AbiMethod other) return false;

            return PCount == other.PCount && Name == other.Name;
        }

        bool IEquatable<AbiMethod>.Equals(AbiMethod other) => PCount == other.PCount && Name == other.Name;
        public override int GetHashCode() => HashCode.Combine(PCount, Name);
        public override string ToString() => _toString;
    }
}
