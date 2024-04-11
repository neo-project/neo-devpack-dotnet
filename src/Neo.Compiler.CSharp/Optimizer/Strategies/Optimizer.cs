// Copyright (C) 2015-2024 The Neo Project.
//
// Optimizer.cs file belongs to the neo project and is free
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
using Neo.VM;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Neo.Optimizer
{
    class Optimizer
    {
        public static readonly int[] OperandSizePrefixTable = new int[256];
        public static readonly int[] OperandSizeTable = new int[256];
        public static readonly Dictionary<string, Func<NefFile, ContractManifest, JObject, (NefFile nef, ContractManifest manifest, JObject debugInfo)>> strategies = new();

        static Optimizer()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
                RegisterStrategies(type);
            foreach (FieldInfo field in typeof(OpCode).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                OperandSizeAttribute? attribute = field.GetCustomAttribute<OperandSizeAttribute>();
                if (attribute == null) continue;
                int index = (int)(OpCode)field.GetValue(null)!;
                OperandSizePrefixTable[index] = attribute.SizePrefix;
                OperandSizeTable[index] = attribute.Size;
            }
        }

        public static void RegisterStrategies(Type type)
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                StrategyAttribute attribute = method.GetCustomAttribute<StrategyAttribute>()!;
                if (attribute is null) continue;
                string name = string.IsNullOrEmpty(attribute.Name) ? method.Name.ToLowerInvariant() : attribute.Name;
                strategies[name] = method.CreateDelegate<Func<NefFile, ContractManifest, JObject, (NefFile nef, ContractManifest manifest, JObject debugInfo)>>();
            }
        }
    }
}
