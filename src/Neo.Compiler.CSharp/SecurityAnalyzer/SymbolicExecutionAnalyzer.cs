// Copyright (C) 2015-2026 The Neo Project.
//
// SymbolicExecutionAnalyzer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Compiler.SecurityAnalyzer.SymbolicExecution;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Coverage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo.Compiler.SecurityAnalyzer
{
    public static class SymbolicExecutionAnalyzer
    {
        public sealed class SymbolicExecutionWarnings
        {
            private readonly IReadOnlyList<SymbolicWarning> warnings;
            private readonly JToken? debugInfo;

            public bool HasUnguardedUpdate { get; }
            public bool HasUnguardedDestroy { get; }
            public bool HasVerifyStorageWrite { get; }
            public bool AnalysisIncomplete { get; }

            internal SymbolicExecutionWarnings(SymbolicFindings findings, JToken? debugInfo)
            {
                warnings = findings.Warnings;
                this.debugInfo = debugInfo;
                HasUnguardedUpdate = findings.HasUnguardedUpdate;
                HasUnguardedDestroy = findings.HasUnguardedDestroy;
                HasVerifyStorageWrite = findings.HasVerifyStorageWrite;
                AnalysisIncomplete = findings.AnalysisIncomplete;
            }

            public string GetWarningInfo(bool print = false)
            {
                if (warnings.Count == 0)
                    return string.Empty;

                NeoDebugInfo? neoDebugInfo = null;
                if (debugInfo is JObject jObj)
                {
                    try
                    {
                        neoDebugInfo = NeoDebugInfo.FromDebugInfoJson(jObj);
                    }
                    catch
                    {
                        neoDebugInfo = null;
                    }
                }

                StringBuilder result = new();
                foreach (SymbolicWarning warning in warnings)
                {
                    switch (warning.Kind)
                    {
                        case SymbolicWarningKind.UnguardedUpdate:
                            result.AppendLine($"[SEC] Symbolic execution detected unguarded contract update in method '{warning.EntryPoint}'.");
                            result.AppendLine("  Recommendation: Guard update with Assert(CheckWitness(owner)) or an equivalent permission check.");
                            break;
                        case SymbolicWarningKind.UnguardedDestroy:
                            result.AppendLine($"[SEC] Symbolic execution detected unguarded contract destroy in method '{warning.EntryPoint}'.");
                            result.AppendLine("  Recommendation: Guard destroy with Assert(CheckWitness(owner)) or an equivalent permission check.");
                            break;
                        case SymbolicWarningKind.VerifyStorageWrite:
                            result.AppendLine($"[SEC] Symbolic execution detected storage writes reachable from Verify in method '{warning.EntryPoint}'.");
                            result.AppendLine("  Recommendation: Avoid storage writes in Verify or move them to runtime methods.");
                            break;
                        case SymbolicWarningKind.AnalysisIncomplete:
                            result.AppendLine("[SEC][INFO] Symbolic execution analysis was incomplete due to path/step limits.");
                            break;
                        default:
                            break;
                    }

                    if (warning.Address >= 0)
                    {
                        if (neoDebugInfo != null)
                        {
                            var location = neoDebugInfo.GetSourceLocation(warning.Address);
                            if (location != null)
                            {
                                result.AppendLine($"  At: {location.FileName}:{location.Line}:{location.Column}");
                            }
                            else
                            {
                                result.AppendLine($"  At instruction address: {warning.Address}");
                            }
                        }
                        else
                        {
                            result.AppendLine($"  At instruction address: {warning.Address}");
                        }
                    }

                    result.AppendLine();
                }

                string output = result.ToString();
                if (print)
                    Console.Write(output);
                return output;
            }
        }

        public static SymbolicExecutionWarnings Analyze(NefFile nef, ContractManifest manifest, JToken? debugInfo = null)
        {
            SymbolicExecutor executor = new();
            SymbolicFindings findings = executor.Analyze(nef, manifest, debugInfo);
            return new SymbolicExecutionWarnings(findings, debugInfo);
        }
    }
}
