using Neo.IO.Json;
using Neo.SmartContract;

namespace Neo.TestingEngine
{
    class BuildNEF : BuildScript
    {
        public BuildNEF(NefFile nefFile, string manifestFile)
            : base(nefFile, manifestJson: JObject.Parse(manifestFile))
        {

        }
    }
}
