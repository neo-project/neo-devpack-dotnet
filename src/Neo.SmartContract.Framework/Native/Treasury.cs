// Copyright (C) 2015-2026 The Neo Project.
//
// Treasury.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

#pragma warning disable CS0626

using Neo.SmartContract.Framework.Attributes;

namespace Neo.SmartContract.Framework.Native
{
    [Contract("0x156326f25b1b5d839a4d326aeaa75383c9563ac1")]
    public class Treasury
    {
        [ContractHash]
        public static extern UInt160 Hash { get; }

        /// <summary>
        /// Verify checks whether the transaction is signed by the committee.
        /// </summary>
        public static extern bool Verify();
    }
}
