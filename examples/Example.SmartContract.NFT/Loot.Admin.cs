// Copyright (C) 2015-2024 The Neo Project.
//
// Loot.Admin.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo;
using Neo.SmartContract;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.Runtime.CompilerServices;

namespace NFT
{
    /// <summary>
    /// Security Requirements:
    ///  All public functions in this partial class
    ///  that has write permission must be owner only
    ///
    ///  [SetOwner] -- confirmed by jinghui
    ///  [_deploy]  -- except this one, confirmed by jinghui
    ///  [Update]   -- confirmed by jinghui
    ///  [Destroy]  -- confirmed by jinghui
    ///  [Pause]    -- confirmed by jinghui
    ///  [Resume]   -- confirmed by jinghui
    ///
    /// </summary>
    public partial class Loot
    {

        [Hash160("NaA5nQieb5YGg5nSFjhJMVEXQCQ5HdukwP")]
        static readonly UInt160 Owner = default;

        /// <summary>
        /// Security requirement:
        /// The prefix should be unique in the contract: checked globally.
        /// </summary>
        private static readonly StorageMap OwnerMap = new(Storage.CurrentContext, (byte)StoragePrefix.Owner);

        public static bool Verify() => Runtime.CheckWitness(GetOwner());


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void OwnerOnly() => Tools.Require(Verify(), "Authorization failed.");


        /// <summary>
        /// Security Requirements:
        /// <0> Only the owner of the contract
        /// are allowed to call this function: constrained internally
        ///
        /// <1> the new address should be
        /// a valid address: constrained internally
        ///
        /// </summary>
        /// <param name="newOwner"></param>
        /// <returns></returns>
        public static UInt160 SetOwner(UInt160 newOwner)
        {
            // <0> -- confirmed by jinghui
            OwnerOnly();
            // <1> -- confirmed by jinghui
            Tools.Require(newOwner.IsValid, "Loot::UInt160 is invalid.");
            OwnerMap.Put("owner", newOwner);
            return GetOwner();
        }

        [Safe]
        public static UInt160 GetOwner()
        {
            var owner = OwnerMap.Get("owner");
            return owner != null ? (UInt160)owner : Owner;
        }

        public static void _deploy(object _, bool update)
        {
            if (update) return;
        }

        public static void Update(ByteString nefFile, string manifest)
        {
            OwnerOnly();
            ContractManagement.Update(nefFile, manifest, null);
        }

        public static void Destroy()
        {
            OwnerOnly();
            ContractManagement.Destroy();
        }
    }
}
