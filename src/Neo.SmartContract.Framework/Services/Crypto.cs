// Copyright (C) 2015-2026 The Neo Project.
//
// Crypto.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.Services
{
    public static class Crypto
    {
        /// <summary>
        /// Checks the signature for the current script container(for example: a transaction).
        /// True if the signature is valid, otherwise false.
        /// <para>
        /// The execution will fail if the format of pubkey is invalid.
        /// </para>
        /// </summary>
        [Syscall("System.Crypto.CheckSig")]
        public extern static bool CheckSig(ECPoint pubkey, ByteString signature);

        /// <summary>
        /// Checks the signatures for the current script container(for example: a transaction).
        /// True if the signatures are valid, otherwise false.
        /// <para>
        /// The execution will fail if:
        /// 1. the pubkeys or signatures is null orempty;
        /// 2. If signatures.Length > pubkeys.Length.
        /// 3. The format of any pubkey is invalid.
        /// </para>
        /// </summary>
        [Syscall("System.Crypto.CheckMultisig")]
        public extern static bool CheckMultisig(ECPoint[] pubkeys, ByteString[] signatures);
    }
}
