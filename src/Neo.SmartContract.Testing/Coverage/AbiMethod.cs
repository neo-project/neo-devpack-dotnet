// Copyright (C) 2015-2025 The Neo Project.
//
// AbiMethod.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

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

                            return
                            [
                                new AbiMethod(nameRead, []),
                                new AbiMethod(nameWrite, ["value"])
                            ];
                        }

                        // Only read property

                        return [new AbiMethod(nameRead, [])];
                    }
                }
            }
            else if (expression is MethodCallExpression methodExpression)
            {
                if (methodExpression.Method is MethodInfo mInfo)
                {
                    var display = mInfo.GetCustomAttribute<DisplayNameAttribute>();
                    var name = display is not null ? display.DisplayName : mInfo.Name;

                    return [new AbiMethod(name, mInfo.GetParameters().Select(u => u.Name ?? "arg").ToArray())];
                }
            }

            return [];
        }

        public override bool Equals(object? obj)
        {
            if (obj is not AbiMethod other) return false;

            return PCount == other.PCount && Name == other.Name;
        }

        bool IEquatable<AbiMethod>.Equals(AbiMethod? other) => other != null && PCount == other.PCount && Name == other.Name;
        public override int GetHashCode() => HashCode.Combine(PCount, Name);
        public override string ToString() => _toString;
    }
}
