// Copyright (C) 2015-2024 The Neo Project.
//
// Nep17Token.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Numerics;

namespace NEP17
{
    /// <inheritdoc />
    [DisplayName("SampleNep17Token")]
    [ContractAuthor("core-dev", "dev@neo.org")]
    [ContractVersion("0.0.1")]
    [ContractDescription("A sample NEP-17 token")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(NepStandard.Nep17)]
    public class SampleNep17Token : Nep17Token
    {
        #region Owner

        private const byte PrefixOwner = 0xff;

        private static readonly UInt160 InitialOwner = "NUuJw4C4XJFzxAvSZnFTfsNoWZytmQKXQP";

        [Safe]
        public static UInt160 GetOwner()
        {
            var currentOwner = Storage.Get(new[] { PrefixOwner });

            if (currentOwner == null)
                return InitialOwner;

            return (UInt160)currentOwner;
        }

        private static bool IsOwner() => Runtime.CheckWitness(GetOwner());

        public delegate void OnSetOwnerDelegate(UInt160 newOwner);

        [DisplayName("SetOwner")]
        public static event OnSetOwnerDelegate OnSetOwner;

        public static void SetOwner(UInt160? newOwner)
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No Authorization!");
            if (newOwner != null && newOwner.IsValid)
            {
                Storage.Put(new[] { PrefixOwner }, newOwner);
                OnSetOwner(newOwner);
            }
        }

        #endregion

        #region Minter

        private const byte PrefixMinter = 0xfd;

        private static readonly UInt160 InitialMinter = "NUuJw4C4XJFzxAvSZnFTfsNoWZytmQKXQP";

        [Safe]
        public static UInt160 GetMinter()
        {
            var currentMinter = Storage.Get(new[] { PrefixMinter });

            if (currentMinter == null)
                return InitialMinter;

            return (UInt160)currentMinter;
        }

        private static bool IsMinter() => Runtime.CheckWitness(GetMinter());

        public delegate void OnSetMinterDelegate(UInt160 newMinter);

        [DisplayName("SetMinter")]
        public static event OnSetMinterDelegate OnSetMinter;

        public static void SetMinter(UInt160 newMinter)
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No Authorization!");
            if (!newMinter.IsValid) return;
            Storage.Put(new[] { PrefixMinter }, newMinter);
            OnSetMinter(newMinter);
        }

        public new static void Mint(UInt160 to, BigInteger amount)
        {
            if (IsOwner() == false && IsMinter() == false)
                throw new InvalidOperationException("No Authorization!");
            Nep17Token.Mint(to, amount);
        }

        #endregion

        #region Example.SmartContract.NEP17

        public override string Symbol { [Safe] get => "SampleNep17Token"; }
        public override byte Decimals { [Safe] get => 8; }

        public new static void Burn(UInt160 account, BigInteger amount)
        {
            if (IsOwner() == false && IsMinter() == false)
                throw new InvalidOperationException("No Authorization!");
            Nep17Token.Burn(account, amount);
        }

        #endregion

        #region Basic

        [Safe]
        public static bool Verify() => IsOwner();

        public static bool Update(ByteString nefFile, string manifest)
        {
            if (IsOwner() == false)
                throw new InvalidOperationException("No Authorization!");
            ContractManagement.Update(nefFile, manifest);
            return true;
        }

        #endregion
    }
}
