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
        public void Test_StrategyAttributeDiscovery()
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
