using Neo.IO.Json;
using Neo.SmartContract;
using Neo.SmartContract.Native;
using Neo.VM;

namespace Neo.TestingEngine
{
    class BuildNative : BuildScript
    {
        public readonly NativeContract nativeContract;
        public BuildNative(NativeContract nativeContract) : base()
        {
            this.nativeContract = nativeContract;
            this.nefFile = nativeContract.Nef;

            byte[] script;
            using (ScriptBuilder sb = new ScriptBuilder())
            {
                sb.EmitPush(nativeContract.Hash);
                sb.EmitSysCall(ApplicationEngine.System_Contract_Call);
                script = sb.ToArray();
            }

            IsBuild = true;
            UseOptimizer = false;
            Error = null;
            JObject manifestJson = nativeContract.Manifest.ToJson();
            var abi = manifestJson["abi"] as JObject;
            finalABI = abi;
            finalNEFScript = script;
        }
    }
}
