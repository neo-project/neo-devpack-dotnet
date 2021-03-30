using Neo.IO.Json;
using Neo.SmartContract;

namespace Neo.TestingEngine
{
    class BuildNEF : BuildScript
    {
        public BuildNEF(NefFile nefFile, string manifestFile) : base(nefFile)
        {
            finalNEFScript = nefFile.Script;
            JObject manifestJson = JObject.Parse(manifestFile);
            var abi = manifestJson["abi"] as JObject;
            finalABI = abi;
            finalManifest = manifestFile;
            SetPropeties();
        }

        public BuildNEF(NefFile nefFile, JObject manifestJson) : base(nefFile)
        {
            finalNEFScript = nefFile.Script;
            var abi = manifestJson["abi"] as JObject;
            finalABI = abi;
            finalManifest = manifestJson.AsString();
            SetPropeties();
        }

        private void SetPropeties()
        {
            IsBuild = true;
            UseOptimizer = false;
            Error = null;
        }
    }
}
