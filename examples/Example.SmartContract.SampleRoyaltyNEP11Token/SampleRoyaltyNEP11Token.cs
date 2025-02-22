// Copyright (C) 2015-2024 The Neo Project.
//
// SampleRoyaltyNEP11Token.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Interfaces;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System.ComponentModel;
using System.Numerics;

namespace NonDivisibleNEP11
{
    /// <inheritdoc />
    [DisplayName("SampleRoyaltyNEP11Token")]
    [ContractAuthor("core-dev", "dev@neo.org")]
    [ContractVersion("0.0.1")]
    [ContractDescription("A sample of NEP-11 Royalty Feature")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/")]
    [ContractPermission(Permission.Any, Method.Any)]
    [SupportedStandards(NepStandard.Nep11)]
    [SupportedStandards(NepStandard.Nep24)]
    public class SampleRoyaltyNEP11Token : Nep11Token<Nep11TokenState>, INep24
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
            ExecutionEngine.Assert(IsOwner(), "No Authorization!");
            ExecutionEngine.Assert(newOwner != null && newOwner.IsValid && !newOwner.IsZero, "Wrong newOwner");

            Storage.Put(new[] { PrefixOwner }, newOwner);
            OnSetOwner(newOwner);
        }

        #endregion

        #region Minter

        private const byte PrefixMinter = 0xfd;

        private const byte PrefixCounter = 0xee;

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

        public static void SetMinter(UInt160? newMinter)
        {
            ExecutionEngine.Assert(IsOwner(), "No Authorization!");
            ExecutionEngine.Assert(newMinter != null && newMinter.IsValid && !newMinter.IsZero, "Wrong newMinter");

            Storage.Put(new[] { PrefixMinter }, newMinter);
            OnSetMinter(newMinter);
        }

        public static void Mint(UInt160 to)
        {
            ExecutionEngine.Assert(IsOwner() || IsMinter(), "No Authorization!");
            IncreaseCount();
            BigInteger tokenId = CurrentCount();
            Nep11TokenState nep11TokenState = new Nep11TokenState()
            {
                Name = "SampleRoyaltyNep11Token",
                Owner = to
            };
            Mint((ByteString)tokenId, nep11TokenState);
        }

        private static void SetCount(BigInteger count)
        {
            Storage.Put(new[] { PrefixCounter }, count);
        }

        [Safe]
        public static BigInteger CurrentCount()
        {
            return (BigInteger)Storage.Get(new[] { PrefixCounter });
        }

        private static void IncreaseCount()
        {
            SetCount(CurrentCount() + 1);
        }

        #endregion

        #region Example.SmartContract.NEP11

        public override string Symbol { [Safe] get => "SampleRoyalty"; }

        #endregion

        #region Royalty
        private const byte PrefixRoyalty = 0xfb;

        private static readonly UInt160 InitialRecipient = "NUuJw4C4XJFzxAvSZnFTfsNoWZytmQKXQP";
        private static readonly BigInteger InitialFactor = 700;

        public static void SetRoyaltyInfo(ByteString tokenId, Map<string, object>[] royaltyInfos)
        {
            ExecutionEngine.Assert(IsOwner(), "No Authorization!");
            ExecutionEngine.Assert(tokenId.Length <= 64, "The argument \"tokenId\" should be 64 or less bytes long.");
            for (uint i = 0; i < royaltyInfos.Length; i++)
            {
                ExecutionEngine.Assert(((UInt160)royaltyInfos[i]["royaltyRecipient"]).IsValid == true && (BigInteger)royaltyInfos[i]["royaltyRecipient"] >= 0 && (BigInteger)royaltyInfos[i]["royaltyRecipient"] <= 10000, "Parameter error");
            }
            Storage.Put(PrefixRoyalty + tokenId, StdLib.Serialize(royaltyInfos));
        }

        /// <summary>
        /// This implements Royalty Standard: https://github.com/neo-project/proposals/pull/155/
        /// This method returns a map of NeoVM Array stack item with single or multi array, each array includes royaltyRecipient and royaltyAmount
        /// </summary>
        /// <param name="tokenId">tokenId</param>
        /// <param name="royaltyToken">royaltyToken hash for payment</param>
        /// <param name="salePrice">royaltyToken amount for payment</param>
        /// <returns>royaltyInfo</returns>
        [Safe]
        public static Map<string, object>[] RoyaltyInfo(ByteString tokenId, UInt160 royaltyToken, BigInteger salePrice)
        {
            ExecutionEngine.Assert(OwnerOf(tokenId) != null, "This TokenId doesn't exist!");
            byte[] data = (byte[])Storage.Get(PrefixRoyalty + tokenId);
            if (data == null)
            {
                var royaltyInfo = new Map<string, object>();
                royaltyInfo["royaltyRecipient"] = InitialRecipient;
                royaltyInfo["royaltyAmount"] = salePrice / InitialFactor;
                return new[] { royaltyInfo };
            }
            else
            {
                return (Map<string, object>[])StdLib.Deserialize((ByteString)data);
            }
        }
        #endregion

        #region Basic

        [Safe]
        public static bool Verify() => IsOwner();

        public static bool Update(ByteString nefFile, ByteString manifest, object data)
        {
            ExecutionEngine.Assert(IsOwner(), "No Authorization!");
            ContractManagement.Update(nefFile, manifest, data);
            return true;
        }

        #endregion
    }
}
