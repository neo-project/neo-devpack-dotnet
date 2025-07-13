// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_WebGuiGeneration.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Compiler.WebGui;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_WebGuiGeneration
    {
        private readonly string testContractsPath = Utils.Extensions.TestContractRoot;
        private readonly string tempTestDir = Path.Combine(Path.GetTempPath(), "neo-webgui-tests", Guid.NewGuid().ToString());

        [TestInitialize]
        public void Setup()
        {
            Directory.CreateDirectory(tempTestDir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(tempTestDir))
            {
                Directory.Delete(tempTestDir, true);
            }
        }

        [TestMethod]
        public void Test_WebGuiGenerator_DefaultConstructor()
        {
            var generator = new WebGuiGenerator();
            Assert.IsNotNull(generator);
        }

        [TestMethod]
        public void Test_WebGuiOptions_DefaultValues()
        {
            var options = new WebGuiOptions();

            Assert.AreEqual("https://neo.coz.io:443", options.RpcEndpoint);
            Assert.AreEqual((uint)860833102, options.NetworkMagic);
            Assert.IsTrue(options.IncludeTransactionHistory);
            Assert.IsTrue(options.IncludeBalanceMonitoring);
            Assert.IsTrue(options.IncludeMethodInvocation);
            Assert.IsTrue(options.IncludeStateMonitoring);
            Assert.IsTrue(options.IncludeEventMonitoring);
            Assert.IsTrue(options.IncludeWalletConnection);
            Assert.AreEqual(30, options.RefreshInterval);
            Assert.IsFalse(options.DarkTheme);
            Assert.IsFalse(options.ShowAdvancedFeatures);
        }

        [TestMethod]
        public void Test_CompilationContext_GenerateWebGui_Success()
        {
            var engine = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended
            });

            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            Assert.AreEqual(1, results.Count);
            var context = results[0];
            Assert.IsTrue(context.Success);

            var webGuiResult = context.GenerateWebGui(tempTestDir);

            Assert.IsNotNull(webGuiResult);
            Assert.IsTrue(webGuiResult.Success);
            Assert.IsNull(webGuiResult.ErrorMessage);
            Assert.IsNotNull(webGuiResult.HtmlFilePath);
            Assert.IsNotNull(webGuiResult.OutputDirectory);
            Assert.IsTrue(webGuiResult.GeneratedFiles.Count > 0);
            Assert.IsTrue(File.Exists(webGuiResult.HtmlFilePath));
        }

        [TestMethod]
        public void Test_CompilationContext_GenerateWebGui_WithCustomOptions()
        {
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            var context = results[0];
            Assert.IsTrue(context.Success);

            var options = new WebGuiOptions
            {
                DarkTheme = true,
                RefreshInterval = 60,
                IncludeTransactionHistory = false,
                IncludeEventMonitoring = false,
                CustomCss = "body { background: red; }",
                CustomJavaScript = "console.log('Custom JS');"
            };

            var webGuiResult = context.GenerateWebGui(tempTestDir, options);

            Assert.IsTrue(webGuiResult.Success);
            Assert.IsNotNull(webGuiResult.HtmlContent);
            Assert.IsTrue(webGuiResult.HtmlContent.Contains("dark-theme"));
        }

        [TestMethod]
        public void Test_CompilationContext_GenerateWebGuiHtml_Success()
        {
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            var context = results[0];
            Assert.IsTrue(context.Success);

            var htmlContent = context.GenerateWebGuiHtml();

            Assert.IsNotNull(htmlContent);
            Assert.IsTrue(htmlContent.Length > 0);
            Assert.IsTrue(htmlContent.Contains("<!DOCTYPE html>"));
            Assert.IsTrue(htmlContent.Contains("Neo Smart Contract Dashboard"));
            Assert.IsTrue(htmlContent.Contains("Contract Information"));
        }

        [TestMethod]
        public void Test_CompilationContext_GenerateWebGui_FailedCompilation()
        {
            var engine = new CompilationEngine();

            // Create an invalid contract
            var invalidContract = @"
using Neo.SmartContract.Framework;

namespace TestContract
{
    public class InvalidContract
    {
        public static void Test()
        {
            invalid syntax here - this will not compile at all
        }
    }
}";

            var contractPath = Path.Combine(tempTestDir, "InvalidContract.cs");
            File.WriteAllText(contractPath, invalidContract);

            // This should throw FormatException due to invalid syntax
            Assert.ThrowsException<FormatException>(() => engine.CompileSources(new[] { contractPath }));
        }

        [TestMethod]
        public void Test_CompilationEngine_GenerateWebGui_MultipleContracts()
        {
            var engine = new CompilationEngine();
            var contractPaths = new[]
            {
                Path.Combine(testContractsPath, "Contract_BigInteger.cs"),
                Path.Combine(testContractsPath, "Contract_Math.cs")
            };
            var results = engine.CompileSources(contractPaths);

            Assert.IsTrue(results.Count >= 2);
            Assert.IsTrue(results.All(r => r.Success));

            var webGuiResults = engine.GenerateWebGui(results, tempTestDir);

            Assert.IsNotNull(webGuiResults);
            Assert.IsTrue(webGuiResults.Count >= 2);
            Assert.IsTrue(webGuiResults.All(r => r.Success));

            foreach (var result in webGuiResults)
            {
                Assert.IsNotNull(result.HtmlFilePath);
                Assert.IsTrue(File.Exists(result.HtmlFilePath));
            }
        }

        [TestMethod]
        public void Test_CompilationEngine_CompileSourcesWithWebGui()
        {
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");

            var (contexts, webGuiResults) = engine.CompileSourcesWithWebGui(
                new[] { contractPath },
                tempTestDir,
                new WebGuiOptions { DarkTheme = true });

            Assert.IsNotNull(contexts);
            Assert.IsNotNull(webGuiResults);
            Assert.AreEqual(contexts.Count, webGuiResults.Count);
            Assert.IsTrue(contexts.All(c => c.Success));
            Assert.IsTrue(webGuiResults.All(r => r.Success));

            foreach (var result in webGuiResults)
            {
                Assert.IsTrue(result.HtmlContent?.Contains("dark-theme") == true);
            }
        }

        [TestMethod]
        public void Test_WebGuiGeneration_ContainsExpectedFiles()
        {
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            var context = results[0];
            var webGuiResult = context.GenerateWebGui(tempTestDir);

            Assert.IsTrue(webGuiResult.Success);
            Assert.IsTrue(webGuiResult.GeneratedFiles.Count >= 4);

            // Check for expected files
            var expectedFiles = new[] { "index.html", "styles.css", "contract.js", "config.json" };
            foreach (var expectedFile in expectedFiles)
            {
                var filePath = Path.Combine(webGuiResult.OutputDirectory!, expectedFile);
                Assert.IsTrue(File.Exists(filePath), $"Expected file {expectedFile} not found");
                Assert.IsTrue(webGuiResult.GeneratedFiles.Contains(filePath));
            }
        }

        [TestMethod]
        public void Test_WebGuiGeneration_HtmlContent_Structure()
        {
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            var context = results[0];
            var htmlContent = context.GenerateWebGuiHtml();

            // Check HTML structure
            Assert.IsTrue(htmlContent.Contains("<!DOCTYPE html>"));
            Assert.IsTrue(htmlContent.Contains("<html lang=\"en\">"));
            Assert.IsTrue(htmlContent.Contains("<head>"));
            Assert.IsTrue(htmlContent.Contains("<body>"));
            Assert.IsTrue(htmlContent.Contains("</html>"));

            // Check for key components
            Assert.IsTrue(htmlContent.Contains("class=\"header\""));
            Assert.IsTrue(htmlContent.Contains("class=\"nav-tabs\""));
            Assert.IsTrue(htmlContent.Contains("class=\"container\""));
            Assert.IsTrue(htmlContent.Contains("Contract Information"));
            Assert.IsTrue(htmlContent.Contains("Neo Smart Contract Dashboard"));
        }

        [TestMethod]
        public void Test_WebGuiGeneration_JavaScriptInclusion()
        {
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            var context = results[0];
            var webGuiResult = context.GenerateWebGui(tempTestDir);

            var jsFilePath = Path.Combine(webGuiResult.OutputDirectory!, "contract.js");
            Assert.IsTrue(File.Exists(jsFilePath));

            var jsContent = File.ReadAllText(jsFilePath);
            Assert.IsTrue(jsContent.Contains("CONFIG"));
            Assert.IsTrue(jsContent.Contains("APP_STATE"));
            Assert.IsTrue(jsContent.Contains("tabManager"));
            Assert.IsTrue(jsContent.Contains("rpc"));
        }

        [TestMethod]
        public void Test_WebGuiGeneration_CssInclusion()
        {
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            var context = results[0];
            var webGuiResult = context.GenerateWebGui(tempTestDir);

            var cssFilePath = Path.Combine(webGuiResult.OutputDirectory!, "styles.css");
            Assert.IsTrue(File.Exists(cssFilePath));

            var cssContent = File.ReadAllText(cssFilePath);
            Assert.IsTrue(cssContent.Contains("/* Base Styles */"));
            Assert.IsTrue(cssContent.Contains(".header"));
            Assert.IsTrue(cssContent.Contains(".nav-tabs"));
            Assert.IsTrue(cssContent.Contains(".card"));
        }

        [TestMethod]
        public void Test_WebGuiGeneration_ConfigFile()
        {
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            var context = results[0];
            var webGuiResult = context.GenerateWebGui(tempTestDir);

            var configFilePath = Path.Combine(webGuiResult.OutputDirectory!, "config.json");
            Assert.IsTrue(File.Exists(configFilePath));

            var configContent = File.ReadAllText(configFilePath);
            Assert.IsTrue(configContent.Contains("\"contract\""));
            Assert.IsTrue(configContent.Contains("\"network\""));
            Assert.IsTrue(configContent.Contains("\"features\""));
            Assert.IsTrue(configContent.Contains("\"ui\""));
        }

        [TestMethod]
        public void Test_WebGuiGeneration_Statistics()
        {
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            var context = results[0];
            var webGuiResult = context.GenerateWebGui(tempTestDir);

            Assert.IsNotNull(webGuiResult.Statistics);
            Assert.IsTrue(webGuiResult.Statistics.MethodCount >= 0);
            Assert.IsTrue(webGuiResult.Statistics.EventCount >= 0);
            Assert.IsTrue(webGuiResult.Statistics.FileCount > 0);
            Assert.IsTrue(webGuiResult.Statistics.TotalFileSize > 0);
            Assert.IsTrue(webGuiResult.Statistics.GenerationTimeMs >= 0);
        }

        [TestMethod]
        public void Test_WebGuiOptions_Customization()
        {
            var options = new WebGuiOptions
            {
                RpcEndpoint = "https://custom.rpc.endpoint",
                NetworkMagic = 12345,
                IncludeTransactionHistory = false,
                IncludeBalanceMonitoring = true,
                IncludeMethodInvocation = true,
                IncludeStateMonitoring = false,
                IncludeEventMonitoring = false,
                IncludeWalletConnection = false,
                RefreshInterval = 120,
                DarkTheme = true,
                ShowAdvancedFeatures = true,
                CustomCss = ".custom { color: blue; }",
                CustomJavaScript = "alert('custom');"
            };

            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            var context = results[0];
            var htmlContent = context.GenerateWebGuiHtml(options);

            Assert.IsTrue(htmlContent.Contains("dark-theme"));
            Assert.IsTrue(htmlContent.Contains("alert('custom');"));
        }

        [TestMethod]
        public void Test_WebGuiGeneration_AccessUrl()
        {
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            var context = results[0];
            var webGuiResult = context.GenerateWebGui(tempTestDir);

            Assert.IsNotNull(webGuiResult.AccessUrl);
            Assert.IsTrue(webGuiResult.AccessUrl.StartsWith("file://"));
            Assert.IsTrue(webGuiResult.AccessUrl.EndsWith("index.html"));
        }

        [TestMethod]
        public void Test_WebGuiGeneration_ErrorHandling_NullContractName()
        {
            var generator = new WebGuiGenerator();
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];
            var manifest = context.CreateManifest();
            var nefFile = context.CreateExecutable();
            var nefBytes = nefFile.Script.ToArray();

            // Test with null contract name
            Assert.ThrowsException<ArgumentException>(() =>
                generator.GenerateWebGui(null!, manifest, nefBytes));
        }

        [TestMethod]
        public void Test_WebGuiGeneration_ErrorHandling_EmptyContractName()
        {
            var generator = new WebGuiGenerator();
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];
            var manifest = context.CreateManifest();
            var nefFile = context.CreateExecutable();
            var nefBytes = nefFile.Script.ToArray();

            // Test with empty contract name
            Assert.ThrowsException<ArgumentException>(() =>
                generator.GenerateWebGui("", manifest, nefBytes));
        }

        [TestMethod]
        public void Test_WebGuiGeneration_ErrorHandling_NullManifest()
        {
            var generator = new WebGuiGenerator();

            // Test with null manifest
            Assert.ThrowsException<ArgumentNullException>(() =>
                generator.GenerateWebGui("TestContract", null!, new byte[] { 1, 2, 3 }));
        }

        [TestMethod]
        public void Test_WebGuiGeneration_ErrorHandling_NullNefBytes()
        {
            var generator = new WebGuiGenerator();
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];
            var manifest = context.CreateManifest();

            // Test with null NEF bytes
            Assert.ThrowsException<ArgumentException>(() =>
                generator.GenerateWebGui("TestContract", manifest, null!));
        }

        [TestMethod]
        public void Test_WebGuiGeneration_ErrorHandling_EmptyNefBytes()
        {
            var generator = new WebGuiGenerator();
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];
            var manifest = context.CreateManifest();

            // Test with empty NEF bytes
            Assert.ThrowsException<ArgumentException>(() =>
                generator.GenerateWebGui("TestContract", manifest, new byte[0]));
        }

        [TestMethod]
        public void Test_WebGuiGeneration_ErrorHandling_InvalidDirectory()
        {
            var generator = new WebGuiGenerator();
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];
            var manifest = context.CreateManifest();
            var nefFile = context.CreateExecutable();
            var nefBytes = nefFile.Script.ToArray();

            // Test with invalid directory path
            var result = generator.GenerateWebGui(
                "TestContract",
                manifest,
                nefBytes,
                null,
                "/invalid/path/that/cannot/be/created"
            );

            Assert.IsFalse(result.Success);
            Assert.IsNotNull(result.ErrorMessage);
        }
    }
}
