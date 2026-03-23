// Copyright (C) 2015-2026 The Neo Project.
//
// Pausable.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework
{
    public abstract class Pausable : SmartContract
    {
        private const byte Prefix_Paused = 0xFE;

        [Safe]
        public static bool Paused => Storage.Get(new[] { Prefix_Paused }) is not null;

        protected static void Pause()
        {
            ExecutionEngine.Assert(!Paused, "contract is paused");
            Storage.Put(new[] { Prefix_Paused }, 1);
        }

        protected static void Unpause()
        {
            ExecutionEngine.Assert(Paused, "contract is not paused");
            Storage.Delete(new[] { Prefix_Paused });
        }
    }
}
