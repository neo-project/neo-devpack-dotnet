// Copyright (C) 2015-2024 The Neo Project.
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
        [Syscall("System.Crypto.CheckSig")]
        public extern static bool CheckSig(ECPoint pubkey, ByteString signature);

        [Syscall("System.Crypto.CheckMultisig")]
        public extern static bool CheckMultisig(ECPoint[] pubkey, ByteString[] signature);
    }
}
