// Copyright (C) 2015-2026 The Neo Project.
//
// WhenPausedAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Services;
using System;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false)]
    public class WhenPausedAttribute : ModifierAttribute
    {
        private readonly byte[] _key;

        public WhenPausedAttribute(byte prefix = 0xFE)
        {
            _key = [prefix];
        }

        public override void Enter()
        {
            ExecutionEngine.Assert(Storage.Get(_key) is not null, "contract is not paused");
        }

        public override void Exit()
        {
        }
    }
}
