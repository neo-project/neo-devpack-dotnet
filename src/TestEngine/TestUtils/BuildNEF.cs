using Neo.IO.Json;
using Neo.SmartContract;

namespace Neo.Compiler.MSIL.UnitTests.Utils
{
    class BuildNEF : BuildScript
    {
        public BuildNEF(NefFile nefFile, string manifestFile) : base()
        {
            IsBuild = true;
            UseOptimizer = false;
            Error = null;
            finalNEF = nefFile.Script;
            JObject manifestAbi = JObject.Parse(manifestFile);
            var abi = manifestAbi["abi"] as JObject;
            finalABI = abi;
            finalManifest = manifestFile;
        }
    }
}
