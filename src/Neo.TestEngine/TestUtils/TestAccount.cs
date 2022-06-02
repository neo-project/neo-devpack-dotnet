// Copyright (C) 2015-2022 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract;
using Neo.Wallets;
using System;

namespace Neo.TestingEngine
{
    class TestAccount : WalletAccount
    {
        public override bool HasKey => this.key != null;
        private readonly KeyPair? key = null;

        public TestAccount(UInt160 scriptHash) : base(scriptHash, ProtocolSettings.Default) { }
        public TestAccount(KeyPair privateKey) : base(Contract.CreateSignatureRedeemScript(privateKey.PublicKey).ToScriptHash(), ProtocolSettings.Default)
        {
            key = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            Contract = new()
            {
                Script = Contract.CreateSignatureRedeemScript(key.PublicKey),
                ParameterList = new[] { ContractParameterType.Signature }
            };
        }

        public override KeyPair? GetKey()
        {
            return this.key;
        }
    }
}
