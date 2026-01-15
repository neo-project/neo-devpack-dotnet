// Copyright (C) 2015-2026 The Neo Project.
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
            Assert.IsTrue(csprojContent.Contains("Version=\"3.8.1\""));
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
