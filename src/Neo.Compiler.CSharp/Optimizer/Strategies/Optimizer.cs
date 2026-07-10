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
        private static readonly Dictionary<(Guid moduleVersionId, int metadataToken), MethodInfo> registeredStrategyMethods = new();

        static Optimizer()
        {
            var assembly = Assembly.GetExecutingAssembly();
            RegisterStrategies(assembly.GetTypes());
            foreach (FieldInfo field in typeof(OpCode).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                OperandSizeAttribute? attribute = field.GetCustomAttribute<OperandSizeAttribute>();
                if (attribute == null) continue;
                int index = (int)(OpCode)field.GetValue(null)!;
                OperandSizePrefixTable[index] = attribute.SizePrefix;
                OperandSizeTable[index] = attribute.Size;
            }
        }

        public static void RegisterStrategies(Type type) => RegisterStrategies([type]);

        private static void RegisterStrategies(IEnumerable<Type> types)
        {
            bool registeredAny = false;
            foreach (Type type in types.OrderBy(type => type.FullName ?? type.Name, StringComparer.Ordinal))
            {
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .OrderBy(method => method.Name, StringComparer.Ordinal)
                    .ThenBy(method => method.MetadataToken))
                {
                    StrategyAttribute? attribute = method.GetCustomAttribute<StrategyAttribute>();
                    if (attribute is null || !HasValidStrategySignature(method))
                        continue;

                    if (!RegisterStrategyMethod(method, attribute))
                        continue;

                    string name = string.IsNullOrEmpty(attribute.Name) ? method.Name.ToLowerInvariant() : attribute.Name;
                    strategies[name] = method.CreateDelegate<Func<NefFile, ContractManifest, JObject, (NefFile nef, ContractManifest manifest, JObject debugInfo)>>();
                    registeredAny = true;
                }
            }

            if (registeredAny)
                orderedStrategies.Sort(CompareStrategies);
        }

        private static bool HasValidStrategySignature(MethodInfo method)
        {
            ParameterInfo[] parameters = method.GetParameters();
            return method.ReturnType == typeof((NefFile, ContractManifest, JObject?)) &&
                parameters.Length == 3 &&
                parameters[0].ParameterType == typeof(NefFile) &&
                parameters[1].ParameterType == typeof(ContractManifest) &&
                parameters[2].ParameterType == typeof(JObject);
        }

        private static bool RegisterStrategyMethod(MethodInfo method, StrategyAttribute attribute)
        {
            var methodId = GetStrategyMethodId(method);
            if (!registeredStrategyMethods.TryAdd(methodId, method))
                return false;

            orderedStrategies.Add((method, attribute));
            return true;
        }

        private static (Guid moduleVersionId, int metadataToken) GetStrategyMethodId(MethodInfo method) =>
            (method.Module.ModuleVersionId, method.MetadataToken);

        private static int CompareStrategies(
            (MethodInfo method, StrategyAttribute attribute) left,
            (MethodInfo method, StrategyAttribute attribute) right)
        {
            int comparison = right.attribute.Priority.CompareTo(left.attribute.Priority);
            if (comparison != 0) return comparison;

            comparison = StringComparer.Ordinal.Compare(left.method.DeclaringType?.Assembly.FullName, right.method.DeclaringType?.Assembly.FullName);
            if (comparison != 0) return comparison;

            comparison = StringComparer.Ordinal.Compare(left.method.DeclaringType?.FullName, right.method.DeclaringType?.FullName);
            if (comparison != 0) return comparison;

            comparison = StringComparer.Ordinal.Compare(left.method.Name, right.method.Name);
            return comparison != 0 ? comparison : left.method.MetadataToken.CompareTo(right.method.MetadataToken);
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
                    var failure = ex is TargetInvocationException { InnerException: not null }
                        ? ex.InnerException
                        : ex;
                    throw new InvalidOperationException($"Optimization strategy '{method.Name}' failed: {failure.Message}", failure);
                }
            }
            // Late strategies can introduce jump patterns after the high-priority jump cleanup has run.
            (nef, manifest, debugInfo) = JumpCompresser.RemoveUnnecessaryJumps(nef, manifest, debugInfo);
            (nef, manifest, debugInfo) = JumpCompresser.ReplaceJumpWithRet(nef, manifest, debugInfo);
            (nef, manifest, debugInfo) = JumpCompresser.FoldJump(nef, manifest, debugInfo);
            (nef, manifest, debugInfo) = JumpCompresser.CompressJump(nef, manifest, debugInfo);
            return (nef, manifest, debugInfo);
        }
    }
}
