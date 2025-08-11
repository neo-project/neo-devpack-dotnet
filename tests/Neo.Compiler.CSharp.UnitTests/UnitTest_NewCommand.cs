// Copyright (C) 2015-2025 The Neo Project.
//
// UnitTest_NewCommand.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_NewCommand
    {
        private string _testOutputPath = null!;
        private string _compilerPath = null!;
        private static bool IsCI => Environment.GetEnvironmentVariable("CI") == "true" || 
                                    Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true";

        [TestInitialize]
        public void TestSetup()
        {
            if (IsCI)
            {
                return;
            }
            
            _testOutputPath = Path.Combine(Path.GetTempPath(), "NeoNewCommandTest_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testOutputPath);

            // Get the path to the compiler project
            var currentDir = Directory.GetCurrentDirectory();
            _compilerPath = Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", "..", "..", "src", "Neo.Compiler.CSharp", "Neo.Compiler.CSharp.csproj"));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (!string.IsNullOrEmpty(_testOutputPath) && Directory.Exists(_testOutputPath))
            {
                Directory.Delete(_testOutputPath, true);
            }
        }

        [TestMethod]
        public void TestNewCommandHelp()
        {
            if (IsCI)
            {
                Assert.Inconclusive("Skipping integration tests in CI environment");
                return;
            }
            var result = RunCompilerCommand("new --help");

            Assert.IsTrue(result.Contains("Create a new smart contract from a template"));
            Assert.IsTrue(result.Contains("--template"));
            Assert.IsTrue(result.Contains("--author"));
            Assert.IsTrue(result.Contains("--email"));
            Assert.IsTrue(result.Contains("--description"));
        }

        [TestMethod]
        public void TestNewCommandBasicContract()
        {
            if (IsCI)
            {
                Assert.Inconclusive("Skipping integration tests in CI environment");
                return;
            }
            string contractName = "TestBasic";
            var result = RunCompilerCommand($"new {contractName} -t Basic --output \"{_testOutputPath}\"");

            Assert.IsTrue(result.Contains($"Creating Basic contract: {contractName}"));
            Assert.IsTrue(result.Contains($"Successfully created Basic contract '{contractName}'"));

            string projectPath = Path.Combine(_testOutputPath, contractName);
            Assert.IsTrue(Directory.Exists(projectPath));
            Assert.IsTrue(File.Exists(Path.Combine(projectPath, $"{contractName}.cs")));
            Assert.IsTrue(File.Exists(Path.Combine(projectPath, $"{contractName}.csproj")));
        }

        [TestMethod]
        public void TestNewCommandNEP17Contract()
        {
            if (IsCI)
            {
                Assert.Inconclusive("Skipping integration tests in CI environment");
                return;
            }
            string contractName = "TestToken";
            var result = RunCompilerCommand($"new {contractName} -t NEP17 --output \"{_testOutputPath}\" --description \"Test Token Contract\"");

            Assert.IsTrue(result.Contains($"Creating NEP17 contract: {contractName}"));
            Assert.IsTrue(result.Contains("Description: Test Token Contract"));

            string csFilePath = Path.Combine(_testOutputPath, contractName, $"{contractName}.cs");
            Assert.IsTrue(File.Exists(csFilePath));

            string content = File.ReadAllText(csFilePath);
            Assert.IsTrue(content.Contains("Test Token Contract"));
            Assert.IsTrue(content.Contains("Nep17Token"));
        }

        [TestMethod]
        public void TestNewCommandWithCustomAuthor()
        {
            if (IsCI)
            {
                Assert.Inconclusive("Skipping integration tests in CI environment");
                return;
            }
            string contractName = "AuthorTest";
            var result = RunCompilerCommand($"new {contractName} --output \"{_testOutputPath}\" --author \"Jane Smith\" --email \"jane@test.com\"");

            Assert.IsTrue(result.Contains("Author: Jane Smith"));
            Assert.IsTrue(result.Contains("Email: jane@test.com"));

            string csFilePath = Path.Combine(_testOutputPath, contractName, $"{contractName}.cs");
            string content = File.ReadAllText(csFilePath);

            Assert.IsTrue(content.Contains("Jane Smith"));
            Assert.IsTrue(content.Contains("jane@test.com"));
        }

        [TestMethod]
        public void TestNewCommandInvalidName()
        {
            if (IsCI)
            {
                Assert.Inconclusive("Skipping integration tests in CI environment");
                return;
            }
            var result = RunCompilerCommand($"new 123Invalid --output \"{_testOutputPath}\"");

            Assert.IsTrue(result.Contains("Error: Contract name must start with a letter"));
        }

        [TestMethod]
        public void TestNewCommandExistingDirectory()
        {
            if (IsCI)
            {
                Assert.Inconclusive("Skipping integration tests in CI environment");
                return;
            }
            string contractName = "ExistingContract";
            string projectPath = Path.Combine(_testOutputPath, contractName);
            Directory.CreateDirectory(projectPath);

            var result = RunCompilerCommand($"new {contractName} --output \"{_testOutputPath}\"");

            Assert.IsTrue(result.Contains($"Directory '{projectPath}' already exists"));
            Assert.IsTrue(result.Contains("Use --force to overwrite"));
        }

        [TestMethod]
        public void TestNewCommandForceOverwrite()
        {
            if (IsCI)
            {
                Assert.Inconclusive("Skipping integration tests in CI environment");
                return;
            }
            string contractName = "ForceContract";
            string projectPath = Path.Combine(_testOutputPath, contractName);
            Directory.CreateDirectory(projectPath);
            File.WriteAllText(Path.Combine(projectPath, "existing.txt"), "test");

            var result = RunCompilerCommand($"new {contractName} --output \"{_testOutputPath}\" --force");

            Assert.IsTrue(result.Contains($"Successfully created Basic contract '{contractName}'"));
            Assert.IsTrue(File.Exists(Path.Combine(projectPath, $"{contractName}.cs")));
        }

        [TestMethod]
        public void TestNewCommandAllTemplates()
        {
            if (IsCI)
            {
                Assert.Inconclusive("Skipping integration tests in CI environment");
                return;
            }
            var templates = new[] { "Basic", "NEP17", "NEP11", "Ownable", "Oracle" };

            foreach (var template in templates)
            {
                string contractName = $"Test{template}";
                var result = RunCompilerCommand($"new {contractName} -t {template} --output \"{_testOutputPath}\"");

                Assert.IsTrue(result.Contains($"Creating {template} contract: {contractName}"));
                Assert.IsTrue(result.Contains($"Successfully created {template} contract"));

                string projectPath = Path.Combine(_testOutputPath, contractName);
                Assert.IsTrue(Directory.Exists(projectPath));
            }
        }

        [TestMethod]
        public void TestGeneratedContractCompilation()
        {
            if (IsCI)
            {
                Assert.Inconclusive("Skipping integration tests in CI environment");
                return;
            }
            string contractName = "CompilableContract";

            // Generate the contract
            var generateResult = RunCompilerCommand($"new {contractName} -t Basic --output \"{_testOutputPath}\"");
            Assert.IsTrue(generateResult.Contains($"Successfully created Basic contract"));

            // Try to compile the generated contract
            string projectPath = Path.Combine(_testOutputPath, contractName, $"{contractName}.csproj");
            var compileResult = RunCompilerCommand($"\"{projectPath}\"");

            // Check if compilation was successful
            Assert.IsTrue(compileResult.Contains("Compilation completed successfully") ||
                         compileResult.Contains($"Created {Path.Combine(_testOutputPath, contractName, "bin", "sc", $"{contractName}.nef")}"));
        }

        private string RunCompilerCommand(string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"run --project \"{_compilerPath}\" -- {arguments}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return output + error;
        }
    }
}
