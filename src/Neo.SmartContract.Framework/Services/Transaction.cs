// Copyright (C) 2015-2024 The Neo Project.
//
// Transaction.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

namespace Neo.SmartContract.Framework.Services
{
    public class Transaction
    {
        public readonly UInt256 Hash;
        public readonly byte Version;
        public readonly uint Nonce;
        public readonly UInt160 Sender;
        public readonly long SystemFee;
        public readonly long NetworkFee;
        public readonly uint ValidUntilBlock;
        public readonly ByteString Script;
    }
}
