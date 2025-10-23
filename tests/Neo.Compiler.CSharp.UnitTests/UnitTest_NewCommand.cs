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
using System.Diagnostics;
using System.IO;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_NewCommand
    {
        private string _testOutputPath = null!;
        private string _compilerPath = null!;

        [TestInitialize]
        public void TestSetup()
        {
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
            var result = RunCompilerCommand("new --help");

            Assert.AreEqual(0, result.ExitCode);
            StringAssert.Contains(result.StdOut, "Create a new smart contract from a template");
            StringAssert.Contains(result.StdOut, "--template");
            StringAssert.Contains(result.StdOut, "--author");
            StringAssert.Contains(result.StdOut, "--email");
            StringAssert.Contains(result.StdOut, "--description");
        }

        [TestMethod]
        public void TestNewCommandBasicContract()
        {
            string contractName = "TestBasic";
            var result = RunCompilerCommand($"new {contractName} -t Basic --output \"{_testOutputPath}\"");

            Assert.AreEqual(0, result.ExitCode);
            StringAssert.Contains(result.StdOut, $"Creating Basic contract: {contractName}");
            StringAssert.Contains(result.StdOut, $"Successfully created Basic contract '{contractName}'");

            string projectPath = Path.Combine(_testOutputPath, contractName);
            Assert.IsTrue(Directory.Exists(projectPath));
            Assert.IsTrue(File.Exists(Path.Combine(projectPath, $"{contractName}.cs")));
            Assert.IsTrue(File.Exists(Path.Combine(projectPath, $"{contractName}.csproj")));
        }

        [TestMethod]
        public void TestNewCommandNEP17Contract()
        {
            string contractName = "TestToken";
            var result = RunCompilerCommand($"new {contractName} -t NEP17 --output \"{_testOutputPath}\" --description \"Test Token Contract\"");

            Assert.AreEqual(0, result.ExitCode);
            StringAssert.Contains(result.StdOut, $"Creating NEP17 contract: {contractName}");
            StringAssert.Contains(result.StdOut, "Description: Test Token Contract");

            string csFilePath = Path.Combine(_testOutputPath, contractName, $"{contractName}.cs");
            Assert.IsTrue(File.Exists(csFilePath));

            string content = File.ReadAllText(csFilePath);
            Assert.IsTrue(content.Contains("Test Token Contract"));
            Assert.IsTrue(content.Contains("Nep17Token"));
        }

        [TestMethod]
        public void TestNewCommandWithCustomAuthor()
        {
            string contractName = "AuthorTest";
            var result = RunCompilerCommand($"new {contractName} --output \"{_testOutputPath}\" --author \"Jane Smith\" --email \"jane@test.com\"");

            Assert.AreEqual(0, result.ExitCode);
            StringAssert.Contains(result.StdOut, "Author: Jane Smith");
            StringAssert.Contains(result.StdOut, "Email: jane@test.com");

            string csFilePath = Path.Combine(_testOutputPath, contractName, $"{contractName}.cs");
            string content = File.ReadAllText(csFilePath);

            Assert.IsTrue(content.Contains("Jane Smith"));
            Assert.IsTrue(content.Contains("jane@test.com"));
        }

        [TestMethod]
        public void TestNewCommandInvalidName()
        {
            var result = RunCompilerCommand($"new 123Invalid --output \"{_testOutputPath}\"");

            Assert.AreEqual(1, result.ExitCode);
            StringAssert.Contains(result.StdErr, "Error: Contract name must start with a letter");
        }

        [TestMethod]
        public void TestNewCommandExistingDirectory()
        {
            string contractName = "ExistingContract";
            string projectPath = Path.Combine(_testOutputPath, contractName);
            Directory.CreateDirectory(projectPath);

            var result = RunCompilerCommand($"new {contractName} --output \"{_testOutputPath}\"");

            Assert.AreEqual(1, result.ExitCode);
            StringAssert.Contains(result.StdErr, $"Directory '{projectPath}' already exists");
            StringAssert.Contains(result.StdErr, "Use --force to overwrite");
        }

        [TestMethod]
        public void TestNewCommandForceOverwrite()
        {
            string contractName = "ForceContract";
            string projectPath = Path.Combine(_testOutputPath, contractName);
            Directory.CreateDirectory(projectPath);
            File.WriteAllText(Path.Combine(projectPath, "existing.txt"), "test");

            var result = RunCompilerCommand($"new {contractName} --output \"{_testOutputPath}\" --force");

            Assert.AreEqual(0, result.ExitCode);
            StringAssert.Contains(result.StdOut, $"Successfully created Basic contract '{contractName}'");
            Assert.IsTrue(File.Exists(Path.Combine(projectPath, $"{contractName}.cs")));
        }

        [TestMethod]
        public void TestNewCommandAllTemplates()
        {
            var templates = new[] { "Basic", "NEP17", "NEP11", "Ownable", "Oracle" };

            foreach (var template in templates)
            {
                string contractName = $"Test{template}";
                var result = RunCompilerCommand($"new {contractName} -t {template} --output \"{_testOutputPath}\"");

                Assert.AreEqual(0, result.ExitCode, $"Expected success for template {template}. Output: {result.StdOut}{result.StdErr}");
                StringAssert.Contains(result.StdOut, $"Creating {template} contract: {contractName}");
                StringAssert.Contains(result.StdOut, $"Successfully created {template} contract");

                string projectPath = Path.Combine(_testOutputPath, contractName);
                Assert.IsTrue(Directory.Exists(projectPath));
            }
        }

        [TestMethod]
        public void TestGeneratedContractCompilation()
        {
            string contractName = "CompilableContract";

            // Generate the contract
            var generateResult = RunCompilerCommand($"new {contractName} -t Basic --output \"{_testOutputPath}\"");
            Assert.AreEqual(0, generateResult.ExitCode);
            StringAssert.Contains(generateResult.StdOut, "Successfully created Basic contract");

            // Try to compile the generated contract
            string projectPath = Path.Combine(_testOutputPath, contractName, $"{contractName}.csproj");
            var compileResult = RunCompilerCommand($"\"{projectPath}\"");

            // Check if compilation was successful
            Assert.AreEqual(0, compileResult.ExitCode, $"Compilation failed. Output: {compileResult.StdOut}{compileResult.StdErr}");
            Assert.IsTrue(compileResult.StdOut.Contains("Compilation completed successfully", StringComparison.OrdinalIgnoreCase) ||
                         compileResult.StdOut.Contains($"Created {Path.Combine(_testOutputPath, contractName, "bin", "sc", $"{contractName}.nef")}", StringComparison.OrdinalIgnoreCase) ||
                         compileResult.StdErr.Contains("Compilation completed successfully", StringComparison.OrdinalIgnoreCase),
                         "Expected compilation success message.");
        }

        private CommandResult RunCompilerCommand(string arguments)
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

            return new CommandResult(process.ExitCode, output, error);
        }

        private sealed record CommandResult(int ExitCode, string StdOut, string StdErr);
    }
}
