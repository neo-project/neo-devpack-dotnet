// Copyright (C) 2015-2026 The Neo Project.
//
// SampleDivisibleNep11Token.cs file belongs to the neo project and is free
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

namespace DivisibleNEP11
{
    [DisplayName("SampleDivisibleNep11Token")]
    [ContractAuthor("core-dev", "dev@neo.org")]
    [ContractVersion("0.0.1")]
    [ContractDescription("A sample divisible NEP-11 token")]
    [ContractSourceCode("https://github.com/neo-project/neo-devpack-dotnet/tree/master/examples/")]
    [ContractPermission(Permission.Any, Method.OnNEP11Payment)]
    [SupportedStandards(NepStandard.Nep11)]
    public class SampleDivisibleNep11Token : SmartContract
    {
        private const byte PrefixOwner = 0xff;
        private const byte PrefixTotalSupply = 0x01;
        private const byte PrefixTokenSupply = 0x02;
        private const byte PrefixTokenName = 0x03;
        private const byte PrefixBalance = 0x04;
        private const byte PrefixOwnerBalance = 0x05;
        private const byte PrefixAccountToken = 0x06;
        private const byte PrefixTokenOwner = 0x07;

        private const byte TokenDecimals = 8;
        private const long TokenUnit = 100_000_000;

        private static readonly UInt160 InitialOwner = Neo.SmartContract.Framework.UInt160.Parse("NUuJw4C4XJFzxAvSZnFTfsNoWZytmQKXQP");
        public delegate void OnTransferDelegate(UInt160? from, UInt160? to, BigInteger amount, ByteString tokenId);

        [DisplayName("Transfer")]
        public static event OnTransferDelegate OnTransfer = null!;

        [Safe]
        public static string Symbol() => "DNEP11";

        [Safe]
        public static byte Decimals() => TokenDecimals;

        [Safe]
        public static BigInteger TotalSupply() => GetInteger(TotalSupplyKey());

        [Safe]
        public static BigInteger BalanceOf(UInt160 owner)
        {
            ValidateOwner(owner);
            return GetInteger(MapKey(PrefixOwnerBalance, owner));
        }

        [Safe]
        public static BigInteger BalanceOf(UInt160 owner, ByteString tokenId)
        {
            ValidateOwner(owner);
            ValidateTokenId(tokenId);
            return GetInteger(BalanceKey(owner, tokenId));
        }

        [Safe]
        public static Iterator OwnerOf(ByteString tokenId)
        {
            EnsureTokenExists(tokenId);
            return Storage.Find(MapKey(PrefixTokenOwner, TokenHash(tokenId)), FindOptions.ValuesOnly);
        }

        [Safe]
        public static Map<string, object> Properties(ByteString tokenId)
        {
            EnsureTokenExists(tokenId);

            return new Map<string, object>
            {
                ["name"] = (string)Storage.Get(MapKey(PrefixTokenName, tokenId))!,
                ["decimals"] = TokenDecimals,
                ["totalSupply"] = GetInteger(MapKey(PrefixTokenSupply, tokenId))
            };
        }

        [Safe]
        public static Iterator Tokens()
        {
            return Storage.Find(MapPrefix(PrefixTokenSupply), FindOptions.KeysOnly | FindOptions.RemovePrefix);
        }

        [Safe]
        public static Iterator TokensOf(UInt160 owner)
        {
            ValidateOwner(owner);
            return Storage.Find(MapKey(PrefixAccountToken, owner), FindOptions.ValuesOnly);
        }

        public static bool Transfer(UInt160 from, UInt160 to, BigInteger amount, ByteString tokenId, object? data = null)
        {
            ValidateOwner(from);
            ValidateOwner(to);
            ValidateTokenId(tokenId);
            EnsureTokenExists(tokenId);
            ValidateTransferAmount(amount);
            if (!Runtime.CheckWitness(from)) return false;
            if (BalanceOf(from, tokenId) < amount) return false;

            if (amount != 0 && from != to)
            {
                UpdateBalance(from, tokenId, -amount);
                UpdateBalance(to, tokenId, amount);
            }

            PostTransfer(from, to, amount, tokenId, data);
            return true;
        }

        [Safe]
        public static UInt160 GetOwner()
        {
            ByteString? currentOwner = Storage.Get(new[] { PrefixOwner });
            return currentOwner is null ? InitialOwner : (UInt160)currentOwner;
        }

        public static void SetOwner(UInt160 newOwner)
        {
            if (!Runtime.CheckWitness(GetOwner())) throw new InvalidOperationException("No authorization.");
            ValidateOwner(newOwner);
            Storage.Put(new[] { PrefixOwner }, newOwner);
        }

        public static void Mint(UInt160 to, ByteString tokenId, BigInteger amount, string name)
        {
            if (!Runtime.CheckWitness(GetOwner())) throw new InvalidOperationException("No authorization.");
            ValidateOwner(to);
            ValidateTokenId(tokenId);
            ValidateMintAmount(amount);
            if (Storage.Get(MapKey(PrefixTokenSupply, tokenId)) is not null) throw new InvalidOperationException("Token already exists.");

            Storage.Put(MapKey(PrefixTokenName, tokenId), name);
            Storage.Put(MapKey(PrefixTokenSupply, tokenId), amount);
            Storage.Put(TotalSupplyKey(), TotalSupply() + amount);
            UpdateBalance(to, tokenId, amount);
            PostTransfer(null, to, amount, tokenId, null);
        }

