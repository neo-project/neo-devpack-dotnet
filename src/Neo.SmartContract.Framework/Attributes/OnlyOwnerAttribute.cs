// Copyright (C) 2015-2026 The Neo Project.
//
// OnlyOwnerAttribute.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.Attributes
{
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false)]
    public class OnlyOwnerAttribute : ModifierAttribute
    {
        private readonly byte[] _key;

        public OnlyOwnerAttribute(byte prefix = 0xFF)
        {
            _key = [prefix];
        }

        public override void Enter()
        {
            UInt160? owner = (UInt160)Storage.Get(_key)!;
            if (owner is null || !Runtime.CheckWitness(owner))
                throw new InvalidOperationException("No Authorization!");
        }

        public override void Exit()
        {
        }
    }
}
