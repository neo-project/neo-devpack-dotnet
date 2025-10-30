// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_TemplateManager.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo.Compiler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_TemplateManager
    {
        private TemplateManager _templateManager = null!;
        private string _testOutputPath = null!;

        [TestInitialize]
        public void TestSetup()
        {
            _templateManager = new TemplateManager();
            _testOutputPath = Path.Combine(Path.GetTempPath(), "NeoTemplateTest_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testOutputPath);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (Directory.Exists(_testOutputPath))
            {
                Directory.Delete(_testOutputPath, true);
            }
        }

        [TestMethod]
        public void TestGetAvailableTemplates()
        {
            var templates = _templateManager.GetAvailableTemplates().ToList();

            Assert.AreEqual(5, templates.Count);
            Assert.IsTrue(templates.Any(t => t.template == ContractTemplate.Basic));
            Assert.IsTrue(templates.Any(t => t.template == ContractTemplate.NEP17));
            Assert.IsTrue(templates.Any(t => t.template == ContractTemplate.NEP11));
            Assert.IsTrue(templates.Any(t => t.template == ContractTemplate.Ownable));
            Assert.IsTrue(templates.Any(t => t.template == ContractTemplate.Oracle));
        }

        [TestMethod]
        public void TestGenerateBasicContract()
        {
            string projectName = "TestBasicContract";
            _templateManager.GenerateContract(ContractTemplate.Basic, projectName, _testOutputPath);

            string projectPath = Path.Combine(_testOutputPath, projectName);
            Assert.IsTrue(Directory.Exists(projectPath));

            string csFilePath = Path.Combine(projectPath, $"{projectName}.cs");
            string csprojFilePath = Path.Combine(projectPath, $"{projectName}.csproj");

            Assert.IsTrue(File.Exists(csFilePath));
            Assert.IsTrue(File.Exists(csprojFilePath));

            string csContent = File.ReadAllText(csFilePath);
            Assert.IsTrue(csContent.Contains($"namespace {projectName}"));
            Assert.IsTrue(csContent.Contains($"public class {projectName} : SmartContract"));
            Assert.IsTrue(csContent.Contains("GetMessage"));
            Assert.IsTrue(csContent.Contains("SetMessage"));
        }

        [TestMethod]
        public void TestGenerateNEP17Contract()
        {
            string projectName = "TestNEP17Token";
            _templateManager.GenerateContract(ContractTemplate.NEP17, projectName, _testOutputPath);

            string projectPath = Path.Combine(_testOutputPath, projectName);
            Assert.IsTrue(Directory.Exists(projectPath));

            string csFilePath = Path.Combine(projectPath, $"{projectName}.cs");
            Assert.IsTrue(File.Exists(csFilePath));

            string csContent = File.ReadAllText(csFilePath);
            Assert.IsTrue(csContent.Contains($"namespace {projectName}"));
            Assert.IsTrue(csContent.Contains($"public class {projectName} : Neo.SmartContract.Framework.Nep17Token"));
            Assert.IsTrue(csContent.Contains("[SupportedStandards(NepStandard.Nep17)]"));
            Assert.IsTrue(csContent.Contains("Symbol"));
            Assert.IsTrue(csContent.Contains("Decimals"));
            Assert.IsTrue(csContent.Contains("Mint"));
            Assert.IsTrue(csContent.Contains("Burn"));
        }

        [TestMethod]
        public void TestGenerateCombinedTemplate()
        {
            string projectName = "ComboToken";
            _templateManager.GenerateContractFromFeatures(new[] { "NEP17", "Ownable", "Oracle" }, projectName, _testOutputPath);

            string csFilePath = Path.Combine(_testOutputPath, projectName, $"{projectName}.cs");
            string csContent = File.ReadAllText(csFilePath);

            Assert.IsTrue(csContent.Contains("public class ComboToken : Nep17Token, IOracle"));
            Assert.IsTrue(csContent.Contains("RequestOracleQuote"));
            Assert.IsTrue(csContent.Contains("SetOwner"));
            Assert.IsTrue(csContent.Contains("OracleDataReceived"));

        }

        [TestMethod]
        public void TestGenerateOwnableOracleFeatureCombination()
        {
            string projectName = "OwnableOracleContract";
            _templateManager.GenerateContractFromFeatures(new[] { "Ownable", "Oracle" }, projectName, _testOutputPath);

            string csFilePath = Path.Combine(_testOutputPath, projectName, $"{projectName}.cs");
            string csContent = File.ReadAllText(csFilePath);

            Assert.IsTrue(csContent.Contains("class OwnableOracleContract : SmartContract, IOracle"));
            Assert.IsTrue(csContent.Contains("RequestOracleData"));
            Assert.IsTrue(csContent.Contains("SetOwner"));
            Assert.IsTrue(csContent.Contains("OracleResponseProcessed"));
        }

        [TestMethod]
        public void TestGenerateFeaturesDefaultBasic()
        {
            string projectName = "FeatureBasic";
            _templateManager.GenerateContractFromFeatures(Array.Empty<string>(), projectName, _testOutputPath);

            string csFilePath = Path.Combine(_testOutputPath, projectName, $"{projectName}.cs");
            string csContent = File.ReadAllText(csFilePath);

            Assert.IsTrue(csContent.Contains($"public class {projectName} : SmartContract"));
            Assert.IsTrue(csContent.Contains("SetMessage"));
            Assert.IsTrue(csContent.Contains("GetOwner"));
        }

        [TestMethod]
        public void TestGenerateFeaturesNep17Only()
        {
            string projectName = "FeatureNep17";
            _templateManager.GenerateContractFromFeatures(new[] { "NEP17" }, projectName, _testOutputPath);

            string csFilePath = Path.Combine(_testOutputPath, projectName, $"{projectName}.cs");
            string csContent = File.ReadAllText(csFilePath);

            Assert.IsTrue(csContent.Contains($"public class {projectName} : Neo.SmartContract.Framework.Nep17Token"));
            Assert.IsTrue(csContent.Contains("public override string Symbol"));
        }

        [TestMethod]
        public void TestGenerateFeaturesOwnableOnly()
        {
            string projectName = "FeatureOwnable";
            _templateManager.GenerateContractFromFeatures(new[] { "Ownable" }, projectName, _testOutputPath);

            string csFilePath = Path.Combine(_testOutputPath, projectName, $"{projectName}.cs");
            string csContent = File.ReadAllText(csFilePath);

            Assert.IsTrue(csContent.Contains("public static event OnSetOwnerDelegate OnSetOwner"));
            Assert.IsTrue(csContent.Contains("AllowOperator"));
        }

        [TestMethod]
        public void TestGenerateFeaturesNep11Ownable()
        {
            string projectName = "FeatureNep11Ownable";
            _templateManager.GenerateContractFromFeatures(new[] { "NEP11", "Ownable" }, projectName, _testOutputPath);

            string csFilePath = Path.Combine(_testOutputPath, projectName, $"{projectName}.cs");
            string csContent = File.ReadAllText(csFilePath);

            Assert.IsTrue(csContent.Contains($"public class {projectName} : Nep11Token<TokenState>"));
            Assert.IsTrue(csContent.Contains("public static event OnSetOwnerDelegate OnSetOwner"));
            Assert.IsTrue(csContent.Contains("SetOwner"));
        }

        [TestMethod]
        public void TestGenerateFeaturesInvalidTokenMix()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                _templateManager.GenerateContractFromFeatures(new[] { "NEP17", "NEP11" }, "InvalidTokenMix", _testOutputPath));
        }

        [TestMethod]
        public void TestGenerateFeaturesNep17OracleWithoutOwnable()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                _templateManager.GenerateContractFromFeatures(new[] { "NEP17", "Oracle" }, "InvalidOracleMix", _testOutputPath));
        }

        [TestMethod]
        public void TestGenerateFeaturesNep11OracleNotSupported()
        {
            Assert.ThrowsException<ArgumentException>(() =>
                _templateManager.GenerateContractFromFeatures(new[] { "NEP11", "Oracle" }, "InvalidNep11Oracle", _testOutputPath));
        }

        [TestMethod]
        public void TestGenerateFeaturesAliasParsing()
        {
            string projectName = "AliasCombo";
            _templateManager.GenerateContractFromFeatures(new[] { "nep17+ownable", "oracle" }, projectName, _testOutputPath);

            string csFilePath = Path.Combine(_testOutputPath, projectName, $"{projectName}.cs");
            string csContent = File.ReadAllText(csFilePath);

            Assert.IsTrue(csContent.Contains($"public class {projectName} : Nep17Token, IOracle"));
            Assert.IsTrue(csContent.Contains("OracleDataReceived"));
        }

        [TestMethod]
        public void TestGenerateFeaturesBasicOracle()
        {
            string projectName = "BasicOracle";
            _templateManager.GenerateContractFromFeatures(new[] { "basic", "oracle" }, projectName, _testOutputPath);

            string csFilePath = Path.Combine(_testOutputPath, projectName, $"{projectName}.cs");
            string csContent = File.ReadAllText(csFilePath);

            Assert.IsTrue(csContent.Contains($"public class {projectName} : SmartContract, IOracle"));
            Assert.IsTrue(csContent.Contains("OnOracleResponse"));
        }

        [TestMethod]
        public void TestGenerateFeaturesUnknownThrows()
        {
            var ex = Assert.ThrowsException<ArgumentException>(() =>
                _templateManager.GenerateContractFromFeatures(new[] { "UnknownFeature" }, "UnknownFeatures", _testOutputPath));
            StringAssert.Contains(ex.Message, "Unknown feature");
        }

        [TestMethod]
        public void TestGenerateFeaturesDuplicateHandled()
        {
            string projectName = "DuplicateMix";
            _templateManager.GenerateContractFromFeatures(new[] { "NEP17", "nep17", "Ownable", "Ownable" }, projectName, _testOutputPath);

            string csFilePath = Path.Combine(_testOutputPath, projectName, $"{projectName}.cs");
            string csContent = File.ReadAllText(csFilePath);

            Assert.IsTrue(csContent.Contains($"public class {projectName} : Neo.SmartContract.Framework.Nep17Token"));
            Assert.IsTrue(csContent.Contains("public override string Symbol"));
        }

        [TestMethod]
        public void TestGenerateNEP11Contract()
        {
            string projectName = "TestNFT";
            _templateManager.GenerateContract(ContractTemplate.NEP11, projectName, _testOutputPath);

            string projectPath = Path.Combine(_testOutputPath, projectName);
            Assert.IsTrue(Directory.Exists(projectPath));

            string csFilePath = Path.Combine(projectPath, $"{projectName}.cs");
            Assert.IsTrue(File.Exists(csFilePath));

            string csContent = File.ReadAllText(csFilePath);
            Assert.IsTrue(csContent.Contains($"public class {projectName} : Nep11Token<TokenState>"));
            Assert.IsTrue(csContent.Contains("[SupportedStandards(NepStandard.Nep11)]"));
            Assert.IsTrue(csContent.Contains("TokenState"));
        }

        [TestMethod]
        public void TestGenerateOwnableContract()
        {
            string projectName = "TestOwnable";
            _templateManager.GenerateContract(ContractTemplate.Ownable, projectName, _testOutputPath);

            string projectPath = Path.Combine(_testOutputPath, projectName);
            Assert.IsTrue(Directory.Exists(projectPath));

            string csFilePath = Path.Combine(projectPath, $"{projectName}.cs");
            Assert.IsTrue(File.Exists(csFilePath));

            string csContent = File.ReadAllText(csFilePath);
            Assert.IsTrue(csContent.Contains("GetOwner"));
            Assert.IsTrue(csContent.Contains("SetOwner"));
            Assert.IsTrue(csContent.Contains("IsOwner"));
            Assert.IsTrue(csContent.Contains("OnSetOwner"));
        }

        [TestMethod]
        public void TestGenerateOracleContract()
        {
            string projectName = "TestOracle";
            _templateManager.GenerateContract(ContractTemplate.Oracle, projectName, _testOutputPath);

            string projectPath = Path.Combine(_testOutputPath, projectName);
            Assert.IsTrue(Directory.Exists(projectPath));

            string csFilePath = Path.Combine(projectPath, $"{projectName}.cs");
            Assert.IsTrue(File.Exists(csFilePath));

            string csContent = File.ReadAllText(csFilePath);
            Assert.IsTrue(csContent.Contains($"public class {projectName} : SmartContract, IOracle"));
            Assert.IsTrue(csContent.Contains("OnOracleResponse"));
            Assert.IsTrue(csContent.Contains("Oracle.Request"));
        }

        [TestMethod]
        public void TestCustomReplacements()
        {
            string projectName = "CustomContract";
            var customReplacements = new Dictionary<string, string>
            {
                { "{{Description}}", "Custom Description" },
                { "{{Author}}", "John Doe" },
                { "{{Email}}", "john@example.com" }
            };

            _templateManager.GenerateContract(ContractTemplate.Basic, projectName, _testOutputPath, customReplacements);

            string csFilePath = Path.Combine(_testOutputPath, projectName, $"{projectName}.cs");
            string csContent = File.ReadAllText(csFilePath);

            Assert.IsTrue(csContent.Contains("Custom Description"));
            Assert.IsTrue(csContent.Contains("John Doe"));
            Assert.IsTrue(csContent.Contains("john@example.com"));
        }

        [TestMethod]
        public void TestProjectFileGeneration()
        {
            string projectName = "TestProject";
            _templateManager.GenerateContract(ContractTemplate.Basic, projectName, _testOutputPath);

            string csprojFilePath = Path.Combine(_testOutputPath, projectName, $"{projectName}.csproj");
            Assert.IsTrue(File.Exists(csprojFilePath));

            string csprojContent = File.ReadAllText(csprojFilePath);
            Assert.IsTrue(csprojContent.Contains("<TargetFramework>net9.0</TargetFramework>"));
            Assert.IsTrue(csprojContent.Contains("<PackageReference Include=\"Neo.SmartContract.Framework\""));
            Assert.IsTrue(csprojContent.Contains($"Version=\"{TemplateManager.DefaultFrameworkPackageVersion}\""));
            Assert.IsTrue(csprojContent.Contains("dotnet tool run nccs"));
        }

        [TestMethod]
        public void TestToolManifestGeneration()
        {
            string projectName = "ManifestContract";
            _templateManager.GenerateContract(ContractTemplate.Basic, projectName, _testOutputPath);

            string manifestPath = Path.Combine(_testOutputPath, projectName, ".config", "dotnet-tools.json");
            Assert.IsTrue(File.Exists(manifestPath));

            string manifestContent = File.ReadAllText(manifestPath);
            Assert.IsTrue(manifestContent.Contains("\"neo.compiler.csharp\""));
            Assert.IsTrue(manifestContent.Contains(TemplateManager.DefaultFrameworkPackageVersion));
        }

        [TestMethod]
        public void TestGenerateContractWithTests()
        {
            string projectName = "ContractWithTests";
            _templateManager.GenerateContract(
                ContractTemplate.Basic,
                projectName,
                _testOutputPath,
                new Dictionary<string, string> { { "{{Description}}", "Has tests" } },
                includeTests: true);

            string testProjectDir = Path.Combine(_testOutputPath, $"{projectName}.UnitTests");
            Assert.IsTrue(Directory.Exists(testProjectDir));

            string testCsprojPath = Path.Combine(testProjectDir, $"{projectName}.UnitTests.csproj");
            Assert.IsTrue(File.Exists(testCsprojPath));

            string testClassPath = Directory.GetFiles(testProjectDir, $"{projectName}Tests.cs", SearchOption.AllDirectories).FirstOrDefault();
            Assert.IsNotNull(testClassPath);

            string testClassContent = File.ReadAllText(testClassPath!);
            Assert.IsTrue(testClassContent.Contains($"global::{projectName}.{projectName}"));

            string testCsprojContent = File.ReadAllText(testCsprojPath);
            Assert.IsTrue(testCsprojContent.Contains($"Version=\"{TemplateManager.DefaultFrameworkPackageVersion}\""));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestInvalidTemplate()
        {
            _templateManager.GenerateContract((ContractTemplate)999, "InvalidContract", _testOutputPath);
        }

        [TestMethod]
        public void TestMultipleContractsGeneration()
        {
            var contracts = new[]
            {
                ("Contract1", ContractTemplate.Basic),
                ("Contract2", ContractTemplate.NEP17),
                ("Contract3", ContractTemplate.NEP11)
            };

            foreach (var (name, template) in contracts)
            {
                _templateManager.GenerateContract(template, name, _testOutputPath);
                string projectPath = Path.Combine(_testOutputPath, name);
                Assert.IsTrue(Directory.Exists(projectPath));
                Assert.IsTrue(File.Exists(Path.Combine(projectPath, $"{name}.cs")));
                Assert.IsTrue(File.Exists(Path.Combine(projectPath, $"{name}.csproj")));
            }
        }

        [TestMethod]
        public void TestTemplateTokenReplacement()
        {
            string projectName = "TokenTest";
            var replacements = new Dictionary<string, string>
            {
                { "{{Version}}", "2.0.0" },
                { "{{Year}}", "2025" }
            };

            _templateManager.GenerateContract(ContractTemplate.Basic, projectName, _testOutputPath, replacements);

            string csFilePath = Path.Combine(_testOutputPath, projectName, $"{projectName}.cs");
            string csContent = File.ReadAllText(csFilePath);

            Assert.IsTrue(csContent.Contains("2.0.0"));
            Assert.IsFalse(csContent.Contains("{{Version}}"));
            Assert.IsFalse(csContent.Contains("{{ProjectName}}"));
            Assert.IsFalse(csContent.Contains("{{Namespace}}"));
        }
    }
}
