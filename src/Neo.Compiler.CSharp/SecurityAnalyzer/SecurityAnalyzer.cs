// Copyright (C) 2015-2026 The Neo Project.
//
// SecurityAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

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
            WriteInTryAnalyzer.AnalyzeWriteInTry(nef, manifest, debugInfo).GetWarningInfo(print: true);
            CheckWitnessAnalyzer.AnalyzeCheckWitness(nef, manifest, debugInfo).GetWarningInfo(print: true);
            MissingCheckWitnessAnalyzer.AnalyzeMissingCheckWitness(nef, manifest, debugInfo).GetWarningInfo(print: true);
            UnboundedOperationAnalyzer.AnalyzeUnboundedOperations(nef, manifest, debugInfo).GetWarningInfo(print: true);
            bool canUpdate = UpdateAnalyzer.AnalyzeUpdate(nef, manifest, debugInfo);
            bool canDestroy = UpdateAnalyzer.AnalyzeDestroy(nef, manifest, debugInfo);
            if (canUpdate)
                Console.WriteLine("[SECURITY] This contract can be updated.");
            if (canDestroy)
                Console.WriteLine("[SECURITY] This contract can be destroyed.");
            if (!canUpdate && !canDestroy)
                Console.WriteLine("[SECURITY] This contract cannot be updated or destroyed, or maybe you used abstract code styles to do so.");
        }
    }
}
