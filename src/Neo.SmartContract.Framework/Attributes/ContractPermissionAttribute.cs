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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ContractPermissionAttribute : Attribute
    {
        /// <summary>
        /// Specify which contract and methods are allowed to call from this contract.
        /// </summary>
        /// <param name="contract">Address of contract allowed to call from this contract</param>
        /// <param name="methods">Name of method allowed to call.</param>
        /// <remarks>
        /// If the contract is specified as <see cref="Permission.Any"/>, then this contract is allowed to call the specified methods <param name="methods"></param> of any other contract.
        /// If the method is specified as <see cref="Method.Any"/>, then all methods of the specified contract <param name="contract"></param> can be called.
        /// </remarks>
        public ContractPermissionAttribute(string contract, params string[] methods)
        {
        }
    }
}
