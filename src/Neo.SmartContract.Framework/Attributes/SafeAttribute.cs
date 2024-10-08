// Copyright (C) 2015-2024 The Neo Project.
//
// The Neo.SmartContract.Framework is free software distributed under the MIT
// software license, see the accompanying file LICENSE in the main directory
// of the project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;

namespace Neo.SmartContract.Framework.Attributes
{
    /// <summary>
    /// Indicates that a method is safe to call during read-only operations.
    /// </summary>
    /// <remarks>
    /// Methods marked with this attribute are considered read-only and do not modify the contract's state.
    /// They can be called without incurring GAS costs and are typically used for querying data.
    ///
    /// It's important to note that any attempt to perform state-changing operations within a method
    /// marked as [Safe] will result in the execution failing. This is a security measure to ensure
    /// that methods declared as safe truly remain read-only.
    ///
    /// Safe methods are particularly useful for:
    /// - Retrieving contract data
    /// - Performing calculations based on existing state
    /// - Validating inputs without modifying state
    ///
    /// Example usage:
    /// <code>
    /// [Safe]
    /// public static string GetName()
    /// {
    ///     return "MyContract";
    /// }
    /// </code>
    ///
    /// Incorrect usage (will fail execution):
    /// <code>
    /// [Safe]
    /// public static void UpdateName(string newName)
    /// {
    ///     // This will cause the execution to fail because it attempts to modify state
    ///     Storage.Put("ContractName", newName);
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class SafeAttribute : Attribute
    {
    }
}
