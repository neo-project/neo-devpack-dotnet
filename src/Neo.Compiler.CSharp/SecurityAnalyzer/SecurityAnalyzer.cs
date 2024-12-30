using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;

namespace Neo.Compiler.SecurityAnalyzer
{
    public static class SecurityAnalyzer
    {
        public static void AnalyzeWithPrint(NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            ReEntrancyAnalyzer.AnalyzeSingleContractReEntrancy(nef, manifest, debugInfo).GetWarningInfo(print: true);
            WriteInTryAnalzyer.AnalyzeWriteInTry(nef, manifest, debugInfo).GetWarningInfo(print: true);
            CheckWitnessAnalyzer.AnalyzeCheckWitness(nef, manifest, debugInfo).GetWarningInfo(print: true);
            if (!UpdateAnalzyer.AnalyzeUpdate(nef, manifest, debugInfo))
                Console.WriteLine("[SEC] This contract cannot be updated, or maybe you used abstract code styles to update it.");
        }
    }
}
