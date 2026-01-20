// Copyright (C) 2015-2026 The Neo Project.
//
// ContractPermission.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Framework.Services
{
    /// <summary>
    /// Represents a permission of a contract. It describes which contracts may be
    /// invoked and which methods are called.
    /// If a contract invokes a contract or method that is not declared in the manifest
    /// at runtime, the invocation will fail.
    /// </summary>
    public struct ContractPermission
    {
        /// <summary>
        /// Indicates which contract to be invoked.
        /// It can be a hash of a contract, a public key of a group, or a wildcard *.
        /// If it specifies a hash of a contract, then the contract will be invoked;
        /// If it specifies a public key of a group, then any contract in this group
        /// may be invoked; If it specifies a wildcard *, then any contract may be invoked.
        /// </summary>
        public ByteString Contract;

        /// <summary>
        /// Indicates which methods to be called.
        /// It can also be assigned with a wildcard *. If it is a wildcard *,
        /// then it means that any method can be called.
        /// </summary>
        public string[] Methods;
    }
}
