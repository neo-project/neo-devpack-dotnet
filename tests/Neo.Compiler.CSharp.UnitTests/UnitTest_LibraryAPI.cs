// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_LibraryAPI.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Extensions;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_LibraryAPI
    {
        private readonly string testContractsPath = Utils.Extensions.TestContractRoot;
        private readonly string tempTestDir = Path.Combine(Path.GetTempPath(), "neo-compiler-tests", Guid.NewGuid().ToString());

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
        public void Test_CompilationEngine_DefaultConstructor()
        {
            var engine = new CompilationEngine();
            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void Test_CompilationEngine_WithOptions()
        {
            var options = new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended,
                Optimize = CompilationOptions.OptimizationType.All,
                Nullable = NullableContextOptions.Enable,
                Checked = true,
                NoInline = false,
                AddressVersion = 0x35,
                BaseName = "TestContract"
            };

            var engine = new CompilationEngine(options);
            Assert.IsNotNull(engine);
        }

        [TestMethod]
        public void Test_CompilationOptions_DefaultValues()
        {
            var options = new CompilationOptions();
            
            Assert.AreEqual(CompilationOptions.DebugType.None, options.Debug);
            Assert.AreEqual(CompilationOptions.OptimizationType.Basic, options.Optimize);
            Assert.AreEqual(false, options.Checked);
            Assert.AreEqual(false, options.NoInline);
            Assert.AreEqual((byte)0x35, options.AddressVersion);
            Assert.IsNotNull(options.CompilerVersion);
            Assert.IsTrue(options.CompilerVersion.Contains("Neo.Compiler.CSharp"));
        }

        [TestMethod]
        public void Test_CompileSources_SingleFile()
        {
            var engine = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended,
                Optimize = CompilationOptions.OptimizationType.All
            });

            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            Assert.AreEqual(1, results.Count);
            Assert.IsTrue(results[0].Success);
            Assert.IsNotNull(results[0].ContractName);
            Assert.IsTrue(results[0].Diagnostics.All(d => d.Severity != DiagnosticSeverity.Error));
        }

        [TestMethod]
        public void Test_CompileSources_MultipleFiles()
        {
            var engine = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended,
                Optimize = CompilationOptions.OptimizationType.All
            });

            var contractPaths = new[]
            {
                Path.Combine(testContractsPath, "Contract_BigInteger.cs"),
                Path.Combine(testContractsPath, "Contract_Math.cs")
            };
            var results = engine.CompileSources(contractPaths);

            Assert.IsTrue(results.Count >= 2);
            foreach (var result in results)
            {
                Assert.IsTrue(result.Success);
                Assert.IsNotNull(result.ContractName);
            }
        }

        [TestMethod]
        public void Test_CreateResults_GeneratesArtifacts()
        {
            var engine = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended,
                Optimize = CompilationOptions.OptimizationType.All
            });

            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            Assert.AreEqual(1, results.Count);
            var context = results[0];
            Assert.IsTrue(context.Success);

            var (nef, manifest, debugInfo) = context.CreateResults(tempTestDir);

            // Verify NEF file
            Assert.IsNotNull(nef);
            Assert.IsTrue(nef.Script.Length > 0);
            // NEF file should have valid structure

            // Verify manifest
            Assert.IsNotNull(manifest);
            Assert.IsNotNull(manifest.Name);
            Assert.IsNotNull(manifest.Abi);
            Assert.IsNotNull(manifest.Permissions);

            // Verify debug info
            Assert.IsNotNull(debugInfo);
            if (debugInfo is JObject debugObj)
            {
                Assert.IsTrue(debugObj.ContainsProperty("hash"));
                Assert.IsTrue(debugObj.ContainsProperty("documents"));
            }
        }

        [TestMethod]
        public void Test_CompilationWithDifferentOptimizations()
        {
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");

            // Test with no optimization
            var engineNone = new CompilationEngine(new CompilationOptions
            {
                Optimize = CompilationOptions.OptimizationType.None
            });
            var resultsNone = engineNone.CompileSources(new[] { contractPath });
            var (nefNone, _, _) = resultsNone[0].CreateResults(tempTestDir);

            // Test with basic optimization
            var engineBasic = new CompilationEngine(new CompilationOptions
            {
                Optimize = CompilationOptions.OptimizationType.Basic
            });
            var resultsBasic = engineBasic.CompileSources(new[] { contractPath });
            var (nefBasic, _, _) = resultsBasic[0].CreateResults(tempTestDir);

            // Test with all optimizations
            var engineAll = new CompilationEngine(new CompilationOptions
            {
                Optimize = CompilationOptions.OptimizationType.All
            });
            var resultsAll = engineAll.CompileSources(new[] { contractPath });
            var (nefAll, _, _) = resultsAll[0].CreateResults(tempTestDir);

            // All should compile successfully
            Assert.IsTrue(resultsNone[0].Success);
            Assert.IsTrue(resultsBasic[0].Success);
            Assert.IsTrue(resultsAll[0].Success);

            // Optimized versions should generally be smaller or equal in size
            Assert.IsTrue(nefAll.Script.Length <= nefNone.Script.Length);
        }

        [TestMethod]
        public void Test_CompilationWithDifferentDebugLevels()
        {
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");

            // Test with no debug info
            var engineNone = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.None
            });
            var resultsNone = engineNone.CompileSources(new[] { contractPath });
            var (_, _, debugNone) = resultsNone[0].CreateResults(tempTestDir);

            // Test with strict debug info
            var engineStrict = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Strict
            });
            var resultsStrict = engineStrict.CompileSources(new[] { contractPath });
            var (_, _, debugStrict) = resultsStrict[0].CreateResults(tempTestDir);

            // Test with extended debug info
            var engineExtended = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended
            });
            var resultsExtended = engineExtended.CompileSources(new[] { contractPath });
            var (_, _, debugExtended) = resultsExtended[0].CreateResults(tempTestDir);

            // All should compile successfully
            Assert.IsTrue(resultsNone[0].Success);
            Assert.IsTrue(resultsStrict[0].Success);
            Assert.IsTrue(resultsExtended[0].Success);

            // Debug info should vary based on debug level
            if (debugNone is JToken debugNoneToken && debugExtended is JToken debugExtendedToken)
            {
                // Extended debug should have more information
                Assert.IsTrue(debugExtendedToken.ToString().Length >= debugNoneToken.ToString().Length);
            }
        }

        [TestMethod]
        public void Test_InvalidSourceFile_ReturnsFailure()
        {
            var engine = new CompilationEngine();
            var invalidPath = Path.Combine(tempTestDir, "nonexistent.cs");

            var results = engine.CompileSources(new[] { invalidPath });

            Assert.AreEqual(1, results.Count);
            Assert.IsFalse(results[0].Success);
            Assert.IsTrue(results[0].Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error));
        }

        [TestMethod]
        public void Test_CompilationWithSyntaxErrors()
        {
            // Create a contract with syntax errors
            var invalidContract = @"
using Neo.SmartContract.Framework;

namespace TestContract
{
    public class InvalidContract : SmartContract
    {
        public static void Test()
        {
            // Missing semicolon - syntax error
            int x = 5
            return x;
        }
    }
}";

            var invalidContractPath = Path.Combine(tempTestDir, "InvalidContract.cs");
            File.WriteAllText(invalidContractPath, invalidContract);

            var engine = new CompilationEngine();
            var results = engine.CompileSources(new[] { invalidContractPath });

            Assert.AreEqual(1, results.Count);
            Assert.IsFalse(results[0].Success);
            Assert.IsTrue(results[0].Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error));
        }

        [TestMethod]
        public void Test_CompileProject_WithValidProject()
        {
            // Create a simple test project
            var projectDir = Path.Combine(tempTestDir, "TestProject");
            Directory.CreateDirectory(projectDir);

            var projectFile = Path.Combine(projectDir, "TestProject.csproj");
            var projectContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""Neo.SmartContract.Framework"" Version=""3.9.0"" />
  </ItemGroup>
</Project>";
            File.WriteAllText(projectFile, projectContent);

            var contractFile = Path.Combine(projectDir, "TestContract.cs");
            var contractContent = @"
using Neo.SmartContract.Framework;

namespace TestProject
{
    public class TestContract : SmartContract
    {
        public static int Add(int a, int b)
        {
            return a + b;
        }
    }
}";
            File.WriteAllText(contractFile, contractContent);

            var engine = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended
            });

            var results = engine.CompileProject(projectFile);

            Assert.IsTrue(results.Count >= 1);
            var result = results.FirstOrDefault(r => r.ContractName == "TestContract");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public void Test_CompilationContext_Properties()
        {
            var engine = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended,
                BaseName = "CustomBaseName"
            });

            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            Assert.AreEqual(1, results.Count);
            var context = results[0];

            Assert.IsTrue(context.Success);
            Assert.IsNotNull(context.ContractName);
            Assert.IsNotNull(context.Diagnostics);
            Assert.IsTrue(context.Diagnostics.Count >= 0);
        }

        [TestMethod]
        public void Test_GetContractHash_ReturnsValidHash()
        {
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");
            var results = engine.CompileSources(new[] { contractPath });

            Assert.AreEqual(1, results.Count);
            var context = results[0];
            Assert.IsTrue(context.Success);

            var contractHash = context.GetContractHash();
            Assert.IsNotNull(contractHash);
            Assert.AreEqual(20, contractHash.ToArray().Length); // UInt160 is 20 bytes
        }

        [TestMethod]
        public void Test_CreateAssembly_GeneratesAssemblyCode()
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

            var assembly = context.CreateAssembly();
            Assert.IsNotNull(assembly);
            Assert.IsTrue(assembly.Length > 0);
            Assert.IsTrue(assembly.Contains("NeoVM Assembly"));
        }

        [TestMethod]
        public void Test_CompilationOptions_ParseOptions()
        {
            var options = new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended
            };

            var parseOptions = options.GetParseOptions();
            Assert.IsNotNull(parseOptions);
            Assert.IsTrue(parseOptions.PreprocessorSymbolNames.Contains("DEBUG"));

            // Test that parse options are cached
            var parseOptions2 = options.GetParseOptions();
            Assert.AreSame(parseOptions, parseOptions2);
        }

        [TestMethod]
        public void Test_CompilationOptions_NoDebug_NoPreprocessorSymbols()
        {
            var options = new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.None
            };

            var parseOptions = options.GetParseOptions();
            Assert.IsNotNull(parseOptions);
            Assert.IsFalse(parseOptions.PreprocessorSymbolNames.Contains("DEBUG"));
        }

        [TestMethod]
        public void Test_CompilationEngine_ThreadSafety()
        {
            var engine = new CompilationEngine();
            var contractPath = Path.Combine(testContractsPath, "Contract_BigInteger.cs");

            // Test multiple concurrent compilations
            var tasks = Enumerable.Range(0, 5).Select(_ => 
                System.Threading.Tasks.Task.Run(() => engine.CompileSources(new[] { contractPath }))
            ).ToArray();

            System.Threading.Tasks.Task.WaitAll(tasks);

            foreach (var task in tasks)
            {
                var results = task.Result;
                Assert.AreEqual(1, results.Count);
                Assert.IsTrue(results[0].Success);
            }
        }
    }
}