// Copyright (C) 2015-2026 The Neo Project.
//
// Optimizer.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Compiler;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.VM;
using System.Reflection;

namespace Neo.Optimizer
{
    public static class Optimizer
    {
        public static readonly int[] OperandSizePrefixTable = new int[256];
        public static readonly int[] OperandSizeTable = new int[256];

        static Optimizer()
        {
            foreach (FieldInfo field in typeof(OpCode).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                OperandSizeAttribute? attribute = field.GetCustomAttribute<OperandSizeAttribute>();
                if (attribute is null) continue;

                int index = (int)(OpCode)field.GetValue(null)!;
                OperandSizePrefixTable[index] = attribute.SizePrefix;
                OperandSizeTable[index] = attribute.Size;
            }
        }

        public static (NefFile, ContractManifest, JObject?) Optimize(NefFile nef, ContractManifest manifest,
            JObject? debugInfo = null, CompilationOptions.OptimizationType optimizationType = CompilationOptions.OptimizationType.All)
        {
            if (!optimizationType.HasFlag(CompilationOptions.OptimizationType.Experimental))
                return (nef, manifest, debugInfo);

            manifest.Extra ??= new JObject();
            manifest.Extra["nef"] = new JObject();
            manifest.Extra["nef"]!["optimization"] = optimizationType.ToString();
            return Strategies.Run(nef, manifest, debugInfo);
        }
    }
}
