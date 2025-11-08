// Copyright (C) 2015-2025 The Neo Project.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Neo.Optimizer
{
    public static class Optimizer
    {
        public static readonly int[] OperandSizePrefixTable = new int[256];
        public static readonly int[] OperandSizeTable = new int[256];
        public static readonly Dictionary<string, Func<NefFile, ContractManifest, JObject, (NefFile nef, ContractManifest manifest, JObject debugInfo)>> strategies = new();
        private static readonly List<(MethodInfo method, StrategyAttribute attribute)> orderedStrategies = new();

        static Optimizer()
        {
            var assembly = Assembly.GetExecutingAssembly();
            foreach (Type type in assembly.GetTypes())
                RegisterStrategies(type);
            DiscoverAndOrderStrategies(assembly);
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
                StrategyAttribute? attribute = method.GetCustomAttribute<StrategyAttribute>();
                if (attribute is null) continue;

                // Validate method signature
                if (method.ReturnType != typeof((NefFile, ContractManifest, JObject?)) ||
                    method.GetParameters().Length != 3 ||
                    method.GetParameters()[0].ParameterType != typeof(NefFile) ||
                    method.GetParameters()[1].ParameterType != typeof(ContractManifest) ||
                    method.GetParameters()[2].ParameterType != typeof(JObject))
                {
                    continue; // Skip methods with incorrect signature
                }

                string name = string.IsNullOrEmpty(attribute.Name) ? method.Name.ToLowerInvariant() : attribute.Name;
                strategies[name] = method.CreateDelegate<Func<NefFile, ContractManifest, JObject, (NefFile nef, ContractManifest manifest, JObject debugInfo)>>();
                orderedStrategies.Add((method, attribute));
            }

            // Sort strategies by priority (highest priority first)
            orderedStrategies.Sort((a, b) => b.attribute.Priority.CompareTo(a.attribute.Priority));
        }

        private static void DiscoverAndOrderStrategies(Assembly assembly)
        {
            var strategyMethods = new List<(MethodInfo method, StrategyAttribute attribute)>();

            foreach (Type type in assembly.GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    var attribute = method.GetCustomAttribute<StrategyAttribute>();
                    if (attribute != null)
                    {
                        // Verify method signature matches expected optimization strategy signature
                        var parameters = method.GetParameters();
                        if (parameters.Length == 3 &&
                            parameters[0].ParameterType == typeof(NefFile) &&
                            parameters[1].ParameterType == typeof(ContractManifest) &&
                            parameters[2].ParameterType == typeof(JObject))
                        {
                            strategyMethods.Add((method, attribute));
                        }
                    }
                }
            }

            // Order by priority (higher priority first)
            orderedStrategies.AddRange(strategyMethods.OrderByDescending(s => s.attribute.Priority));
        }

        public static (NefFile, ContractManifest, JObject?) Optimize(NefFile nef, ContractManifest manifest, JObject? debugInfo = null, CompilationOptions.OptimizationType optimizationType = CompilationOptions.OptimizationType.All)
        {
            if (!optimizationType.HasFlag(CompilationOptions.OptimizationType.Experimental))
                return (nef, manifest, debugInfo);  // do nothing
            // Define the optimization type inside the manifest
            manifest.Extra ??= new JObject();
            manifest.Extra["nef"] = new JObject();
            manifest.Extra["nef"]!["optimization"] = optimizationType.ToString();
            // Execute optimization strategies in priority order (attribute-driven)
            foreach (var (method, attribute) in orderedStrategies)
            {
                try
                {
                    var result = method.Invoke(null, new object?[] { nef, manifest, debugInfo });
                    if (result is ValueTuple<NefFile, ContractManifest, JObject?> tuple)
                    {
                        (nef, manifest, debugInfo) = tuple;
                    }
                }
                catch (Exception ex)
                {
                    // Log warning but continue with other optimizations
                    System.Diagnostics.Debug.WriteLine($"Warning: Optimization strategy '{method.Name}' failed: {ex.Message}");
                }
            }
            return (nef, manifest, debugInfo);
        }
    }
}
