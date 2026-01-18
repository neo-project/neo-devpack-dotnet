// Copyright (C) 2015-2026 The Neo Project.
//
// ContractGroup.cs file belongs to the neo project and is free
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
    /// Represents a set of mutually trusted contracts.
    /// A contract will trust and allow any contract in the same group to invoke it,
    /// and the user interface will not give any warnings.
    /// A group is identified by a public key and must be accompanied by a signature 
    /// for the contract hash to prove that the contract is indeed included in the group.
    /// </summary>
    public struct ContractGroup
    {
        /// <summary>
        /// The public key of the group.
        /// </summary>
        public ECPoint PubKey;

        /// <summary>
        /// The signature of the contract hash which can be verified by the `PubKey`.
        /// </summary>
        public ByteString Signature;
    }
}
