// Copyright (C) 2015-2024 The Neo Project.
//
// Token.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;
using System.Numerics;
namespace NFT
{
    public class TokenState : Nep11TokenState
    {
        public BigInteger TokenId;

        public BigInteger Credential;

        public static TokenState MintLoot(UInt160 owner, BigInteger tokenId, BigInteger credential) => new(owner, tokenId, credential);

        private TokenState(UInt160 owner, BigInteger tokenId, BigInteger credential)
        {
            Owner = owner;
            TokenId = tokenId;
            Credential = credential;
            Name = "N3 Secure Loot #" + TokenId;
        }

        public void OwnerOnly()
        {
            Tools.Require(Runtime.CheckWitness(Owner), "Authorization failed.");
        }
    }
}
