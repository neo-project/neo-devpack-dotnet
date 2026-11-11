// Copyright (C) 2015-2026 The Neo Project.
//
// Nep11Token.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract.Framework.Attributes;
using Neo.SmartContract.Framework.Interfaces;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Neo.SmartContract.Framework
{
    [SupportedStandards(NepStandard.Nep11)]
    [ContractPermission(Permission.Any, Method.OnNEP11Payment)]
    [ExcludeFromCodeCoverage]
    public abstract class Nep11Token<TokenState> : TokenContract, INEP11
        where TokenState : Nep11TokenState
    {
        public delegate void OnTransferDelegate(UInt160? from, UInt160? to, BigInteger amount, ByteString tokenId);

        [DisplayName("Transfer")]
        public static event OnTransferDelegate OnTransfer = null!;

        protected const byte Prefix_TokenId = 0x02;
        protected const byte Prefix_Token = 0x03;
        protected const byte Prefix_AccountToken = 0x04;

        /// <summary>
        /// Builds a storage key with the same helper used by every access to this namespace.
        /// The key is typically a fixed prefix followed by the caller-provided key.
        /// </summary>
        /// <remarks>
        /// This raw-key form is a performance optimization. The recommended way is to use
        /// <see cref="StorageMap"/> / <see cref="LocalStorageMap"/>, which is safer against storage injection.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static byte[] TokenKey(ByteString tokenId) => new byte[] { Prefix_Token }.Concat(tokenId);

        /// <summary>
        /// Builds the account namespace prefix with the same helper used by every access.
        /// The resulting storage key is a fixed prefix followed by the account.
        /// </summary>
        /// <remarks>
        /// This raw-key form is a performance optimization. The recommended way is to use
        /// <see cref="StorageMap"/> / <see cref="LocalStorageMap"/>, which is safer against storage injection.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static byte[] AccountKey(ByteString account) => new byte[] { Prefix_AccountToken }.Concat(account);

        /// <summary>
        /// Builds an account-token storage key with the same helper used by every access.
        /// The key is a fixed prefix followed by the account and token ID.
        /// </summary>
        /// <remarks>
        /// This raw-key form is a performance optimization. The recommended way is to use
        /// <see cref="StorageMap"/> / <see cref="LocalStorageMap"/>, which is safer against storage injection.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static byte[] AccountTokenKey(ByteString account, ByteString tokenId) =>
                    new byte[] { Prefix_AccountToken }.Concat(account).Concat(tokenId);

        [Safe]
        public sealed override byte Decimals => 0;

        [Safe]
        public static UInt160 OwnerOf(ByteString tokenId)
        {
            if (tokenId.Length > 43) throw new Exception("The argument \"tokenId\" should be 43 or less bytes long.");
            var key = TokenKey(tokenId);
            var value = Storage.Get(key) ?? throw new Exception("The token with given \"tokenId\" does not exist.");
            TokenState token = (TokenState)StdLib.Deserialize(value);
            return token.Owner;
        }

        [Safe]
        public virtual Map<string, object> Properties(ByteString tokenId)
        {
            if (tokenId.Length > 43) throw new Exception("The argument \"tokenId\" should be 43 or less bytes long.");

            var key = TokenKey(tokenId);
            var value = Storage.Get(key) ?? throw new Exception("The token with given \"tokenId\" does not exist.");
            TokenState token = (TokenState)StdLib.Deserialize(value);
            return new Map<string, object>()
            {
                ["name"] = token.Name
            };
        }

        [Safe]
        public static Iterator Tokens()
        {
            var tokenPrefix = new byte[] { Prefix_Token };
            return Storage.Find(tokenPrefix, FindOptions.KeysOnly | FindOptions.RemovePrefix);
        }

        [Safe]
        public static Iterator TokensOf(UInt160 owner)
        {
            if (!owner.IsValid) throw new Exception("The argument \"owner\" is invalid");

            var accountPrefix = AccountKey(owner);
            return Storage.Find(accountPrefix, FindOptions.KeysOnly | FindOptions.RemovePrefix);
        }

        public static bool Transfer(UInt160 to, ByteString tokenId, object? data = null)
        {
            if (!to.IsValid) throw new Exception("The argument \"to\" is invalid.");
            if (tokenId.Length > 43) throw new Exception("The argument \"tokenId\" should be 43 or less bytes long.");

            var key = TokenKey(tokenId);
            var value = Storage.Get(key) ?? throw new Exception("The token with given \"tokenId\" does not exist.");
            TokenState token = (TokenState)StdLib.Deserialize(value);
            UInt160 from = token.Owner;
            if (!Runtime.CheckWitness(from)) return false;

            if (from != to)
            {
                token.Owner = to;
                Storage.Put(key, StdLib.Serialize(token));
                UpdateBalance(from, tokenId, -1);
                UpdateBalance(to, tokenId, +1);
            }
            PostTransfer(from, to, tokenId, data);
            return true;
        }

        protected static ByteString NewTokenId()
        {
            return NewTokenId(Runtime.ExecutingScriptHash);
        }

        protected static ByteString NewTokenId(ByteString salt)
        {
            StorageContext context = Storage.CurrentContext;
            byte[] key = new byte[] { Prefix_TokenId };
            ByteString? id = Storage.Get(context, key);
            Storage.Put(context, key, (BigInteger)(id ?? ByteString.Empty) + 1);
            if (id is not null) salt += id;
            return CryptoLib.Sha256(salt);
        }

        protected static void Mint(ByteString tokenId, TokenState token)
        {
            if (tokenId.Length > 43) throw new Exception("The argument \"tokenId\" should be 43 or less bytes long.");

            var key = TokenKey(tokenId);
            var value = Storage.Get(key);
            if (value is not null) throw new InvalidOperationException("The token with given \"tokenId\" already exists.");

            Storage.Put(key, StdLib.Serialize(token));
            UpdateBalance(token.Owner, tokenId, +1);
            TotalSupply++;
            PostTransfer(null, token.Owner, tokenId, null);
        }

        protected static void Burn(ByteString tokenId)
        {
            var key = TokenKey(tokenId);
            var value = Storage.Get(key) ?? throw new Exception("The token with given \"tokenId\" does not exist.");
            TokenState token = (TokenState)StdLib.Deserialize(value);

            Storage.Delete(key);
            UpdateBalance(token.Owner, tokenId, -1);
            TotalSupply--;
            PostTransfer(token.Owner, null, tokenId, null);
        }

        protected static void UpdateBalance(UInt160 owner, ByteString tokenId, int increment)
        {
            UpdateBalance(owner, increment);

            var key = AccountTokenKey(owner, tokenId);
            if (increment > 0)
                Storage.Put(key, 0);
            else
                Storage.Delete(key);
        }

        protected static void PostTransfer(UInt160? from, UInt160? to, ByteString tokenId, object? data)
        {
            OnTransfer(from, to, 1, tokenId);
            if (to is not null && ContractManagement.IsContract(to))
                Contract.Call(to, Method.OnNEP11Payment, CallFlags.All, from, 1, tokenId, data);
        }
    }
}
