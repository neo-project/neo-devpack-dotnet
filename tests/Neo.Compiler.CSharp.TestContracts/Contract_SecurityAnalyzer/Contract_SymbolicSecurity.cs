// Copyright (C) 2015-2026 The Neo Project.
//
// Contract_SymbolicSecurity.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.Compiler.CSharp.TestContracts
{
    public class Contract_SymbolicSecurity : SmartContract.Framework.SmartContract
    {
        // Unguarded update should be flagged by symbolic execution analyzer.
        public static void UnguardedUpdate(ByteString nefFile, string manifest)
        {
            ContractManagement.Update(nefFile, manifest, null);
        }

        // Verify entrypoint that writes storage should be flagged.
        public static bool Verify()
        {
            Storage.Put("symbolic", "security");
            return true;
        }
    }
}
