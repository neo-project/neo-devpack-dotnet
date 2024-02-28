// Copyright (C) 2015-2023 The Neo Project.
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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ContractPermissionAttribute : Attribute
    {
        /// <summary>
        /// Specify which contract and methods are allowed to call from this contract.
        /// </summary>
        /// <param name="contract">Address of contract allowed to call</param>
        /// <param name="methods">Name of method allowed to call.</param>
        /// <remarks>
        /// If the contract is specified as Permission.WildCard, then all contracts are allowed to call the specified methods.
        /// If the method is specified as Method.WildCard, then all methods are allowed to call from the specified contract.
        /// </remarks>
        public ContractPermissionAttribute(string contract, params string[] methods)
        {
        }
    }
}
