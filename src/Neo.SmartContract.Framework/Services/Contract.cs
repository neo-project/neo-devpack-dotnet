// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.SmartContract.Framework is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.Services
{
    public class Contract
    {
        /// <summary>
        /// Id
        /// </summary>
        public readonly int Id;

        /// <summary>
        /// UpdateCounter
        /// </summary>
        public readonly ushort UpdateCounter;

        /// <summary>
        /// Hash
        /// </summary>
        public readonly UInt160 Hash;

        /// <summary>
        /// Nef
        /// </summary>
        public readonly ByteString Nef;

        /// <summary>
        /// Manifest
        /// </summary>
        public readonly ContractManifest Manifest;

        [Syscall("System.Contract.Call")]
        public static extern object Call(UInt160 scriptHash, string method, CallFlags flags, params object[] args);

        [Syscall("System.Contract.GetCallFlags")]
        public static extern byte GetCallFlags();

        [Syscall("System.Contract.CreateStandardAccount")]
        public static extern UInt160 CreateStandardAccount(Cryptography.ECC.ECPoint pubKey);

        [Syscall("System.Contract.CreateMultisigAccount")]
        public static extern UInt160 CreateMultisigAccount(int m, params Cryptography.ECC.ECPoint[] pubKey);
    }
}
