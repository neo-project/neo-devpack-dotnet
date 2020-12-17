using Neo.IO.Json;
using Neo.Ledger;
using Neo.Network.P2P.Payloads;
using System.Collections.Generic;

namespace Neo.TestingEngine
{
    public class SmartContractTest
    {
        public readonly string nefPath;
        public readonly string methodName;
        public JArray methodParameters;
        public Dictionary<StorageKey, StorageItem> storage;
        public List<TestContract> contracts;
        public uint currentHeight = 0;
        public UInt160[] signers;
        public Block[] blocks;

        public SmartContractTest(string path, string method, JArray parameters)
        {
            nefPath = path;
            methodName = method;
            methodParameters = parameters;
            storage = new Dictionary<StorageKey, StorageItem>();
            contracts = new List<TestContract>();
            signers = new UInt160[] { };
            blocks = new Block[] { };
        }
    }
}
