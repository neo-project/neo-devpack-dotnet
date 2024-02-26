using Neo.SmartContract.Framework.Interfaces;
using Neo.SmartContract.Framework.Native;
using Neo.SmartContract.Framework.Services;

namespace Neo.SmartContract.Framework.UnitTests.TestClasses
{
    public class Contract_IOracle : SmartContract, IOracle
    {
        private const string PreData = "PreData";

        public void OnOracleResponse(string url, object userData, OracleResponseCode code, string result)
        {
            if (Runtime.CallingScriptHash != Oracle.Hash)
                throw new System.Exception("Unauthorized!");
            Storage.Put(Storage.CurrentContext, PreData, result);
        }
    }
}
