// Copyright (C) 2015-2025 The Neo Project.
//
// CustomMethodAttribute.cs file belongs to the neo project and is free
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
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.ContractInvocation.Attributes
{
    /// <summary>
    /// Specifies custom method mapping for non-standard contract methods.
    /// Allows flexible parameter transformation and method name mapping.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class CustomMethodAttribute : Attribute
    {
        /// <summary>
        /// Gets the method name in the target contract.
        /// If not specified, the proxy method name is used.
        /// </summary>
        public string? MethodName { get; set; }

        /// <summary>
        /// Gets or sets the call flags for this method invocation.
        /// </summary>
        public CallFlags CallFlags { get; set; } = CallFlags.All;

        /// <summary>
        /// Gets or sets whether this method is read-only.
        /// When true, CallFlags.ReadOnly is used regardless of the CallFlags setting.
        /// </summary>
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets whether this method modifies state.
        /// When true, CallFlags.States is included in the call flags.
        /// </summary>
        public bool ModifiesState { get; set; } = true;

        /// <summary>
        /// Gets or sets whether this method can make contract calls.
        /// When true, CallFlags.AllowCall is included in the call flags.
        /// </summary>
        public bool AllowCall { get; set; } = true;

        /// <summary>
        /// Gets or sets whether this method can emit notifications.
        /// When true, CallFlags.AllowNotify is included in the call flags.
        /// </summary>
        public bool AllowNotify { get; set; } = true;

        /// <summary>
        /// Gets or sets the parameter transformation strategy.
        /// </summary>
        public ParameterTransformStrategy ParameterTransform { get; set; } = ParameterTransformStrategy.None;

        /// <summary>
        /// Gets or sets custom parameter serialization format.
        /// </summary>
        public string? CustomParameterFormat { get; set; }

        /// <summary>
        /// Gets or sets whether to validate parameters at runtime.
        /// </summary>
        public bool ValidateParameters { get; set; } = true;

        /// <summary>
        /// Gets or sets the expected return type for validation.
        /// </summary>
        public Type? ExpectedReturnType { get; set; }

        /// <summary>
        /// Gets or sets whether this method supports array parameters.
        /// </summary>
        public bool SupportsArrayParameters { get; set; } = true;

        /// <summary>
        /// Gets or sets whether this method supports object parameters.
        /// </summary>
        public bool SupportsObjectParameters { get; set; } = true;

        /// <summary>
        /// Gets or sets the minimum number of parameters required.
        /// </summary>
        public int MinParameters { get; set; } = 0;

        /// <summary>
        /// Gets or sets the maximum number of parameters allowed (-1 for unlimited).
        /// </summary>
        public int MaxParameters { get; set; } = -1;

        /// <summary>
        /// Initializes a new CustomMethodAttribute.
        /// </summary>
        public CustomMethodAttribute()
        {
        }

        /// <summary>
        /// Initializes a new CustomMethodAttribute with the specified target method name.
        /// </summary>
        /// <param name="methodName">The actual method name in the contract</param>
        public CustomMethodAttribute(string methodName)
        {
            MethodName = methodName;
        }

        /// <summary>
        /// Gets the effective call flags based on the attribute settings.
        /// </summary>
        /// <returns>The computed call flags</returns>
        public CallFlags GetEffectiveCallFlags()
        {
            if (ReadOnly)
                return CallFlags.ReadOnly;

            var flags = CallFlags.None;

            if (ModifiesState)
                flags |= CallFlags.States;
            if (AllowCall)
                flags |= CallFlags.AllowCall;
            if (AllowNotify)
                flags |= CallFlags.AllowNotify;

            return flags == CallFlags.None ? CallFlags : flags;
        }

        /// <summary>
        /// Validates parameters according to the configured constraints.
        /// </summary>
        /// <param name="parameters">The parameters to validate</param>
        /// <returns>True if parameters are valid</returns>
        public bool ValidateParameterConstraints(object?[]? parameters)
        {
            if (!ValidateParameters)
                return true;

            var paramCount = parameters?.Length ?? 0;

            // Check parameter count constraints
            if (paramCount < MinParameters)
                return false;

            if (MaxParameters >= 0 && paramCount > MaxParameters)
                return false;

            if (parameters == null)
                return paramCount == 0;

            // Check parameter type constraints
            foreach (var param in parameters)
            {
                if (param == null)
                    continue;

                var paramType = param.GetType();

                if (!SupportsArrayParameters && paramType.IsArray)
                    return false;

                if (!SupportsObjectParameters && !paramType.IsPrimitive && 
                    paramType != typeof(string) && paramType != typeof(UInt160) && 
                    paramType != typeof(UInt256) && paramType != typeof(ECPoint))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Transforms parameters according to the specified strategy.
        /// </summary>
        /// <param name="parameters">The original parameters</param>
        /// <returns>The transformed parameters</returns>
        public object?[]? TransformParameters(object?[]? parameters)
        {
            if (parameters == null || ParameterTransform == ParameterTransformStrategy.None)
                return parameters;

            return ParameterTransform switch
            {
                ParameterTransformStrategy.SerializeToByteArray => SerializeParametersToByteArray(parameters),
                ParameterTransformStrategy.WrapInArray => new object[] { parameters },
                ParameterTransformStrategy.FlattenArrays => FlattenArrayParameters(parameters),
                ParameterTransformStrategy.Custom => ApplyCustomTransformation(parameters),
                _ => parameters
            };
        }

        private object?[]? SerializeParametersToByteArray(object?[] parameters)
        {
            // Implementation would serialize parameters to byte array
            // This is a placeholder for the actual serialization logic
            return new object[] { SerializeToBytes(parameters) };
        }

        private object?[]? FlattenArrayParameters(object?[] parameters)
        {
            var flattened = new System.Collections.Generic.List<object?>();
            
            foreach (var param in parameters)
            {
                if (param is Array array)
                {
                    foreach (var item in array)
                    {
                        flattened.Add(item);
                    }
                }
                else
                {
                    flattened.Add(param);
                }
            }

            return flattened.ToArray();
        }

        private object?[]? ApplyCustomTransformation(object?[] parameters)
        {
            // Custom transformation logic based on CustomParameterFormat
            // This would be implemented based on specific needs
            return parameters;
        }

        private byte[] SerializeToBytes(object?[] parameters)
        {
            // Placeholder for actual serialization implementation
            return new byte[0];
        }
    }

    /// <summary>
    /// Specifies parameter transformation strategies for custom methods.
    /// </summary>
    public enum ParameterTransformStrategy
    {
        /// <summary>
        /// No parameter transformation.
        /// </summary>
        None,

        /// <summary>
        /// Serialize all parameters into a single byte array.
        /// </summary>
        SerializeToByteArray,

        /// <summary>
        /// Wrap all parameters in a single array parameter.
        /// </summary>
        WrapInArray,

        /// <summary>
        /// Flatten array parameters into individual parameters.
        /// </summary>
        FlattenArrays,

        /// <summary>
        /// Apply custom transformation based on CustomParameterFormat.
        /// </summary>
        Custom
    }
}