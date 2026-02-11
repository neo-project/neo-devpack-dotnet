// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_MissingCheckWitness.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_MissingCheckWitness : SmartContract.Framework.SmartContract
    {
        // Vulnerable: writes storage without CheckWitness
        public static void UnsafeUpdate(byte[] key, byte[] value)
        {
            var context = Storage.CurrentContext;
            Storage.Put(context, key, value);
        }

        // Safe: checks witness before writing storage
        public static void SafeUpdate(UInt160 owner, byte[] key, byte[] value)
        {
            ExecutionEngine.Assert(Runtime.CheckWitness(owner));
            var context = Storage.CurrentContext;
            Storage.Put(context, key, value);
        }
    }
}
