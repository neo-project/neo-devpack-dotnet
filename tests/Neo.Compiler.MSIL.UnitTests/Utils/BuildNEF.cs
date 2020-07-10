using Neo.Compiler.MSIL.UnitTests.Utils;
using Neo.SmartContract;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.MSIL.Utils
{
    class BuildNEF : BuildScript
    {
        public BuildNEF(NefFile nefFile, string manifestFile) : base()
        {
            IsBuild = true;
            UseOptimizer = false;
            Error = null;
            finalNEF = nefFile.Script;
            MyJson.JsonNode_Object manifestAbi = MyJson.Parse(manifestFile) as MyJson.JsonNode_Object;
            var abi = manifestAbi.GetDictItem("abi") as MyJson.JsonNode_Object;
            finialABI = abi;
            finalManifest = manifestFile;
        }
    }
}
