// Copyright (C) 2015-2026 The Neo Project.
//
// Ownable.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;

namespace Neo.SmartContract.Framework
{
    public abstract class Ownable : SmartContract
    {
        private const byte Prefix_Owner = 0xFF;

        [Safe]
        public static UInt160? GetOwner()
        {
            return (UInt160)Storage.Get(new[] { Prefix_Owner })!;
        }

        protected static bool IsOwner()
        {
            UInt160? owner = GetOwner();
            return owner is not null && Runtime.CheckWitness(owner);
        }

        public delegate void OnSetOwnerDelegate(UInt160? previousOwner, UInt160? newOwner);

        [DisplayName("SetOwner")]
        public static event OnSetOwnerDelegate OnSetOwner = null!;

        public static void SetOwner(UInt160 newOwner)
        {
            if (!IsOwner())
                throw new System.InvalidOperationException("No Authorization!");

            ExecutionEngine.Assert(newOwner.IsValid && !newOwner.IsZero, "owner must be valid");

            UInt160? previousOwner = GetOwner();
            ExecutionEngine.Assert(previousOwner != newOwner, "owner must change");

            Storage.Put(new[] { Prefix_Owner }, newOwner);
            OnSetOwner(previousOwner, newOwner);
        }

        protected static void InitializeOwner(object? data, bool update)
        {
            if (update)
                return;

            data ??= Runtime.Transaction.Sender;

            UInt160 initialOwner = (UInt160)data;
            ExecutionEngine.Assert(initialOwner.IsValid && !initialOwner.IsZero, "owner must be valid");

            Storage.Put(new[] { Prefix_Owner }, initialOwner);
            OnSetOwner(null, initialOwner);
        }
    }
}
