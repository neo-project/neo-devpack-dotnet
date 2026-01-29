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
            if (!UpdateAnalyzer.AnalyzeUpdate(nef, manifest, debugInfo))
                Console.WriteLine("[SEC] This contract cannot be updated, or maybe you used abstract code styles to update it.");
        }
    }
}
