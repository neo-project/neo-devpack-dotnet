// Copyright (C) 2015-2025 The Neo Project.
//
// DevelopmentContractProxy.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.ContractInvocation
{
    /// <summary>
    /// Specialized proxy for development contracts that handles method resolution
    /// and compilation-time dependencies.
    /// </summary>
    public abstract class DevelopmentContractProxy : ContractProxyBase
    {
        private static readonly Dictionary<string, MethodInfo> _methodCache = new();

        /// <summary>
        /// Gets the source contract type for method reflection.
        /// </summary>
        protected abstract Type SourceContractType { get; }

        /// <summary>
        /// Initializes a new DevelopmentContractProxy.
        /// </summary>
        /// <param name="contractReference">The development contract reference</param>
        protected DevelopmentContractProxy(DevelopmentContractReference contractReference)
            : base(contractReference)
        {
            ValidateSourceContract();
        }

        /// <summary>
        /// Invokes a method on the development contract with compile-time validation.
        /// </summary>
        /// <param name="method">The method name to invoke</param>
        /// <param name="flags">The call flags for the invocation</param>
        /// <param name="args">The method arguments</param>
        /// <returns>The result of the contract method call</returns>
        protected override object InvokeMethod(string method, CallFlags flags, params object?[]? args)
        {
            // Validate method exists in source contract
            var methodInfo = GetValidatedMethodInfo(method, args);

            // If contract is not resolved (still developing), provide development-time behavior
            if (!ContractReference.IsResolved)
            {
                return HandleDevelopmentTimeInvocation(methodInfo, args);
            }

            // Normal invocation for resolved contracts
            return base.InvokeMethod(method, flags, args);
        }

        /// <summary>
        /// Handles method invocation during development time before contract is deployed.
        /// </summary>
        /// <param name="methodInfo">The method info from source contract</param>
        /// <param name="args">The method arguments</param>
        /// <returns>Development-time result or simulation</returns>
        protected virtual object HandleDevelopmentTimeInvocation(MethodInfo methodInfo, object?[]? args)
        {
            // Options for development-time behavior:

            // 1. Throw exception indicating contract not deployed yet
            throw new InvalidOperationException(
                $"Development contract '{ContractReference.Identifier}' is not yet compiled. " +
                $"Method '{methodInfo.Name}' cannot be invoked until contract is deployed or compiled.");

            // 2. Alternative: Return default values for testing
            // return GetDefaultReturnValue(methodInfo.ReturnType);

            // 3. Alternative: Invoke source method directly for unit testing
            // return InvokeSourceMethodDirectly(methodInfo, args);
        }

        /// <summary>
        /// Gets method info for the specified method name with validation.
        /// </summary>
        private MethodInfo GetValidatedMethodInfo(string methodName, object?[]? args)
        {
            var cacheKey = $"{SourceContractType.FullName}.{methodName}";

            if (!_methodCache.TryGetValue(cacheKey, out var methodInfo))
            {
                methodInfo = FindMatchingMethod(methodName, args);
                if (methodInfo == null)
                {
                    throw new MethodNotFoundException(
                        $"Method '{methodName}' not found in development contract '{SourceContractType.Name}'. " +
                        $"Available methods: {string.Join(", ", GetAvailableMethodNames())}");
                }
                _methodCache[cacheKey] = methodInfo;
            }

            return methodInfo;
        }

        /// <summary>
        /// Finds a matching method in the source contract type.
        /// </summary>
        private MethodInfo? FindMatchingMethod(string methodName, object?[]? args)
        {
            var methods = SourceContractType.GetMethods(BindingFlags.Public | BindingFlags.Static);

            foreach (var method in methods)
            {
                if (method.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase))
                {
                    // Check parameter compatibility
                    if (IsParameterCompatible(method, args))
                    {
                        return method;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if method parameters are compatible with provided arguments.
        /// </summary>
        private static bool IsParameterCompatible(MethodInfo method, object?[]? args)
        {
            var parameters = method.GetParameters();

            if (args == null && parameters.Length == 0)
                return true;

            if (args == null || parameters.Length != args.Length)
                return false;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (args[i] != null && !parameters[i].ParameterType.IsAssignableFrom(args[i]!.GetType()))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets available method names for error reporting.
        /// </summary>
        private string[] GetAvailableMethodNames()
        {
            var methods = SourceContractType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            var names = new List<string>();

            foreach (var method in methods)
            {
                if (!method.IsSpecialName && method.DeclaringType != typeof(object))
                {
                    names.Add(method.Name);
                }
            }

            return names.ToArray();
        }

        /// <summary>
        /// Validates that the source contract type is properly configured.
        /// </summary>
        private void ValidateSourceContract()
        {
            if (SourceContractType == null)
            {
                throw new InvalidOperationException(
                    $"SourceContractType must be specified for development contract proxy.");
            }

            // Validate that source contract inherits from SmartContract
            if (!typeof(SmartContract).IsAssignableFrom(SourceContractType))
            {
                throw new ArgumentException(
                    $"Source contract type '{SourceContractType.Name}' must inherit from SmartContract.");
            }
        }

        /// <summary>
        /// Gets default return value for a given type (for simulation purposes).
        /// </summary>
        protected static object? GetDefaultReturnValue(Type returnType)
        {
            if (returnType == typeof(void))
                return null;

            if (returnType.IsValueType)
                return Activator.CreateInstance(returnType);

            return null;
        }
    }

    /// <summary>
    /// Exception thrown when a method is not found in the development contract.
    /// </summary>
    public class MethodNotFoundException : Exception
    {
        public MethodNotFoundException(string message) : base(message) { }
        public MethodNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