        public static void Burn(UInt160 from, ByteString tokenId, BigInteger amount)
        {
            ValidateOwner(from);
            ValidateTokenId(tokenId);
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (!Runtime.CheckWitness(from)) throw new InvalidOperationException("No authorization.");
            if (BalanceOf(from, tokenId) < amount) throw new InvalidOperationException("Insufficient balance.");

            UpdateBalance(from, tokenId, -amount);
            BigInteger newTokenSupply = GetInteger(MapKey(PrefixTokenSupply, tokenId)) - amount;
            if (newTokenSupply < 0) throw new InvalidOperationException("Token supply cannot be negative.");
            if (newTokenSupply == 0)
            {
                Storage.Delete(MapKey(PrefixTokenSupply, tokenId));
                Storage.Delete(MapKey(PrefixTokenName, tokenId));
            }
            else
            {
                Storage.Put(MapKey(PrefixTokenSupply, tokenId), newTokenSupply);
            }
            Storage.Put(TotalSupplyKey(), TotalSupply() - amount);
            PostTransfer(from, null, amount, tokenId, null);
        }

        [Safe]
        public static bool Verify() => Runtime.CheckWitness(GetOwner());

        public static bool Update(ByteString nefFile, string manifest)
        {
            if (!Runtime.CheckWitness(GetOwner())) throw new InvalidOperationException("No authorization.");
            ContractManagement.Update(nefFile, manifest);
            return true;
        }

        private static void UpdateBalance(UInt160 owner, ByteString tokenId, BigInteger delta)
        {
            ByteString balanceKey = BalanceKey(owner, tokenId);
            BigInteger newBalance = GetInteger(balanceKey) + delta;
            if (newBalance < 0) throw new InvalidOperationException("Balance cannot be negative.");

            ByteString accountTokenKey = AccountTokenKey(owner, tokenId);
            ByteString tokenOwnerKey = TokenOwnerKey(tokenId, owner);
            if (newBalance == 0)
            {
                Storage.Delete(balanceKey);
                Storage.Delete(accountTokenKey);
                Storage.Delete(tokenOwnerKey);
            }
            else
            {
                Storage.Put(balanceKey, newBalance);
                Storage.Put(accountTokenKey, tokenId);
                Storage.Put(tokenOwnerKey, owner);
            }

            ByteString ownerBalanceKey = MapKey(PrefixOwnerBalance, owner);
            BigInteger ownerBalance = GetInteger(ownerBalanceKey) + delta;
            if (ownerBalance == 0)
                Storage.Delete(ownerBalanceKey);
            else
                Storage.Put(ownerBalanceKey, ownerBalance);
        }

        private static void PostTransfer(UInt160? from, UInt160? to, BigInteger amount, ByteString tokenId, object? data)
        {
            OnTransfer(from, to, amount, tokenId);
            if (to is not null && ContractManagement.GetContract(to) is not null)
                Contract.Call(to, Method.OnNEP11Payment, CallFlags.All, from, amount, tokenId, data);
        }

        private static ByteString BalanceKey(UInt160 owner, ByteString tokenId) => MapKey(PrefixBalance, owner + TokenHash(tokenId));

        private static ByteString AccountTokenKey(UInt160 owner, ByteString tokenId) => MapKey(PrefixAccountToken, owner + TokenHash(tokenId));

        private static ByteString TokenOwnerKey(ByteString tokenId, UInt160 owner) => MapKey(PrefixTokenOwner, TokenHash(tokenId) + owner);

        private static ByteString TokenHash(ByteString tokenId) => CryptoLib.Sha256(tokenId);

        private static ByteString TotalSupplyKey() => MapPrefix(PrefixTotalSupply);

        private static ByteString MapKey(byte prefix, ByteString key) => MapPrefix(prefix) + key;

        private static ByteString MapPrefix(byte prefix) => (ByteString)new byte[] { prefix };

        private static BigInteger GetInteger(ByteString key)
        {
            ByteString? value = Storage.Get(key);
            return value is null ? 0 : (BigInteger)value;
        }

        private static void EnsureTokenExists(ByteString tokenId)
        {
            ValidateTokenId(tokenId);
            if (Storage.Get(MapKey(PrefixTokenSupply, tokenId)) is null) throw new InvalidOperationException("Token does not exist.");
        }

        private static void ValidateOwner(UInt160 owner)
        {
            if (!owner.IsValid) throw new ArgumentException("Invalid owner.");
        }

        private static void ValidateTokenId(ByteString tokenId)
        {
            if (tokenId.Length == 0 || tokenId.Length > 64)
                throw new ArgumentException("Token ID must be between 1 and 64 bytes.");
        }

        private static void ValidateTransferAmount(BigInteger amount)
        {
            if (amount < 0 || amount > TokenUnit) throw new ArgumentOutOfRangeException(nameof(amount));
        }

        private static void ValidateMintAmount(BigInteger amount)
        {
            if (amount <= 0 || amount > TokenUnit) throw new ArgumentOutOfRangeException(nameof(amount));
        }
    }
}
