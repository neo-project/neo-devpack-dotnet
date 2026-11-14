// Copyright (C) 2015-2026 The Neo Project.
//
// OptimizationPipeline.cs file belongs to the neo project and is free
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

namespace Neo.Optimizer
{
    internal static class Strategies
    {
        public static (NefFile, ContractManifest, JObject?) Run(NefFile nef, ContractManifest manifest, JObject? debugInfo = null)
        {
            nef = Peephole.Rewrite(nef, manifest, PeepholeRule.All);
            ScriptGraph graph = ScriptGraph.Parse(nef, manifest, debugInfo);
            graph.ApplyStaticFieldConstants(graph.BuildCoverage());
            graph.RemoveUnusedInitialize();
            graph.ApplyConstantConditionalJumps();
            graph.NormalizeJumps();
            graph.CollapseAdjacentReturns();
            graph.RemoveUnreachable();
            graph.NormalizeJumps();
            graph.CollapseAdjacentReturns();
            graph.RemoveUnusedMethodTokens();
            return graph.Emit();
        }

        internal static (NefFile, ContractManifest, JObject?) ApplyPeephole(NefFile nef, ContractManifest manifest, JObject? debugInfo, PeepholeRule rules)
        {
            nef = Peephole.Rewrite(nef, manifest, rules);
            ScriptGraph graph = ScriptGraph.Parse(nef, manifest, debugInfo);
            graph.RemoveUnreachable();
            return graph.Emit();
        }

        internal static (NefFile, ContractManifest, JObject?) NormalizeJumps(NefFile nef, ContractManifest manifest, JObject? debugInfo)
        {
            ScriptGraph graph = ScriptGraph.Parse(nef, manifest, debugInfo);
            graph.NormalizeJumps();
            return graph.Emit();
        }

        internal static (NefFile, ContractManifest, JObject?) ReplaceJumpWithRet(NefFile nef, ContractManifest manifest, JObject? debugInfo)
        {
            ScriptGraph graph = ScriptGraph.Parse(nef, manifest, debugInfo);
            graph.ReplaceJumpToReturn();
            return graph.Emit();
        }

        internal static (NefFile, ContractManifest, JObject?) RemoveUnnecessaryJumps(NefFile nef, ContractManifest manifest, JObject? debugInfo)
        {
            ScriptGraph graph = ScriptGraph.Parse(nef, manifest, debugInfo);
            graph.RemoveJumpsToNextInstruction();
            return graph.Emit();
        }

        internal static (NefFile, ContractManifest, JObject?) CollapseAdjacentReturns(NefFile nef, ContractManifest manifest, JObject? debugInfo)
        {
            ScriptGraph graph = ScriptGraph.Parse(nef, manifest, debugInfo);
            graph.CollapseAdjacentReturns();
            return graph.Emit();
        }

        internal static (NefFile, ContractManifest, JObject?) RemoveUnreachable(NefFile nef, ContractManifest manifest, JObject? debugInfo)
        {
            ScriptGraph graph = ScriptGraph.Parse(nef, manifest, debugInfo);
            graph.RemoveUnreachable();
            return graph.Emit();
        }

        internal static (NefFile, ContractManifest, JObject?) RemoveMethodTokens(NefFile nef, ContractManifest manifest, JObject? debugInfo)
        {
            ScriptGraph graph = ScriptGraph.Parse(nef, manifest, debugInfo);
            graph.RemoveUnusedMethodTokens();
            return graph.Emit();
        }
    }
}
