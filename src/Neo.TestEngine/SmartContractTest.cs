using Neo.IO.Json;
using Neo.Network.P2P.Payloads;
using Neo.VM.Types;
using System.Collections.Generic;

namespace Neo.TestingEngine
{
    public class SmartContractTest
    {
        public string nefPath;
        public string methodName;
        public JArray methodParameters;
        public Dictionary<PrimitiveType, StackItem> storage;
        public List<TestContract> contracts;
        public uint currentHeight = 0;
        public UInt160[] signers;
        public Block[] blocks;

        public SmartContractTest(string path, string method, JArray parameters)
        {
            nefPath = path;
            methodName = method;
            methodParameters = parameters;
            storage = new Dictionary<PrimitiveType, StackItem>();
            contracts = new List<TestContract>();
            signers = new UInt160[] { };
            blocks = new Block[] { };
        }
    }
}
