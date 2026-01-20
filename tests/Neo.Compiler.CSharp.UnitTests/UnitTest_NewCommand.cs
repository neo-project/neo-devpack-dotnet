// Copyright (C) 2015-2026 The Neo Project.
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Neo.Compiler.CSharp.UnitTests
{
    [TestClass]
    public class UnitTest_NewCommand
    {
        private static readonly object ConsoleLock = new();
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
            UseLocalFrameworkReference(projectPath);
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
            var args = SplitArgs(arguments);
            var stdout = new StringWriter();
            var stderr = new StringWriter();
            int exitCode;

            lock (ConsoleLock)
            {
                var originalOut = Console.Out;
                var originalErr = Console.Error;
                try
                {
                    Console.SetOut(stdout);
                    Console.SetError(stderr);
                    exitCode = Program.Main(args);
                }
                finally
                {
                    Console.SetOut(originalOut);
                    Console.SetError(originalErr);
                }
            }

            return new CommandResult(exitCode, stdout.ToString(), stderr.ToString());
        }

        private void UseLocalFrameworkReference(string projectPath)
        {
            string compilerDirectory = Path.GetDirectoryName(_compilerPath)!;
            string repoRoot = Path.GetFullPath(Path.Combine(compilerDirectory, "..", ".."));
            string frameworkProject = Path.Combine(repoRoot, "src", "Neo.SmartContract.Framework", "Neo.SmartContract.Framework.csproj");
            if (!File.Exists(frameworkProject))
            {
                return;
            }

            string content = File.ReadAllText(projectPath);
            string updated = Regex.Replace(
                content,
                "<PackageReference\\s+Include=\"Neo\\.SmartContract\\.Framework\"\\s+Version=\"[^\"]+\"\\s*/>",
                $"<ProjectReference Include=\"{frameworkProject}\" />");

            if (updated != content)
            {
                File.WriteAllText(projectPath, updated);
            }
        }

        private sealed record CommandResult(int ExitCode, string StdOut, string StdErr);

        private static string[] SplitArgs(string commandLine)
        {
            var args = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            foreach (char c in commandLine)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    if (current.Length > 0)
                    {
                        args.Add(current.ToString());
                        current.Clear();
                    }
                }
                else
                {
                    current.Append(c);
                }
            }

            if (current.Length > 0)
            {
                args.Add(current.ToString());
            }

            return args.ToArray();
        }
    }
}
