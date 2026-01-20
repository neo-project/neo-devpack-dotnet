// Copyright (C) 2015-2026 The Neo Project.
//
// ContractManifest.cs file belongs to the neo project and is free
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
    /// Represents the manifest of a smart contract.
    /// When a smart contract is deployed, it must explicitly declare the features and permissions it will use.
    /// When it is running, it will be limited by its declared list of features and permissions, and cannot make any behavior beyond the scope of the list.
    /// </summary>
    /// <remarks>For more details, see NEP-15.</remarks>
    public struct ContractManifest
    {
        public string Name;
        public ContractGroup[] Groups;
        public readonly object Reserved;
        public string[] SupportedStandards;
        public ContractAbi Abi;
        public ContractPermission[] Permissions;
        public ByteString[] Trusts;
        public string Extra;
    }
}
