using Neo.IO.Json;
using Neo.VM.Types;
using System.Collections.Generic;

namespace Neo.TestingEngine
{
    public class SmartContractTest
    {
        public string nefPath;
        public string methodName;
        public JArray methodParameters;
        public Dictionary<PrimitiveType, StackItem> storage = new Dictionary<PrimitiveType, StackItem>();
        public List<TestContract> contracts = new List<TestContract>();
        public uint currentHeight = 0;

        public SmartContractTest(string path, string method, JArray parameters)
        {
            nefPath = path;
            methodName = method;
            methodParameters = parameters;
            storage.Clear();
            contracts.Clear();
        }
    }
}
