// Copyright (C) 2015-2024 The Neo Project.
//
// Tools.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Runtime.CompilerServices;

namespace NFT
{
    static class Tools
    {
        /// <summary>
        /// If the condition `istrue` does not hold,
        /// then the transaction throw exception
        /// making transaction `FAULT`
        /// </summary>
        /// <param name="isTrue">true condition, has to be true to run</param>
        /// <param name="msg">Transaction FAULT reason</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Require(bool isTrue, string msg = "Invalid") { if (!isTrue) throw new Exception($"Loot::{msg}"); }
    }
}
