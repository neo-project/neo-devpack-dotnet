// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_LibraryAPI_ArtifactGeneration.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.Extensions;
using Neo.Json;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using Neo.SmartContract.Testing.Extensions;
using System;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_LibraryAPI_ArtifactGeneration
    {
        private readonly string tempTestDir = Path.Combine(Path.GetTempPath(), "neo-compiler-artifact-tests", Guid.NewGuid().ToString());

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
        public void Test_GenerateArtifacts_CreatesValidCSharpCode()
        {
            // Arrange
            var engine = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended
            });

            var contractSource = CreateTestContractWithMethods();
            var contractPath = Path.Combine(tempTestDir, "ArtifactTestContract.cs");
            File.WriteAllText(contractPath, contractSource);

            // Act
            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];
            Assert.IsTrue(context.Success);

            var (nef, manifest, debugInfo) = context.CreateResults(tempTestDir);
            var artifactSource = manifest.GetArtifactsSource(context.ContractName, nef, debugInfo: debugInfo);

            // Assert
            Assert.IsNotNull(artifactSource);
            Assert.IsTrue(artifactSource.Length > 0);

            // Should contain class definition
            Assert.IsTrue(artifactSource.Contains($"public class {context.ContractName}"));
            
            // Should contain method definitions based on manifest
            foreach (var method in manifest.Abi.Methods.Where(m => m.Name != "_deploy"))
            {
                Assert.IsTrue(artifactSource.Contains(method.Name), 
                    $"Artifact should contain method: {method.Name}");
            }

            // Should contain proper usings
            Assert.IsTrue(artifactSource.Contains("using Neo"));
            Assert.IsTrue(artifactSource.Contains("using System.Numerics"));
        }

        [TestMethod]
        public void Test_GenerateArtifacts_WithComplexTypes()
        {
            // Arrange
            var engine = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended
            });

            var contractSource = CreateContractWithComplexTypes();
            var contractPath = Path.Combine(tempTestDir, "ComplexTypesContract.cs");
            File.WriteAllText(contractPath, contractSource);

            // Act
            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];
            Assert.IsTrue(context.Success);

            var (nef, manifest, debugInfo) = context.CreateResults(tempTestDir);
            var artifactSource = manifest.GetArtifactsSource(context.ContractName, nef, debugInfo: debugInfo);

            // Assert
            Assert.IsNotNull(artifactSource);
            
            // Should handle BigInteger types
            Assert.IsTrue(artifactSource.Contains("BigInteger"));
            
            // Should handle array types
            Assert.IsTrue(artifactSource.Contains("object[]") || artifactSource.Contains("Array"));
            
            // Should be valid C# code that can be parsed
            Assert.IsTrue(IsValidCSharpCode(artifactSource));
        }

        [TestMethod]
        public void Test_GenerateArtifacts_WithEvents()
        {
            // Arrange
            var engine = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended
            });

            var contractSource = CreateContractWithEvents();
            var contractPath = Path.Combine(tempTestDir, "EventsContract.cs");
            File.WriteAllText(contractPath, contractSource);

            // Act
            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];
            Assert.IsTrue(context.Success);

            var (nef, manifest, debugInfo) = context.CreateResults(tempTestDir);
            var artifactSource = manifest.GetArtifactsSource(context.ContractName, nef, debugInfo: debugInfo);

            // Assert
            Assert.IsNotNull(artifactSource);
            
            // Should contain event definitions
            foreach (var eventDef in manifest.Abi.Events)
            {
                Assert.IsTrue(artifactSource.Contains(eventDef.Name),
                    $"Artifact should contain event: {eventDef.Name}");
            }
        }

        [TestMethod]
        public void Test_DebugInfo_ContainsExpectedStructure()
        {
            // Arrange
            var engine = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended
            });

            var contractSource = CreateTestContractWithMethods();
            var contractPath = Path.Combine(tempTestDir, "DebugInfoContract.cs");
            File.WriteAllText(contractPath, contractSource);

            // Act
            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];
            Assert.IsTrue(context.Success);

            var (nef, manifest, debugInfo) = context.CreateResults(tempTestDir);

            // Assert
            Assert.IsNotNull(debugInfo);

            if (debugInfo is JObject debugObj)
            {
                // Should contain essential debug info properties
                Assert.IsTrue(debugObj.ContainsProperty("hash"));
                Assert.IsTrue(debugObj.ContainsProperty("documents"));
                Assert.IsTrue(debugObj.ContainsProperty("methods"));

                // Hash should match the manifest hash
                var debugHash = debugObj["hash"]?.AsString();
                Assert.IsNotNull(debugHash);

                // Documents should contain source file information
                var documents = debugObj["documents"] as JArray;
                Assert.IsNotNull(documents);
                Assert.IsTrue(documents.Count > 0);

                // Methods should contain debug information for contract methods
                var methods = debugObj["methods"] as JArray;
                Assert.IsNotNull(methods);
                Assert.IsTrue(methods.Count > 0);
            }
        }

        [TestMethod]
        public void Test_Assembly_Generation_ContainsInstructions()
        {
            // Arrange
            var engine = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended
            });

            var contractSource = CreateTestContractWithMethods();
            var contractPath = Path.Combine(tempTestDir, "AssemblyContract.cs");
            File.WriteAllText(contractPath, contractSource);

            // Act
            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];
            Assert.IsTrue(context.Success);

            var assembly = context.CreateAssembly();

            // Assert
            Assert.IsNotNull(assembly);
            Assert.IsTrue(assembly.Length > 0);

            // Should contain assembly header
            Assert.IsTrue(assembly.Contains("NeoVM Assembly"));

            // Should contain instruction information
            Assert.IsTrue(assembly.Contains("PUSH") || assembly.Contains("CALL") || assembly.Contains("RET"));

            // Should contain method names or labels
            Assert.IsTrue(assembly.Contains("Add") || assembly.Contains("GetMessage"));
        }

        [TestMethod]
        public void Test_ContractHash_Generation_IsConsistent()
        {
            // Arrange
            var engine = new CompilationEngine();
            var contractSource = CreateTestContractWithMethods();
            var contractPath = Path.Combine(tempTestDir, "HashContract.cs");
            File.WriteAllText(contractPath, contractSource);

            // Act - Compile the same contract multiple times
            var results1 = engine.CompileSources(new[] { contractPath });
            var results2 = engine.CompileSources(new[] { contractPath });

            var hash1 = results1[0].GetContractHash();
            var hash2 = results2[0].GetContractHash();

            // Assert
            Assert.IsNotNull(hash1);
            Assert.IsNotNull(hash2);
            Assert.AreEqual(hash1, hash2, "Contract hash should be deterministic");
        }

        [TestMethod]
        public void Test_NEF_File_Structure()
        {
            // Arrange
            var engine = new CompilationEngine(new CompilationOptions
            {
                Debug = CompilationOptions.DebugType.Extended
            });

            var contractSource = CreateTestContractWithMethods();
            var contractPath = Path.Combine(tempTestDir, "NEFContract.cs");
            File.WriteAllText(contractPath, contractSource);

            // Act
            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];
            Assert.IsTrue(context.Success);

            var (nef, manifest, debugInfo) = context.CreateResults(tempTestDir);

            // Assert
            Assert.IsNotNull(nef);
            Assert.IsNotNull(nef.Script);
            Assert.IsTrue(nef.Script.Length > 0);
            Assert.IsNotNull(nef.Compiler);
            Assert.IsTrue(nef.Compiler.Contains("Neo.Compiler.CSharp"));

            // NEF should be serializable
            var nefBytes = nef.ToArray();
            Assert.IsNotNull(nefBytes);
            Assert.IsTrue(nefBytes.Length > 0);

            // Should be able to deserialize back
            var deserializedNef = NefFile.Parse(nefBytes);
            Assert.AreEqual(nef.Script.ToArray(), deserializedNef.Script.ToArray());
        }

        [TestMethod]
        public void Test_Manifest_Structure()
        {
            // Arrange
            var engine = new CompilationEngine();
            var contractSource = CreateContractWithPermissions();
            var contractPath = Path.Combine(tempTestDir, "ManifestContract.cs");
            File.WriteAllText(contractPath, contractSource);

            // Act
            var results = engine.CompileSources(new[] { contractPath });
            var context = results[0];
            Assert.IsTrue(context.Success);

            var (nef, manifest, debugInfo) = context.CreateResults(tempTestDir);

            // Assert
            Assert.IsNotNull(manifest);
            Assert.IsNotNull(manifest.Name);
            Assert.IsNotNull(manifest.Abi);
            Assert.IsNotNull(manifest.Permissions);
            Assert.IsNotNull(manifest.Trusts);
            Assert.IsNotNull(manifest.Groups);

            // ABI should contain methods
            Assert.IsTrue(manifest.Abi.Methods.Length > 0);

            // Should have proper serialization
            var manifestJson = manifest.ToJson();
            Assert.IsNotNull(manifestJson);

            // Should be able to deserialize back
            var deserializedManifest = ContractManifest.Parse(manifestJson.ToString());
            Assert.AreEqual(manifest.Name, deserializedManifest.Name);
        }

        private string CreateTestContractWithMethods()
        {
            return @"
using Neo.SmartContract.Framework;
using System.Numerics;

namespace TestContracts
{
    public class TestContract : SmartContract
    {
        public static int Add(int a, int b)
        {
            return a + b;
        }

        public static string GetMessage()
        {
            return ""Hello, World!"";
        }

        public static bool IsPositive(BigInteger value)
        {
            return value > 0;
        }

        public static byte[] GetBytes()
        {
            return new byte[] { 1, 2, 3, 4 };
        }
    }
}";
        }

        private string CreateContractWithComplexTypes()
        {
            return @"
using Neo.SmartContract.Framework;
using System.Numerics;

namespace TestContracts
{
    public class ComplexContract : SmartContract
    {
        public static BigInteger CalculateSum(BigInteger[] numbers)
        {
            BigInteger sum = 0;
            foreach (var num in numbers)
            {
                sum += num;
            }
            return sum;
        }

        public static object[] GetInfo(string name, int age)
        {
            return new object[] { name, age, true };
        }

        public static string[] GetStringArray()
        {
            return new string[] { ""first"", ""second"", ""third"" };
        }
    }
}";
        }

        private string CreateContractWithEvents()
        {
            return @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services;

namespace TestContracts
{
    public class EventContract : SmartContract
    {
        [DisplayName(""Transfer"")]
        public static event System.Action<byte[], byte[], System.Numerics.BigInteger> OnTransfer;

        [DisplayName(""Message"")]
        public static event System.Action<string> OnMessage;

        public static void TriggerTransfer(byte[] from, byte[] to, System.Numerics.BigInteger amount)
        {
            OnTransfer(from, to, amount);
        }

        public static void TriggerMessage(string message)
        {
            OnMessage(message);
        }
    }
}";
        }

        private string CreateContractWithPermissions()
        {
            return @"
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Attributes;

namespace TestContracts
{
    [ContractPermission(""*"", ""*"")]
    public class PermissionContract : SmartContract
    {
        public static int GetValue()
        {
            return 42;
        }
    }
}";
        }

        private bool IsValidCSharpCode(string code)
        {
            try
            {
                var syntaxTree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(code);
                var diagnostics = syntaxTree.GetDiagnostics();
                return !diagnostics.Any(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error);
            }
            catch
            {
                return false;
            }
        }
    }
}