using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.IO.Json;
using Neo.SmartContract;

namespace Neo.Compiler.MSIL.Utils
{
    class BuildNEF : BuildScript
    {
        public BuildNEF(NefFile nefFile, string manifestFile) : base()
        {
            IsBuild = true;
            UseOptimizer = false;
            Error = null;
            finalNEFScript = nefFile.Script;
            finalABI = nefFile.Abi.ToJson();
            finalManifest = manifestFile;
        }
    }
}
