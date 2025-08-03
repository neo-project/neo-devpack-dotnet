// Copyright (C) 2015-2025 The Neo Project.
//
// MethodResolver.cs file belongs to the neo project and is free
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
using Neo.SmartContract.Framework.ContractInvocation.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.ContractInvocation
{
    /// <summary>
    /// Resolves method calls for both standard and non-standard contract methods.
    /// Handles development contracts, custom methods, and parameter transformations.
    /// </summary>
    public static class MethodResolver
    {
        private static readonly Dictionary<string, MethodResolutionInfo> _resolutionCache = new();

        /// <summary>
        /// Resolves a method call for the specified contract reference.
        /// </summary>
        /// <param name="contractReference">The contract reference</param>
        /// <param name="methodName">The method name to invoke</param>
        /// <param name="parameters">The method parameters</param>
        /// <param name="sourceType">Optional source contract type for development contracts</param>
        /// <returns>Method resolution information</returns>
        public static MethodResolutionInfo ResolveMethod(
            IContractReference contractReference,
            string methodName,
            object?[]? parameters,
            Type? sourceType = null)
        {
            var cacheKey = GenerateCacheKey(contractReference, methodName, parameters, sourceType);

            if (_resolutionCache.TryGetValue(cacheKey, out var cached))
            {
                return cached;
            }

            var resolution = PerformMethodResolution(contractReference, methodName, parameters, sourceType);
            _resolutionCache[cacheKey] = resolution;

            return resolution;
        }

        /// <summary>
        /// Performs the actual method resolution logic.
        /// </summary>
        private static MethodResolutionInfo PerformMethodResolution(
            IContractReference contractReference,
            string methodName,
            object?[]? parameters,
            Type? sourceType)
        {
            var resolution = new MethodResolutionInfo
            {
                OriginalMethodName = methodName,
                OriginalParameters = parameters,
                ContractReference = contractReference
            };

            try
            {
                // 1. Handle development contracts
                if (contractReference is DevelopmentContractReference devRef)
                {
                    ResolveForDevelopmentContract(resolution, devRef, sourceType);
                }
                // 2. Handle deployed contracts with manifest
                else if (contractReference is DeployedContractReference deployedRef && deployedRef.Manifest.HasValue)
                {
                    ResolveForDeployedContract(resolution, deployedRef);
                }
                // 3. Handle deployed contracts without manifest (assume standard)
                else
                {
                    ResolveAsStandardMethod(resolution);
                }

                resolution.IsResolved = true;
            }
            catch (Exception ex)
            {
                resolution.IsResolved = false;
                resolution.ResolutionError = ex;
                resolution.ErrorMessage = $"Failed to resolve method '{methodName}': {ex.Message}";
            }

            return resolution;
        }

        /// <summary>
        /// Resolves method for development contracts using source type reflection.
        /// </summary>
        private static void ResolveForDevelopmentContract(
            MethodResolutionInfo resolution,
            DevelopmentContractReference devRef,
            Type? sourceType)
        {
            if (sourceType == null)
            {
                throw new InvalidOperationException(
                    $"Source contract type must be provided for development contract '{devRef.Identifier}'.");
            }

            var methods = sourceType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name.Equals(resolution.OriginalMethodName, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            if (methods.Length == 0)
            {
                throw new MethodNotFoundException(
                    $"Method '{resolution.OriginalMethodName}' not found in development contract '{sourceType.Name}'.");
            }

            // Find best matching method
            var bestMatch = FindBestMatchingMethod(methods, resolution.OriginalParameters);
            if (bestMatch == null)
            {
                throw new MethodNotFoundException(
                    $"No compatible overload found for method '{resolution.OriginalMethodName}' with provided parameters.");
            }

            resolution.SourceMethod = bestMatch;
            resolution.ResolvedMethodName = bestMatch.Name;
            resolution.ResolvedParameters = resolution.OriginalParameters;

            // Apply custom attributes if present
            ApplyCustomMethodAttributes(resolution, bestMatch);

            // For development contracts, determine call flags based on method characteristics
            resolution.CallFlags = DetermineCallFlags(bestMatch);
        }

        /// <summary>
        /// Resolves method for deployed contracts using manifest information.
        /// </summary>
        private static void ResolveForDeployedContract(
            MethodResolutionInfo resolution,
            DeployedContractReference deployedRef)
        {
            var manifest = deployedRef.Manifest!.Value;
            var contractMethod = manifest.Abi.Methods
                .FirstOrDefault(m => m.Name.Equals(resolution.OriginalMethodName, StringComparison.OrdinalIgnoreCase));

            if (contractMethod.Name == null)
            {
                throw new MethodNotFoundException(
                    $"Method '{resolution.OriginalMethodName}' not found in deployed contract manifest.");
            }

            resolution.ResolvedMethodName = contractMethod.Name;
            resolution.ResolvedParameters = resolution.OriginalParameters;
            resolution.CallFlags = contractMethod.Safe ? CallFlags.ReadOnly : CallFlags.All;
            resolution.ContractMethod = contractMethod;

            // Validate parameters against manifest
            ValidateParametersAgainstManifest(resolution, contractMethod);
        }

        /// <summary>
        /// Resolves as standard method without additional validation.
        /// </summary>
        private static void ResolveAsStandardMethod(MethodResolutionInfo resolution)
        {
            resolution.ResolvedMethodName = resolution.OriginalMethodName;
            resolution.ResolvedParameters = resolution.OriginalParameters;
            resolution.CallFlags = CallFlags.All; // Default to full permissions
        }

        /// <summary>
        /// Applies custom method attributes to the resolution.
        /// </summary>
        private static void ApplyCustomMethodAttributes(MethodResolutionInfo resolution, MethodInfo method)
        {
            var customAttr = method.GetCustomAttribute<CustomMethodAttribute>();
            if (customAttr != null)
            {
                // Override method name if specified
                if (!string.IsNullOrEmpty(customAttr.MethodName))
                {
                    resolution.ResolvedMethodName = customAttr.MethodName;
                }

                // Override call flags
                resolution.CallFlags = customAttr.GetEffectiveCallFlags();

                // Validate parameters
                if (!customAttr.ValidateParameterConstraints(resolution.OriginalParameters))
                {
                    throw new ArgumentException(
                        $"Parameters for method '{method.Name}' do not meet the constraints specified in CustomMethodAttribute.");
                }

                // Transform parameters
                resolution.ResolvedParameters = customAttr.TransformParameters(resolution.OriginalParameters);
            }
            else
            {
                // Apply standard ContractMethodAttribute if present
                var contractAttr = method.GetCustomAttribute<ContractMethodAttribute>();
                if (contractAttr != null)
                {
                    if (!string.IsNullOrEmpty(contractAttr.MethodName))
                    {
                        resolution.ResolvedMethodName = contractAttr.MethodName;
                    }
                    resolution.CallFlags = contractAttr.GetEffectiveCallFlags();
                }
            }
        }

        /// <summary>
        /// Determines call flags based on method characteristics.
        /// </summary>
        private static CallFlags DetermineCallFlags(MethodInfo method)
        {
            // Check for attributes first
            var contractAttr = method.GetCustomAttribute<ContractMethodAttribute>();
            if (contractAttr != null)
            {
                return contractAttr.GetEffectiveCallFlags();
            }

            // Heuristic-based determination
            var methodName = method.Name.ToLowerInvariant();

            // Read-only method patterns
            if (methodName.StartsWith("get") || methodName.StartsWith("is") ||
                methodName.StartsWith("has") || methodName.StartsWith("check") ||
                methodName.StartsWith("view") || methodName.StartsWith("query") ||
                methodName.Contains("balance") || methodName.Contains("supply"))
            {
                return CallFlags.ReadOnly;
            }

            // Write methods
            if (methodName.StartsWith("set") || methodName.StartsWith("update") ||
                methodName.StartsWith("transfer") || methodName.StartsWith("mint") ||
                methodName.StartsWith("burn") || methodName.StartsWith("approve"))
            {
                return CallFlags.States | CallFlags.AllowNotify;
            }

            // Default to all permissions for unknown methods
            return CallFlags.All;
        }

        /// <summary>
        /// Finds the best matching method based on parameter compatibility.
        /// </summary>
        private static MethodInfo? FindBestMatchingMethod(MethodInfo[] methods, object?[]? parameters)
        {
            if (methods.Length == 1)
                return methods[0];

            var paramCount = parameters?.Length ?? 0;

            // First, try exact parameter count match
            var exactCountMatches = methods.Where(m => m.GetParameters().Length == paramCount).ToArray();
            if (exactCountMatches.Length == 1)
                return exactCountMatches[0];

            // If multiple matches, try parameter type compatibility
            foreach (var method in exactCountMatches)
            {
                if (IsParameterTypeCompatible(method, parameters))
                {
                    return method;
                }
            }

            // Fall back to first method
            return methods[0];
        }

        /// <summary>
        /// Checks parameter type compatibility.
        /// </summary>
        private static bool IsParameterTypeCompatible(MethodInfo method, object?[]? args)
        {
            var parameters = method.GetParameters();

            if (args == null)
                return parameters.Length == 0;

            if (parameters.Length != args.Length)
                return false;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (args[i] != null && !IsTypeCompatible(parameters[i].ParameterType, args[i].GetType()))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if two types are compatible for Neo contract invocation.
        /// </summary>
        private static bool IsTypeCompatible(Type parameterType, Type argumentType)
        {
            if (parameterType.IsAssignableFrom(argumentType))
                return true;

            // Neo-specific type compatibility
            if (parameterType == typeof(UInt160) && argumentType == typeof(string))
                return true;

            if (parameterType == typeof(UInt256) && argumentType == typeof(string))
                return true;

            if (parameterType == typeof(System.Numerics.BigInteger) && argumentType.IsPrimitive)
                return true;

            return false;
        }

        /// <summary>
        /// Validates parameters against contract manifest.
        /// </summary>
        private static void ValidateParametersAgainstManifest(
            MethodResolutionInfo resolution,
            ContractMethodDescriptor contractMethod)
        {
            var parameters = resolution.OriginalParameters;
            var expectedParams = contractMethod.Parameters;

            var paramCount = parameters?.Length ?? 0;
            if (paramCount != expectedParams.Length)
            {
                throw new ArgumentException(
                    $"Method '{contractMethod.Name}' expects {expectedParams.Length} parameters, but {paramCount} were provided.");
            }

            // Additional parameter type validation could be added here
        }

        /// <summary>
        /// Generates a cache key for method resolution.
        /// </summary>
        private static string GenerateCacheKey(
            IContractReference contractReference,
            string methodName,
            object?[]? parameters,
            Type? sourceType)
        {
            var paramSignature = parameters == null ? "void" :
                string.Join(",", parameters.Select(p => p?.GetType().Name ?? "null"));

            return $"{contractReference.Identifier}.{methodName}({paramSignature})" +
                   (sourceType != null ? $"@{sourceType.FullName}" : "");
        }

        /// <summary>
        /// Clears the method resolution cache.
        /// </summary>
        public static void ClearCache()
        {
            _resolutionCache.Clear();
        }
    }

    /// <summary>
    /// Contains information about method resolution results.
    /// </summary>
    public class MethodResolutionInfo
    {
        /// <summary>
        /// The original method name as requested.
        /// </summary>
        public string OriginalMethodName { get; set; } = string.Empty;

        /// <summary>
        /// The resolved method name to be used in Contract.Call.
        /// </summary>
        public string ResolvedMethodName { get; set; } = string.Empty;

        /// <summary>
        /// The original parameters as provided.
        /// </summary>
        public object?[]? OriginalParameters { get; set; }

        /// <summary>
        /// The resolved parameters after transformation.
        /// </summary>
        public object?[]? ResolvedParameters { get; set; }

        /// <summary>
        /// The call flags to use for the invocation.
        /// </summary>
        public CallFlags CallFlags { get; set; }

        /// <summary>
        /// The contract reference being resolved.
        /// </summary>
        public IContractReference ContractReference { get; set; } = null!;

        /// <summary>
        /// The source method info for development contracts.
        /// </summary>
        public MethodInfo? SourceMethod { get; set; }

        /// <summary>
        /// The contract method descriptor from manifest.
        /// </summary>
        public ContractMethodDescriptor? ContractMethod { get; set; }

        /// <summary>
        /// Whether the method was successfully resolved.
        /// </summary>
        public bool IsResolved { get; set; }

        /// <summary>
        /// Error message if resolution failed.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Exception that occurred during resolution.
        /// </summary>
        public Exception? ResolutionError { get; set; }
    }
}
