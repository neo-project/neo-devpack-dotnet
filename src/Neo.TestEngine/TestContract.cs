using Neo.SmartContract;

namespace Neo.TestingEngine
{
    public class TestContract
    {
        internal string nefPath;
        internal NefFile nefFile = null;

        public TestContract(string path)
        {
            nefPath = path;
        }
    }
}
