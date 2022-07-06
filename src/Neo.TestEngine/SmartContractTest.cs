// Copyright (C) 2015-2022 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.SmartContract;
using System.Collections.Generic;

namespace Neo.TestingEngine
{
    public class SmartContractTest
    {
        public readonly UInt160? scriptHash = null;
        public readonly string? nefPath = null;
        public readonly string methodName;
        public JArray methodParameters;
        public Dictionary<StorageKey, StorageItem> storage;
        public List<TestContract> contracts;
        public uint currentHeight = 0;
        public Signer[] signers;
        public TestBlock[] blocks;
        public Transaction? currentTx;

        public SmartContractTest(string path, string method, JArray parameters) : this(method, parameters)
        {
            nefPath = path;
        }

        public SmartContractTest(UInt160 scriptHash, string method, JArray parameters) : this(method, parameters)
        {
            this.scriptHash = scriptHash;
        }

        private SmartContractTest(string method, JArray parameters)
        {
            methodName = method;
            methodParameters = parameters;
            storage = new Dictionary<StorageKey, StorageItem>();
            contracts = new List<TestContract>();
            signers = new Signer[] { };
            blocks = new TestBlock[] { };
        }
    }
}
