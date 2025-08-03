// Copyright (C) 2015-2025 The Neo Project.
//
// ContractProxyBase.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.ContractInvocation
{
    /// <summary>
    /// Base class for generated contract proxies.
    /// This class provides the foundation for type-safe contract method invocation.
    /// </summary>
    public abstract class ContractProxyBase
    {
        /// <summary>
        /// Gets the contract reference for this proxy.
        /// </summary>
        protected IContractReference ContractReference { get; }

        /// <summary>
        /// Initializes a new ContractProxyBase with the specified contract reference.
        /// </summary>
        /// <param name="contractReference">The contract reference to proxy</param>
        protected ContractProxyBase(IContractReference contractReference)
        {
            ContractReference = contractReference ?? throw new ArgumentNullException(nameof(contractReference));
        }

        /// <summary>
        /// Invokes a contract method with the specified parameters.
        /// This method will be replaced by the compiler with appropriate Contract.Call instructions.
        /// </summary>
        /// <param name="method">The method name to invoke</param>
        /// <param name="flags">The call flags for the invocation</param>
        /// <param name="args">The method arguments</param>
        /// <returns>The result of the contract method call</returns>
        protected virtual object InvokeMethod(string method, CallFlags flags, params object?[]? args)
        {
            // Use MethodResolver for enhanced method resolution
            var resolution = MethodResolver.ResolveMethod(ContractReference, method, args, GetSourceContractType());
            
            if (!resolution.IsResolved)
            {
                throw new InvalidOperationException($"Failed to resolve method '{method}': {resolution.ErrorMessage}");
            }

            // Handle development contracts that aren't resolved yet
            if (ContractReference is DevelopmentContractReference devRef && !devRef.IsResolved)
            {
                return HandleDevelopmentContractInvocation(resolution);
            }

            // Ensure contract is resolved for deployed contracts
            if (!ContractReference.IsResolved)
            {
                throw new InvalidOperationException($"Contract reference '{ContractReference.Identifier}' is not resolved. Cannot invoke method '{method}'.");
            }

            // Use resolved method name and parameters
            return Contract.Call(ContractReference.ResolvedHash!, resolution.ResolvedMethodName, resolution.CallFlags, resolution.ResolvedParameters);
        }

        /// <summary>
        /// Gets the source contract type for development contracts.
        /// Override this in derived classes for development contract proxies.
        /// </summary>
        /// <returns>The source contract type, or null for deployed contracts</returns>
        protected virtual Type? GetSourceContractType()
        {
            return null;
        }

        /// <summary>
        /// Handles method invocation for development contracts.
        /// Override this to provide custom development-time behavior.
        /// </summary>
        /// <param name="resolution">The method resolution information</param>
        /// <returns>The result of the method invocation</returns>
        protected virtual object HandleDevelopmentContractInvocation(MethodResolutionInfo resolution)
        {
            // Default behavior: throw exception indicating contract not deployed
            throw new InvalidOperationException(
                $"Development contract '{ContractReference.Identifier}' is not yet compiled. " +
                $"Method '{resolution.OriginalMethodName}' cannot be invoked until contract is deployed. " +
                $"Consider overriding HandleDevelopmentContractInvocation for custom development-time behavior.");
        }

        /// <summary>
        /// Invokes a contract method with read-only access.
        /// </summary>
        /// <param name="method">The method name to invoke</param>
        /// <param name="args">The method arguments</param>
        /// <returns>The result of the contract method call</returns>
        protected object InvokeReadOnly(string method, params object?[]? args)
        {
            return InvokeMethod(method, CallFlags.ReadOnly, args);
        }

        /// <summary>
        /// Invokes a contract method with full access permissions.
        /// </summary>
        /// <param name="method">The method name to invoke</param>
        /// <param name="args">The method arguments</param>
        /// <returns>The result of the contract method call</returns>
        protected object InvokeWithAllFlags(string method, params object?[]? args)
        {
            return InvokeMethod(method, CallFlags.All, args);
        }

        /// <summary>
        /// Invokes a contract method with state modification permissions.
        /// </summary>
        /// <param name="method">The method name to invoke</param>
        /// <param name="args">The method arguments</param>
        /// <returns>The result of the contract method call</returns>
        protected object InvokeWithStates(string method, params object?[]? args)
        {
            return InvokeMethod(method, CallFlags.States, args);
        }

        /// <summary>
        /// Gets the contract hash for this proxy.
        /// </summary>
        /// <returns>The contract hash</returns>
        public UInt160 GetContractHash()
        {
            if (!ContractReference.IsResolved)
            {
                throw new InvalidOperationException($"Contract reference '{ContractReference.Identifier}' is not resolved.");
            }

            return ContractReference.ResolvedHash!;
        }

        /// <summary>
        /// Gets the contract identifier for this proxy.
        /// </summary>
        /// <returns>The contract identifier</returns>
        public string GetContractIdentifier()
        {
            return ContractReference.Identifier;
        }
    }
}