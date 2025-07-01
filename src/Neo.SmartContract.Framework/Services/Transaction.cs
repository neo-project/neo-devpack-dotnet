// Copyright (C) 2015-2025 The Neo Project.
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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
#pragma warning restore CS8618
}
