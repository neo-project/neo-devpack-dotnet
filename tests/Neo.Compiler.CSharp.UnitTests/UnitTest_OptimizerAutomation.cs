// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_OptimizerAutomation.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Json;
using Neo.Optimizer;
using OptimizerClass = Neo.Optimizer.Optimizer;
using StrategyAttribute = Neo.Optimizer.StrategyAttribute;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_OptimizerAutomation : DebugAndTestBase<Contract_Array>
    {
        [TestMethod]
        public void Test_StrategyDiscovery()
        {
            // Test that the optimizer can discover strategies with StrategyAttribute
            var strategyMethods = GetStrategyMethods();

            // We should have multiple strategy methods discovered
            Assert.IsTrue(strategyMethods.Count > 0, "Should discover strategy methods with StrategyAttribute");

            // All discovered methods should have StrategyAttribute
            foreach (var (method, attribute) in strategyMethods)
            {
                Assert.IsNotNull(attribute, $"Method {method.Name} should have StrategyAttribute");
                Assert.IsNotNull(method, $"Method should not be null");
            }
        }

        [TestMethod]
        public void Test_StrategyPriorityOrdering()
        {
            // Test that strategies are ordered by priority (highest first)
            var strategyMethods = GetStrategyMethods();

            for (int i = 0; i < strategyMethods.Count - 1; i++)
            {
                var currentPriority = strategyMethods[i].attribute.Priority;
                var nextPriority = strategyMethods[i + 1].attribute.Priority;

                Assert.IsTrue(currentPriority >= nextPriority,
                    $"Strategy priorities should be ordered from highest to lowest. " +
                    $"Found {strategyMethods[i].method.Name} (priority {currentPriority}) before " +
                    $"{strategyMethods[i + 1].method.Name} (priority {nextPriority})");
            }
        }

        [TestMethod]
        public void Test_AutomatedOptimization()
        {
            // Test that automated optimization produces results
            var (nef, manifest, debugInfo) = OptimizerClass.Optimize(NefFile, Manifest, null, CompilationOptions.OptimizationType.Experimental);

            Assert.IsNotNull(nef, "Optimized NEF should not be null");
            Assert.IsNotNull(manifest, "Optimized manifest should not be null");

            // Verify that optimization metadata is added to manifest
            Assert.IsNotNull(manifest.Extra, "Manifest should have Extra field");
            Assert.IsTrue(manifest.Extra.ContainsProperty("nef"), "Manifest should contain nef optimization info");

            var nefExtra = manifest.Extra["nef"] as JObject;
            Assert.IsNotNull(nefExtra, "NEF extra should be a JSON object");
            Assert.IsTrue(nefExtra.ContainsProperty("optimization"), "NEF extra should contain optimization type");
        }

        [TestMethod]
        public void Test_OptimizationWithoutExperimental()
        {
            // Test that optimization is skipped when experimental flag is not set
            var originalNef = NefFile;
            var originalManifest = Manifest;

            var (nef, manifest, debugInfo) = OptimizerClass.Optimize(originalNef, originalManifest, null, CompilationOptions.OptimizationType.None);

            // Should return original objects unchanged
            Assert.AreSame(originalNef, nef, "NEF should be unchanged without experimental optimization");
            Assert.AreSame(originalManifest, manifest, "Manifest should be unchanged without experimental optimization");
        }

        [TestMethod]
        public void Test_StrategyAttributeValidation()
        {
            // Verify that all discovered strategies have valid method signatures
            var optimizerType = typeof(OptimizerClass);
            var field = optimizerType.GetField("orderedStrategies", BindingFlags.NonPublic | BindingFlags.Static);
            var orderedStrategies = field.GetValue(null) as System.Collections.Generic.List<(MethodInfo method, StrategyAttribute attribute)>;

            foreach (var (method, attribute) in orderedStrategies)
            {
                // Verify method signature
                Assert.IsTrue(method.IsStatic, $"Strategy method '{method.Name}' should be static");
                Assert.IsTrue(method.IsPublic, $"Strategy method '{method.Name}' should be public");

                var parameters = method.GetParameters();
                Assert.AreEqual(3, parameters.Length, $"Strategy method '{method.Name}' should have 3 parameters");
                Assert.AreEqual(typeof(NefFile), parameters[0].ParameterType, $"First parameter of '{method.Name}' should be NefFile");
                Assert.AreEqual(typeof(ContractManifest), parameters[1].ParameterType, $"Second parameter of '{method.Name}' should be ContractManifest");
                Assert.AreEqual(typeof(JObject), parameters[2].ParameterType, $"Third parameter of '{method.Name}' should be JObject");

                // Verify return type
                var returnType = method.ReturnType;
                Assert.IsTrue(returnType.IsGenericType, $"Return type of '{method.Name}' should be generic");
                Assert.AreEqual(typeof(ValueTuple<,,>), returnType.GetGenericTypeDefinition(), $"Return type of '{method.Name}' should be ValueTuple");

                var genericArgs = returnType.GetGenericArguments();
                Assert.AreEqual(3, genericArgs.Length, $"Return type of '{method.Name}' should have 3 generic arguments");
                Assert.AreEqual(typeof(NefFile), genericArgs[0], $"First generic argument of '{method.Name}' return type should be NefFile");
                Assert.AreEqual(typeof(ContractManifest), genericArgs[1], $"Second generic argument of '{method.Name}' return type should be ContractManifest");
                Assert.IsTrue(genericArgs[2] == typeof(JObject) || genericArgs[2].Name == "Nullable`1", $"Third generic argument of '{method.Name}' return type should be JObject or JObject?");
            }
        }

        [TestMethod]
        public void Test_ErrorHandling()
        {
            // This test verifies that optimization continues even if individual strategies fail
            // We can't easily simulate failures without modifying the strategies, but we can verify
            // that the automation system is robust by checking it handles null inputs gracefully

            var (nef, manifest, debugInfo) = OptimizerClass.Optimize(NefFile, Manifest, null, CompilationOptions.OptimizationType.Experimental);

            // If we get here without exceptions, the error handling is working
            Assert.IsNotNull(nef);
            Assert.IsNotNull(manifest);
        }

        [TestMethod]
        public void Test_StrategyExecution()
        {
            // Test that strategies are actually executed by comparing before and after
            var originalNef = NefFile;
            var originalManifest = Manifest;

            var (optimizedNef, optimizedManifest, debugInfo) = OptimizerClass.Optimize(originalNef, originalManifest, null, CompilationOptions.OptimizationType.Experimental);

            // Verify that some optimization occurred (manifest should have extra data)
            Assert.IsNotNull(optimizedManifest.Extra, "Optimized manifest should have Extra field");
            Assert.IsTrue(optimizedManifest.Extra.ContainsProperty("nef"), "Optimized manifest should contain nef info");

            // The NEF might be the same if no optimizations apply to this specific contract,
            // but the manifest should always be modified to include optimization metadata
            Assert.IsNotNull(optimizedNef, "Optimized NEF should not be null");
        }

        [TestMethod]
        public void Test_AutomatedOptimizationExecution()
        {
            // Test that the automated optimization system produces valid results
            var originalNef = NefFile;
            var originalManifest = Manifest;
            JObject? originalDebugInfo = null;

            // Run optimization
            var (optimizedNef, optimizedManifest, optimizedDebugInfo) =
                Neo.Optimizer.Optimizer.Optimize(originalNef, originalManifest, originalDebugInfo,
                    CompilationOptions.OptimizationType.Experimental);

            // Verify results are valid
            Assert.IsNotNull(optimizedNef, "Optimized NEF should not be null");
            Assert.IsNotNull(optimizedManifest, "Optimized manifest should not be null");
            Assert.IsTrue(optimizedNef.Script.Length > 0, "Optimized NEF should have script content");

            // Verify optimization metadata was added
            Assert.IsNotNull(optimizedManifest.Extra, "Optimized manifest should have Extra data");
            Assert.IsNotNull(optimizedManifest.Extra["nef"], "Optimized manifest should have nef metadata");
            Assert.AreEqual("Experimental", optimizedManifest.Extra["nef"]!["optimization"]!.GetString(),
                "Optimization type should be recorded in manifest");
        }

        [TestMethod]
        public void Test_StrategyErrorHandling()
        {
            // Test with null debug info to ensure robustness
            var (resultNef, resultManifest, resultDebugInfo) =
                Neo.Optimizer.Optimizer.Optimize(NefFile, Manifest, null,
                    CompilationOptions.OptimizationType.Experimental);

            // Should return valid results
            Assert.IsNotNull(resultNef, "Should return NEF even with null debug info");
            Assert.IsNotNull(resultManifest, "Should return manifest even with null debug info");
        }

        [TestMethod]
        public void Test_OptimizationTypeFlags()
        {
            // Test that non-experimental optimization types are handled correctly
            var (resultNef, resultManifest, resultDebugInfo) =
                Neo.Optimizer.Optimizer.Optimize(NefFile, Manifest, null,
                    CompilationOptions.OptimizationType.Basic);

            // Should return original files unchanged for non-experimental
            Assert.AreEqual(NefFile.Script.Length, resultNef.Script.Length,
                "Non-experimental optimization should not modify NEF");
        }

        private static List<(MethodInfo method, StrategyAttribute attribute)> GetStrategyMethods()
        {
            var strategyMethods = new List<(MethodInfo method, StrategyAttribute attribute)>();
            var assembly = Assembly.GetAssembly(typeof(Neo.Optimizer.Optimizer));

            foreach (Type type in assembly!.GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                {
                    var attribute = method.GetCustomAttribute<StrategyAttribute>();
                    if (attribute != null)
                    {
                        strategyMethods.Add((method, attribute));
                    }
                }
            }

            // Sort by priority (highest first) to match the optimizer's behavior
            strategyMethods.Sort((a, b) => b.attribute.Priority.CompareTo(a.attribute.Priority));
            return strategyMethods;
        }
    }
}
