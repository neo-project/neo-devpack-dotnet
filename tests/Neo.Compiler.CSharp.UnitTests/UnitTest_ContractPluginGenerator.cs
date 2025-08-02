// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_ContractPluginGenerator.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using Neo.SmartContract;
using Neo.SmartContract.Manifest;
using System;
using System.IO;
using System.Linq;
using Neo.Json;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_ContractPluginGenerator
    {
        private string _testOutputPath = null!;
        private ContractManifest _testManifest = null!;
        private UInt160 _testContractHash = null!;

        [TestInitialize]
        public void Initialize()
        {
            _testOutputPath = Path.Combine(Path.GetTempPath(), $"PluginGenTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testOutputPath);

            // Create a test manifest
            _testManifest = new ContractManifest
            {
                Name = "TestContract",
                Abi = new ContractAbi
                {
                    Methods = new[]
                    {
                        new ContractMethodDescriptor
                        {
                            Name = "testMethod",
                            Parameters = new[]
                            {
                                new ContractParameterDefinition
                                {
                                    Name = "value",
                                    Type = ContractParameterType.Integer
                                }
                            },
                            ReturnType = ContractParameterType.String,
                            Safe = true
                        },
                        new ContractMethodDescriptor
                        {
                            Name = "setState",
                            Parameters = new[]
                            {
                                new ContractParameterDefinition
                                {
                                    Name = "key",
                                    Type = ContractParameterType.String
                                },
                                new ContractParameterDefinition
                                {
                                    Name = "value",
                                    Type = ContractParameterType.ByteArray
                                }
                            },
                            ReturnType = ContractParameterType.Boolean,
                            Safe = false
                        },
                        new ContractMethodDescriptor
                        {
                            Name = "noParams",
                            Parameters = System.Array.Empty<ContractParameterDefinition>(),
                            ReturnType = ContractParameterType.Integer,
                            Safe = true
                        },
                        new ContractMethodDescriptor
                        {
                            Name = "_deploy",
                            Parameters = new[]
                            {
                                new ContractParameterDefinition
                                {
                                    Name = "data",
                                    Type = ContractParameterType.Any
                                },
                                new ContractParameterDefinition
                                {
                                    Name = "update",
                                    Type = ContractParameterType.Boolean
                                }
                            },
                            ReturnType = ContractParameterType.Void,
                            Safe = false
                        }
                    }
                }
            };

            _testContractHash = UInt160.Parse("0x1234567890123456789012345678901234567890");
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_testOutputPath))
            {
                Directory.Delete(_testOutputPath, true);
            }
        }

        [TestMethod]
        public void TestGeneratePlugin_CreatesPluginDirectory()
        {
            // Act
            ContractPluginGenerator.GeneratePlugin("TestContract", _testManifest, _testContractHash, _testOutputPath);

            // Assert
            string pluginPath = Path.Combine(_testOutputPath, "TestContractPlugin");
            Assert.IsTrue(Directory.Exists(pluginPath), "Plugin directory should be created");
        }

        [TestMethod]
        public void TestGeneratePlugin_CreatesAllRequiredFiles()
        {
            // Act
            ContractPluginGenerator.GeneratePlugin("TestContract", _testManifest, _testContractHash, _testOutputPath);

            // Assert
            string pluginPath = Path.Combine(_testOutputPath, "TestContractPlugin");
            Assert.IsTrue(File.Exists(Path.Combine(pluginPath, "TestContractPlugin.cs")), "Main plugin file should exist");
            Assert.IsTrue(File.Exists(Path.Combine(pluginPath, "TestContractPlugin.csproj")), "Project file should exist");
            Assert.IsTrue(File.Exists(Path.Combine(pluginPath, "TestContractPlugin.json")), "Configuration file should exist");
            Assert.IsTrue(File.Exists(Path.Combine(pluginPath, "TestContractCommands.cs")), "Commands file should exist");
            Assert.IsTrue(File.Exists(Path.Combine(pluginPath, "TestContractWrapper.cs")), "Wrapper file should exist");
        }

        [TestMethod]
        public void TestGeneratePlugin_MainPluginFileContent()
        {
            // Act
            ContractPluginGenerator.GeneratePlugin("TestContract", _testManifest, _testContractHash, _testOutputPath);

            // Assert
            string pluginPath = Path.Combine(_testOutputPath, "TestContractPlugin");
            string content = File.ReadAllText(Path.Combine(pluginPath, "TestContractPlugin.cs"));

            Assert.IsTrue(content.Contains("public class TestContractPlugin : Plugin"), "Should contain plugin class");
            Assert.IsTrue(content.Contains("public override string Name => \"TestContractPlugin\""), "Should contain name property");
            Assert.IsTrue(content.Contains("Contract Hash: " + _testContractHash.ToString()), "Should contain contract hash");
            Assert.IsTrue(content.Contains("protected override void OnSystemLoaded(NeoSystem system)"), "Should contain OnSystemLoaded method");
            Assert.IsTrue(content.Contains("ConsoleHelper.RegisterCommand(\"testcontract\""), "Should register CLI command");
        }

        [TestMethod]
        public void TestGeneratePlugin_ProjectFileContent()
        {
            // Act
            ContractPluginGenerator.GeneratePlugin("TestContract", _testManifest, _testContractHash, _testOutputPath);

            // Assert
            string pluginPath = Path.Combine(_testOutputPath, "TestContractPlugin");
            string content = File.ReadAllText(Path.Combine(pluginPath, "TestContractPlugin.csproj"));

            Assert.IsTrue(content.Contains("<TargetFramework>net9.0</TargetFramework>"), "Should target .NET 9.0");
            Assert.IsTrue(content.Contains("<EnableDynamicLoading>true</EnableDynamicLoading>"), "Should enable dynamic loading");
            Assert.IsTrue(content.Contains("TestContractPlugin.json"), "Should include configuration file");
        }

        [TestMethod]
        public void TestGeneratePlugin_ConfigurationFileContent()
        {
            // Act
            ContractPluginGenerator.GeneratePlugin("TestContract", _testManifest, _testContractHash, _testOutputPath);

            // Assert
            string pluginPath = Path.Combine(_testOutputPath, "TestContractPlugin");
            string content = File.ReadAllText(Path.Combine(pluginPath, "TestContractPlugin.json"));
            var config = JObject.Parse(content);

            Assert.AreEqual(_testContractHash.ToString(), config["PluginConfiguration"]?["ContractHash"]?.GetString());
            Assert.AreEqual(860833102, config["PluginConfiguration"]?["Network"]?.GetInt32());
            var maxGas = config["PluginConfiguration"]?["MaxGasPerTransaction"];
            Assert.AreEqual(50_00000000L, maxGas?.AsNumber() ?? 0);
        }

        [TestMethod]
        public void TestGeneratePlugin_CommandsFileContent()
        {
            // Act
            ContractPluginGenerator.GeneratePlugin("TestContract", _testManifest, _testContractHash, _testOutputPath);

            // Assert
            string pluginPath = Path.Combine(_testOutputPath, "TestContractPlugin");
            string content = File.ReadAllText(Path.Combine(pluginPath, "TestContractCommands.cs"));

            // Should have handler methods for non-private methods
            Assert.IsTrue(content.Contains("private async Task HandletestMethod"), "Should have testMethod handler");
            Assert.IsTrue(content.Contains("private async Task HandlesetState"), "Should have setState handler");
            Assert.IsTrue(content.Contains("private async Task HandlenoParams"), "Should have noParams handler");

            // Should NOT have handler for private methods
            Assert.IsFalse(content.Contains("private async Task Handle_deploy"), "Should NOT have _deploy handler");

            // Should have help method
            Assert.IsTrue(content.Contains("private void ShowHelp()"), "Should have ShowHelp method");
            Assert.IsTrue(content.Contains("testmethod <value:Integer> [SAFE]"), "Should show testMethod in help");
            Assert.IsTrue(content.Contains("setstate <key:String> <value:ByteArray>"), "Should show setState in help");
        }

        [TestMethod]
        public void TestGeneratePlugin_WrapperFileContent()
        {
            // Act
            ContractPluginGenerator.GeneratePlugin("TestContract", _testManifest, _testContractHash, _testOutputPath);

            // Assert
            string pluginPath = Path.Combine(_testOutputPath, "TestContractPlugin");
            string content = File.ReadAllText(Path.Combine(pluginPath, "TestContractWrapper.cs"));

            Assert.IsTrue(content.Contains($"private static readonly UInt160 ContractHash = UInt160.Parse(\"{_testContractHash}\")"),
                "Should contain contract hash");
            Assert.IsTrue(content.Contains("public async Task<string> testMethodAsync(BigInteger value)"),
                "Should have async wrapper for testMethod");
            Assert.IsTrue(content.Contains("public async Task<bool> setStateAsync(string key, byte[] value)"),
                "Should have async wrapper for setState");
            Assert.IsTrue(content.Contains("public async Task<BigInteger> noParamsAsync()"),
                "Should have async wrapper for noParams");
        }

        [TestMethod]
        public void TestGeneratePlugin_HandlesSpecialCharactersInContractName()
        {
            // Arrange
            string specialName = "Test-Contract_123";

            // Act
            ContractPluginGenerator.GeneratePlugin(specialName, _testManifest, _testContractHash, _testOutputPath);

            // Assert
            string pluginPath = Path.Combine(_testOutputPath, $"{specialName}Plugin");
            Assert.IsTrue(Directory.Exists(pluginPath), "Plugin directory should be created with special characters");
        }

        [TestMethod]
        public void TestGeneratePlugin_TypeConversions()
        {
            // Arrange
            var manifest = new ContractManifest
            {
                Name = "TypeTest",
                Abi = new ContractAbi
                {
                    Methods = new[]
                    {
                        new ContractMethodDescriptor
                        {
                            Name = "allTypes",
                            Parameters = new[]
                            {
                                new ContractParameterDefinition { Name = "boolParam", Type = ContractParameterType.Boolean },
                                new ContractParameterDefinition { Name = "intParam", Type = ContractParameterType.Integer },
                                new ContractParameterDefinition { Name = "stringParam", Type = ContractParameterType.String },
                                new ContractParameterDefinition { Name = "byteArrayParam", Type = ContractParameterType.ByteArray },
                                new ContractParameterDefinition { Name = "hash160Param", Type = ContractParameterType.Hash160 },
                                new ContractParameterDefinition { Name = "hash256Param", Type = ContractParameterType.Hash256 },
                                new ContractParameterDefinition { Name = "pubKeyParam", Type = ContractParameterType.PublicKey },
                                new ContractParameterDefinition { Name = "arrayParam", Type = ContractParameterType.Array }
                            },
                            ReturnType = ContractParameterType.Map,
                            Safe = false
                        }
                    }
                }
            };

            // Act
            ContractPluginGenerator.GeneratePlugin("TypeTest", manifest, _testContractHash, _testOutputPath);

            // Assert
            string pluginPath = Path.Combine(_testOutputPath, "TypeTestPlugin");
            string wrapperContent = File.ReadAllText(Path.Combine(pluginPath, "TypeTestWrapper.cs"));

            Assert.IsTrue(wrapperContent.Contains("bool boolParam"), "Should have bool parameter");
            Assert.IsTrue(wrapperContent.Contains("BigInteger intParam"), "Should have BigInteger parameter");
            Assert.IsTrue(wrapperContent.Contains("string stringParam"), "Should have string parameter");
            Assert.IsTrue(wrapperContent.Contains("byte[] byteArrayParam"), "Should have byte[] parameter");
            Assert.IsTrue(wrapperContent.Contains("UInt160 hash160Param"), "Should have UInt160 parameter");
            Assert.IsTrue(wrapperContent.Contains("UInt256 hash256Param"), "Should have UInt256 parameter");
            Assert.IsTrue(wrapperContent.Contains("ECPoint pubKeyParam"), "Should have ECPoint parameter");
            Assert.IsTrue(wrapperContent.Contains("object[] arrayParam"), "Should have object[] parameter");
            Assert.IsTrue(wrapperContent.Contains("Task<Dictionary<object, object>>"), "Should return Dictionary for Map type");
        }

        [TestMethod]
        public void TestGeneratePlugin_WithConfigurationOptions()
        {
            // Arrange
            var options = new Options
            {
                PluginNeoVersion = "3.6.0",
                PluginMaxGas = 100_00000000L,
                PluginNetworkId = 5195086
            };

            // Act
            ContractPluginGenerator.GeneratePlugin("TestContract", _testManifest, _testContractHash, _testOutputPath, options);

            // Assert - Check project file
            string pluginPath = Path.Combine(_testOutputPath, "TestContractPlugin");
            string projectContent = File.ReadAllText(Path.Combine(pluginPath, "TestContractPlugin.csproj"));
            Assert.IsTrue(projectContent.Contains("Version=\"3.6.0\""), "Should use custom Neo version");

            // Assert - Check configuration file
            string configContent = File.ReadAllText(Path.Combine(pluginPath, "TestContractPlugin.json"));
            var config = JObject.Parse(configContent);
            Assert.AreEqual(5195086, config["PluginConfiguration"]?["Network"]?.GetInt32());
            var maxGas = config["PluginConfiguration"]?["MaxGasPerTransaction"];
            Assert.AreEqual(100_00000000L, maxGas?.AsNumber() ?? 0);
        }

        [TestMethod]
        public void TestGeneratePlugin_InvalidOutputPath_CreatesDirectory()
        {
            // Arrange
            string invalidPath = Path.Combine(_testOutputPath, "non", "existent", "path");

            // Act
            ContractPluginGenerator.GeneratePlugin("TestContract", _testManifest, _testContractHash, invalidPath);

            // Assert
            string pluginPath = Path.Combine(invalidPath, "TestContractPlugin");
            Assert.IsTrue(Directory.Exists(pluginPath), "Should create nested directories");
        }
    }
}
