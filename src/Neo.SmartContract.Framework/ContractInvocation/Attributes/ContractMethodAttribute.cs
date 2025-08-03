// Copyright (C) 2015-2025 The Neo Project.
//
// ContractMethodAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.ContractInvocation.Attributes
{
    /// <summary>
    /// Specifies metadata for a contract method proxy.
    /// This attribute is used to customize how method calls are translated to contract invocations.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ContractMethodAttribute : Attribute
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
        /// Initializes a new ContractMethodAttribute.
        /// </summary>
        public ContractMethodAttribute()
        {
        }

        /// <summary>
        /// Initializes a new ContractMethodAttribute with the specified method name.
        /// </summary>
        /// <param name="methodName">The method name in the target contract</param>
        public ContractMethodAttribute(string methodName)
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
    }
}
