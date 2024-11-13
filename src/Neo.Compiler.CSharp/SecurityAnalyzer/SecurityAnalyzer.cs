using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;

namespace Neo.Compiler.SecurityAnalyzer
{
    public static class SecurityAnalyzer
    {
        public static void AnalyzeWithPrint(NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            ReEntrancyAnalyzer.AnalyzeSingleContractReEntrancy(nef, manifest, debugInfo).GetWarningInfo(print: true);
        }
    }
}
